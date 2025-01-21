using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SyncIT.Sync.Models;

/// <summary>
///     Represents a valid email address.
/// </summary>
[JsonConverter(typeof(EmailSerializer))]
public partial class EmailAddress
{
    private readonly string _email = null!;

    public EmailAddress(string email)
    {
        email = Format(email);
        if (!IsValid(email)) throw new ArgumentException($"Invalid email address '{email}'");
        Email = email;
    }

    public string Email
    {
        get => _email;
        init
        {
            _email = Format(value);
            if (!IsValid(_email)) throw new ArgumentException($"Invalid email address '{value}'");
        }
    }

    /**
     * public bool Equals(EmailAddress? other)
     * {
     * if (other is null) return false;
     * if (ReferenceEquals(this, other)) return true;
     * return _email == other._email;
     * }
     */
    [return: NotNullIfNotNull("emailAddress")]
    public static implicit operator string?(EmailAddress? emailAddress)
    {
        return emailAddress?.Email!;
    }

    public static implicit operator EmailAddress(string email)
    {
        return new EmailAddress(email);
    }

    public override string ToString()
    {
        return Email;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((EmailAddress)obj);
    }

    public override int GetHashCode()
    {
        return _email.GetHashCode();
    }

    private static string Format(string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    public static bool IsValid(string email)
    {
        return ValidEmailRegex().IsMatch(email);
    }

    [GeneratedRegex(@"^[a-z0-9][-+_.a-z0-9]*@[a-z0-9][-.a-z0-9]*\.[a-z0-9][-.a-z0-9]*$")]
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