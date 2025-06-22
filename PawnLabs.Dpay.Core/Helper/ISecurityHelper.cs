using PawnLabs.Dpay.Core.Enum;

namespace PawnLabs.Dpay.Core.Helper
{
    public interface ISecurityHelper
    {
        string GenerateToken(string email, EnumApplication application);

        string Hash(string input);

        string EncryptString(string plainText);

        string DecryptString(string encryptedBase64);
    }
}