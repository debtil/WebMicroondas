using System.Security.Cryptography;
using System.Text;

namespace WebMicroondas.Infra.Security
{
    public static class PasswordHasher
    {
        public static string Sha256(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();
        }
    }
}
