using FinexaApi.Models;

namespace FinexaApi.Services.Abstract
{
    public interface IAuthService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
        string GenerateJwtToken(User user);
    }
}
