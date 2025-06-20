namespace PawnLabs.Dpay.Core.Configuration
{
    public class TokenConfiguration
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string SecretKey { get; set; }

        public int ExpireTime { get; set; }
    }
}
