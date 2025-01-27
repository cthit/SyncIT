using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SyncIT.Sync.Models;

/// <summary>
///     Represents a valid email address.
/// </summary>
[JsonConverter(typeof(EmailSerializer))]
public sealed partial record EmailAddress : IEquatable<EmailAddress>
{

    private readonly string _email = null!;

    public EmailAddress(string email)
    {
        email = Format(email);
        if (!ValidEmailRegex().IsMatch(email)) throw new ArgumentException($"Invalid email address '{email}'");
        Email = email;
    }

    public string Email
    {
        get => _email;
        init
        {
            _email = Format(value);
            if (!ValidEmailRegex().IsMatch(_email)) throw new ArgumentException($"Invalid email address '{value}'");
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

    public bool Equals(EmailAddress? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _email == other._email;
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
        return ValidEmailRegex().IsMatch(Format(email));
    }
    
    private sealed class EmailEqualityComparer : IEqualityComparer<EmailAddress>
    {
        public bool Equals(EmailAddress? x, EmailAddress? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x._email == y._email;
        }

        public int GetHashCode(EmailAddress obj)
        {
            return obj._email.GetHashCode();
        }
    }

    public static IEqualityComparer<EmailAddress> EmailComparer { get; } = new EmailEqualityComparer();

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