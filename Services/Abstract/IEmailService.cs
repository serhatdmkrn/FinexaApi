namespace FinexaApi.Services.Abstract
{
    public interface IEmailService
    {
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }
}
