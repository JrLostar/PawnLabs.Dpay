namespace PawnLabs.Dpay.Core.Helper
{
    public interface IMailHelper
    {
        Task<string> SendVerificationEmail(string toEmail);
    }
}