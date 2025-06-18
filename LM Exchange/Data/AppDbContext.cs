using LM_Exchange.Model;
using Microsoft.EntityFrameworkCore;

namespace LM_Exchange.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Client> Client { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<WalletBalance> WalletBalances { get; set; }


    }
}
