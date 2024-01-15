using System.Security.Cryptography;

namespace Cafe.POS.Services;
// <summary>
/// Provides utility methods for common tasks in the Cafe POS application.
/// </summary>
public class UtilityService
{

    private const char SegmentDelimiter = ':';
        
    public static string HashSecret(string input)
    {
        var saltSize = 16;
        var iterations = 100_000;
        var keySize = 32;
        var algorithm = HashAlgorithmName.SHA256;
        var salt = RandomNumberGenerator.GetBytes(saltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, algorithm, keySize);

        var result = string.Join(
            SegmentDelimiter,
            Convert.ToHexString(hash),
            Convert.ToHexString(salt),
            iterations,
            algorithm
        );

        return result;
    }

    public static bool VerifyHash(string input, string hashString)
    {
        var segments = hashString.Split(SegmentDelimiter);
        var hash = Convert.FromHexString(segments[0]);
        var salt = Convert.FromHexString(segments[1]);
        var iterations = int.Parse(segments[2]);
        var algorithm = new HashAlgorithmName(segments[3]);
        var inputHash = Rfc2898DeriveBytes.Pbkdf2(
            input,
            salt,
            iterations,
            algorithm,
            hash.Length
        );

        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }

    // <summary>
    /// Gets the path to the application's root directory.
    /// </summary>
    /// <returns>The path to the root directory.</returns>

    public static string GetAppDirectoryPath()
    {
        return @"C:\Users\nisch\Desktop\Cafe-POS\Cafe.POS\wwwroot";
    }
    /// <returns>The file path for users.json.</returns>
    public static string GetAppUsersFilePath()
    {
        return Path.Combine(GetAppDirectoryPath(), "users.json");
    }

    public static string GetAppCustomersFilePath()
    {
        return Path.Combine(GetAppDirectoryPath(), "customers.json");
    }
    
    public static string GetAppCoffeesFilePath()
    {
        return Path.Combine(GetAppDirectoryPath(), "coffees.json");
    }
    
    public static string GetAppAddInsFilePath()
    {
        return Path.Combine(GetAppDirectoryPath(), "add-ins.json");
    }

    public static string GetAppOrdersFilePath()
    {
        return Path.Combine(GetAppDirectoryPath(), "orders.json");
    }
    
    public static string GetAppOrderAddInsFilePath()
    {
        return Path.Combine(GetAppDirectoryPath(), "order-add-ins.json");
    }
}