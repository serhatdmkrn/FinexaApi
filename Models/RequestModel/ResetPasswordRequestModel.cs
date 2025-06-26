namespace FinexaApi.Models.RequestModel
{
    public class ResetPasswordRequestModel
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
