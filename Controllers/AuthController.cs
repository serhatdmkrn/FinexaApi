using Microsoft.AspNetCore.Mvc;
using FinexaApi.Data;
using FinexaApi.Models;
using FinexaApi.Models.RequestModel;
using Microsoft.AspNetCore.Authorization;
using FinexaApi.Services.Abstract;
using System.Security.Claims;

namespace FinexaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public AuthController(
            IAuthService authService,
            ApplicationDbContext context,
            IEmailService emailService)
        {
            _authService = authService;
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel model)
        {
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                return BadRequest(new { message = "Bu e-posta zaten kayıtlı." });
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = _authService.HashPassword(model.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kayıt başarılı." });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

            if (user == null || !_authService.VerifyPassword(model.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Geçersiz e-posta veya şifre." });
            }

            var token = _authService.GenerateJwtToken(user);

            return Ok(new { token });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });

            var resetToken = Guid.NewGuid().ToString();
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            var resetLink = $"https://serhatdmkrn.github.io/finexa-app/#/reset-password/{resetToken}";

            var success = await _emailService.SendPasswordResetEmailAsync(model.Email, resetLink);
            if (!success)
            {
                return StatusCode(500, new { message = "E-posta gönderilirken hata oluştu." });
            }

            return Ok(new { message = "Şifre sıfırlama bağlantısı gönderildi." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.PasswordResetToken == model.Token);

            if (user == null || user.PasswordResetTokenExpires < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Kullanılmış veya süresi dolmuş link. Lütfen yeni bir link talep ediniz." });
            }

            user.PasswordHash = _authService.HashPassword(model.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Şifreniz başarıyla güncellendi." });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestModel model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Geçersiz kullanıcı kimliği." });

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });

            if (!_authService.VerifyPassword(model.CurrentPassword, user.PasswordHash))
            {
                return BadRequest(new { message = "Mevcut şifre yanlış." });
            }

            user.PasswordHash = _authService.HashPassword(model.NewPassword);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Şifreniz başarıyla güncellendi." });
        }
    }
}
