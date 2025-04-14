using System.Collections.Frozen;
using System.Text;
using System.Text.RegularExpressions;

namespace SyncIT.Sync.Utils;

public partial class EmailSanitizer
{
    private static readonly FrozenDictionary<char, string> ReplaceDictionary = new KeyValuePair<char, string>[]
    {
        new(' ', "-"),
        new('å', "a"),
        new('ä', "a"),
        new('ö', "o"),
        new('é', "e"),
        new('è', "e"),
        new('ê', "e"),
        new('ë', "e"),
        new('à', "a"),
        new('æ', "ae"),
        new('ø', "o"),
        new('ü', "ue"),
        new('ß', "ss"),
        new('µ', "u"),
        new('µ', "micro"),
        new('φ', "fi"),
        new('Φ', "fi"),
        new('Ω', "omega"),
        new('Σ', "sigma"),
        new('Δ', "delta"),
        new('Θ', "theta"),
        new('Λ', "lambda"),
        new('Π', "pi"),
        new('π', "pi"),
        new('Ψ', "psi"),
        new('¶', "para")
    }.ToFrozenDictionary();

    /// <summary>
    ///     Sanitizes the local part (left of @) of an email address.
    ///     Replaces some characters with ASCII equivalent and converts to lowercase.
    ///     Removes all other characters that are not letters, digits, '.', '-' or '+'
    /// </summary>
    /// <param name="localPart">The string to sanitize</param>
    /// <returns>The sanitized string. May be empty if it original only contained illegal characters.</returns>
    public static string SanitizeLocal(string localPart)
    {
        localPart = localPart.Trim().ToLowerInvariant();
        if (localPart.Length == 0 || LocalPartValidRegex().IsMatch(localPart))
            return localPart;

        StringBuilder sb = new(localPart.Length);

        foreach (var c in localPart)
            if (ReplaceDictionary.TryGetValue(c, out var replacement))
                sb.Append(replacement);
            else if ((sb.Length == 0 ? ValidFirstCharRegex() : ValidCharRegex()).IsMatch(c.ToString())) sb.Append(c);

        return sb.ToString().Trim('-');
    }

    [GeneratedRegex(@"^[a-z0-9][-+_.a-z0-9]*$")]
    private static partial Regex LocalPartValidRegex();

    [GeneratedRegex(@"[-+_.a-z0-9]")]
    private static partial Regex ValidCharRegex();

    [GeneratedRegex(@"[a-z0-9]")]
    private static partial Regex ValidFirstCharRegex();
}