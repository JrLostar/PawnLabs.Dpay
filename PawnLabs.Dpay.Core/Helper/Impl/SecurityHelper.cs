using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PawnLabs.Dpay.Core.Configuration;
using PawnLabs.Dpay.Core.Enum;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PawnLabs.Dpay.Core.Helper.Impl
{
    public class SecurityHelper : ISecurityHelper
    {
        private static readonly byte[] EncryptionKey = Encoding.UTF8.GetBytes("@1B2c3D4e5F6g7H8");

        private TokenConfiguration tokenConfiguration;

        public SecurityHelper(IOptions<TokenConfiguration> tokenConfiguration)
        {
            this.tokenConfiguration = tokenConfiguration.Value;
        }

        public string GenerateToken(string email, EnumApplication application)
        {
            var claims = new List<Claim>() { new Claim("Email", email), new Claim("Application", ((int)application).ToString())};

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = tokenConfiguration.Issuer,
                Audience = tokenConfiguration.Audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(tokenConfiguration.ExpireTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenConfiguration.SecretKey)), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var securityTokenValue = tokenHandler.WriteToken(securityToken);

            return securityTokenValue;
        }

        public string Hash(string input)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha512.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public string EncryptString(string plainText)
        {
            byte[] nonce = new byte[12];
            RandomNumberGenerator.Fill(nonce);

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = new byte[plainBytes.Length];
            byte[] tag = new byte[16];

            using (var aes = new AesGcm(EncryptionKey))
            {
                aes.Encrypt(nonce, plainBytes, cipherBytes, tag, associatedData: null);
            }

            byte[] result = new byte[nonce.Length + cipherBytes.Length + tag.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(cipherBytes, 0, result, nonce.Length, cipherBytes.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length + cipherBytes.Length, tag.Length);

            return Convert.ToBase64String(result);
        }

        public string DecryptString(string encryptedBase64)
        {
            byte[] fullCipher = Convert.FromBase64String(encryptedBase64);

            byte[] nonce = new byte[12];
            byte[] tag = new byte[16];
            int cipherLength = fullCipher.Length - nonce.Length - tag.Length;
            byte[] cipherBytes = new byte[cipherLength];

            Buffer.BlockCopy(fullCipher, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(fullCipher, nonce.Length, cipherBytes, 0, cipherLength);
            Buffer.BlockCopy(fullCipher, nonce.Length + cipherLength, tag, 0, tag.Length);

            byte[] plainBytes = new byte[cipherLength];
            using (var aes = new AesGcm(EncryptionKey))
            {
                aes.Decrypt(nonce, cipherBytes, tag, plainBytes, associatedData: null);
            }

            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
