using ApiAdmin.Domain.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace ApiAdmin.Infrastructure.Repositories;

internal class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128 bits
    private const int KeySize = 32; // 256 bits
    private const int Iterations = 10000; // 迭代次数
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;
    private const char SegmentDelimiter = ':';

    public string HashPassword(string password)
    {
        // 生成随机盐值
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        // 使用 PBKDF2 哈希密码
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithm,
            KeySize
        );

        // 将盐值和哈希值组合成一个字符串
        return string.Join(
            SegmentDelimiter,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash)
        );
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        // 从哈希字符串中提取盐值和哈希值
        var segments = hashedPassword.Split(SegmentDelimiter);
        var salt = Convert.FromBase64String(segments[0]);
        var hash = Convert.FromBase64String(segments[1]);

        // 对提供的密码进行哈希
        var providedHash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(providedPassword),
            salt,
            Iterations,
            HashAlgorithm,
            KeySize
        );

        // 比较哈希值
        return CryptographicOperations.FixedTimeEquals(hash, providedHash);
    }
}
