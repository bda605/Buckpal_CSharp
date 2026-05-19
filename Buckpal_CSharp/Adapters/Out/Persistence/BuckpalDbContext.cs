using Microsoft.EntityFrameworkCore;

namespace Buckpal_CSharp.Adapters.Out.Persistence;

public class BuckpalDbContext : DbContext
{
    public BuckpalDbContext(DbContextOptions<BuckpalDbContext> options)
        : base(options)
    {
    }

    public DbSet<AccountJpaEntity> Accounts { get; set; }
    public DbSet<ActivityJpaEntity> Activities { get; set; }
}
