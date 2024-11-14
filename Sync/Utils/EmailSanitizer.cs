using System.Collections.Frozen;
using System.Text.RegularExpressions;

namespace SyncIT.Sync.Utils;

public partial class EmailSanitizer
{
    private static readonly FrozenDictionary<string, string> _replaceDictionary = new KeyValuePair<string, string>[]
    {
        new(" ", "-"),
        new("ä", "a"),
        new("ö", "o"),
        new("ü", "ue"),
        new("ß", "ss"),
        new("€", "euro"),
        new("£", "pound"),
        new("¥", "yen"),
        new("§", "paragraph"),
        new("µ", "u"),
        new("µ", "micro"),
        new("φ", "fi"),
        new("Φ", "fi"),
        new("Ω", "omega"),
        new("Σ", "sigma"),
        new("Δ", "delta"),
        new("Θ", "theta"),
        new("Λ", "lambda"),
        new("Ξ", "xi"),
        new("Π", "pi"),
        new("π", "pi"),
        new("Ψ", "psi"),
        new("¶", "para"),
    }.ToFrozenDictionary();
    
    /// <summary>
    /// Sanitizes the local part (left of @) of an email address.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static string SanitizeLocal(string localPart)
    {
        if (LocalPartValidRegex().IsMatch(localPart))
            return localPart;
        
        return "";
        
    }

    [GeneratedRegex(@"^[a-z0-9]+[-+.a-z0-9]$")]
    private static partial Regex LocalPartValidRegex();
    


}