using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SyncIT.Sync.Models;


/// <summary>
/// Represents a valid email address.
/// </summary>
[JsonConverter(typeof(EmailSerializer))]
public partial class EmailAddress
{
    private readonly string _email = null!;
    public string Email
    {
        get => _email;
        init
        {
            if (!IsValid(value))
            {
                throw new ArgumentException($"Invalid email address '{value}'");
            }
            _email = Format(value);
        }
    }
    
    public EmailAddress(string email)
    {
        Email = email;
    }
    
    public static implicit operator string(EmailAddress emailAddress) => emailAddress.Email;
    
    public static implicit operator EmailAddress(string email) => new(email);
    
    public override string ToString() => Email;
    
    private static string Format(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
    
    private static bool IsValid(string email)
    {
        return ValidEmailRegex().IsMatch(email);
    }

    [GeneratedRegex(@"^[a-z0-9]+[-+.a-z0-9]*@[a-z0-9]+[-.a-z0-9]*\.[a-z0-9]+[-.a-z0-9]*$")]
    private static partial Regex ValidEmailRegex();
    
}

public class EmailSerializer : JsonConverter<EmailAddress>
{
    public override EmailAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new EmailAddress(reader.GetString() ?? throw new InvalidOperationException());
    }

    public override void Write(Utf8JsonWriter writer, EmailAddress value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Email);
    }
}