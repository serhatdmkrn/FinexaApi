using Microsoft.EntityFrameworkCore;
using FinexaApi.Models;

namespace FinexaApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<WalletItem> WalletItems { get; set; }
    }

}
