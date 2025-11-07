using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using Serilog;
using SyncIT.Sync;
using SyncIT.Sync.Services;
using SyncIT.Web.Blazor;
using SyncIT.Web.Database;
using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

namespace SyncIT.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Host.UseSerilog();

        var databasePath = builder.Configuration.GetValue<string>("DATABASE_PATH") ?? "syncit.db";

        builder.Services.AddDbContext<SyncItContext>(options =>
            options.UseSqlite($"Data Source={databasePath}"));

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

            var knownNetworksRaw = builder.Configuration.GetValue<string?>("ForwardedHeaders:KnownNetworks");
            if (!string.IsNullOrWhiteSpace(knownNetworksRaw))
            {
                var entries = knownNetworksRaw.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var networks = new List<IPNetwork>();
                foreach (var n in entries)
                {
                    var txt = n.Trim();
                    if (string.IsNullOrWhiteSpace(txt))
                        continue;

                    try
                    {
                        networks.Add(IPNetwork.Parse(txt));
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Warning("Failed to parse forwarded header known network '{Network}': {Message}", txt,
                            ex.Message);
                    }
                }

                options.KnownNetworks.Clear();
                foreach (var net in networks) options.KnownNetworks.Add(net);
            }

            var knownProxiesRaw = builder.Configuration.GetValue<string?>("ForwardedHeaders:KnownProxies");
            if (!string.IsNullOrWhiteSpace(knownProxiesRaw))
            {
                var entries = knownProxiesRaw.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var proxies = new List<IPAddress>();
                foreach (var p in entries)
                {
                    var txt = p.Trim();
                    if (string.IsNullOrWhiteSpace(txt))
                        continue;

                    if (IPAddress.TryParse(txt, out var ip))
                        proxies.Add(ip);
                    else
                        Log.Logger.Warning("Failed to parse forwarded header known proxy '{Proxy}'", txt);
                }

                options.KnownProxies.Clear();
                foreach (var ip in proxies) options.KnownProxies.Add(ip);
            }
        });

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                builder.Configuration.GetSection("OIDC").Bind(options);

                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.PushedAuthorizationBehavior = PushedAuthorizationBehavior.Disable;

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                };
            });

        builder.Services.AddMudServices();

        builder.Services.AddHttpClient();

        builder.Services.AddSingleton<BitwardenSync>();

        builder.Services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        builder.Services.AddRazorPages().AddMvcOptions(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        });

        builder.Services.AddControllersWithViews(options =>
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddSingleton<ChangeResolver>();

        var app = builder.Build();

        await using (var dbContext =
                     app.Services.CreateScope().ServiceProvider.GetRequiredService<SyncItContext>())
        {
            await dbContext.Database.MigrateAsync();
        }

        app.UseForwardedHeaders();

        app.UseSerilogRequestLogging(o =>
        {
            o.EnrichDiagnosticContext = (context, httpContext) =>
            {
                context.Set("RequestHost", httpContext.Request.Host.Value ?? "unknown");
            };
        });

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Logger.LogInformation(
            "It is fine to ignore the data protection warnings on first run as it is only used for session cookies.");
        await app.RunAsync();
    }
}