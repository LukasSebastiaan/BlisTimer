using System.Security.Cryptography;

namespace ConsoleApp1;

public sealed class PHasher
{
    private const int saltSize = 16;
    private const int hashSize = 32;
    private OptionsHash options;
    
    public PHasher(OptionsHash options)
    {
        this.options = new OptionsHash(options.Iterations);
    }
    
    public string Hash(string password)
    {
        var algorithm = new Rfc2898DeriveBytes(password, saltSize, options.Iterations, HashAlgorithmName.SHA256);
        return $"{algorithm.IterationCount}.{Convert.ToBase64String(algorithm.Salt)}.{Convert.ToBase64String(algorithm.GetBytes(hashSize))}";
    }
    
    public (bool outdated, bool verified) Check(string hash, string password)
    {
        var split = hash.Split('.');
        if (split.Length != 3)
        {
            throw new FormatException("Unexpected hash format. Should be formatted as `{iterations}.{salt}.{hash}`");
        }

        var outdated = options.Iterations != int.Parse(split[0]); 
        var algorithm = new Rfc2898DeriveBytes(password, Convert.FromBase64String(split[1]), Convert.ToInt32(split[0]), HashAlgorithmName.SHA256);
        var keyToCheck = algorithm.GetBytes(hashSize);
        var hashToCheck = Convert.ToBase64String(keyToCheck);
        
        bool verified = hashToCheck == split[2];
        
        return (outdated, verified);
    }
}