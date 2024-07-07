namespace RouteManagement.Dtos
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string MerchantId { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiredAt { get; set; }

    }
}
