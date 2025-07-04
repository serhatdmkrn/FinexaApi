using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinexaApi.Data;
using FinexaApi.Models;
using FinexaApi.Models.RequestModel;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinexaApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WalletController(ApplicationDbContext context)
        {
            _context = context;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("nameidentifier")?.Value
                              ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("Kullanıcı doğrulanamadı.");

            return new Guid(userIdClaim);
        }

        [HttpGet]
        public async Task<IActionResult> GetWallet()
        {
            var userId = GetCurrentUserId();
                
            var varliklar = await _context.WalletItems
                .Where(v => v.UserId == userId)
                .ToListAsync();

            return Ok(varliklar);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            var userId = GetCurrentUserId();

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            user.PasswordHash = null;
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalletItem([FromBody] WalletItemCreateRequestModel model)
        {
            var userId = GetCurrentUserId();

            var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            DateTime nowInTurkey = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, turkeyTimeZone);

            var walletItem = new WalletItem
            {
                UserId = userId,
                Type = model.Type,
                Quantity = model.Quantity,
                PurchasePrice = model.PurchasePrice,
                Date = model.Date ?? nowInTurkey
            };

            _context.WalletItems.Add(walletItem);
            await _context.SaveChangesAsync();

            return Ok(walletItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWalletItem(Guid id, [FromBody] WalletItemUpdateRequestModel model)
        {
            var userId = GetCurrentUserId();

            var walletItem = await _context.WalletItems.FirstOrDefaultAsync(v => v.Id == id && v.UserId == userId);

            if (walletItem == null)
                return NotFound("Varlık bulunamadı.");

            walletItem.Quantity = model.Quantity;
            walletItem.PurchasePrice = model.PurchasePrice;

            await _context.SaveChangesAsync();

            return Ok(walletItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVarlik(Guid id)
        {
            var userId = GetCurrentUserId();

            var varlik = await _context.WalletItems.FirstOrDefaultAsync(v => v.Id == id && v.UserId == userId);

            if (varlik == null)
                return NotFound("Varlık bulunamadı.");

            _context.WalletItems.Remove(varlik);
            await _context.SaveChangesAsync();

            return Ok("Varlık başarıyla silindi.");
        }
    }
}
