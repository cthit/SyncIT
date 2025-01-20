using System.Security.Cryptography;

namespace SyncIT.Sync.Utils;

public static class PasswordGenerator
{
    public static string GeneratePassword()
    {
        const string passwordCharactersUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string passwordCharactersLower = "abcdefghijklmnopqrstuvwxyz";
        const string passwordCharactersNumbers = "0123456789";
        const string passwordCharactersSpecial = "!@#$%^&*()_+";

        var password =
            RandomNumberGenerator.GetString(passwordCharactersUpper, RandomNumberGenerator.GetInt32(5, 10));
        password += RandomNumberGenerator.GetString(passwordCharactersLower, RandomNumberGenerator.GetInt32(5, 10));
        password += RandomNumberGenerator.GetString(passwordCharactersNumbers,
            RandomNumberGenerator.GetInt32(3, 5));
        password += RandomNumberGenerator.GetString(passwordCharactersSpecial,
            RandomNumberGenerator.GetInt32(1, 5));
        var passwordArr = password.ToCharArray();
        RandomNumberGenerator.Shuffle<char>(passwordArr);
        password = new string(passwordArr);
        return password;
    }
}