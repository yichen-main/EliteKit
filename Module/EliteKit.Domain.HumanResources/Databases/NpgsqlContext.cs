using Microsoft.EntityFrameworkCore;

namespace EliteKit.Domain.HumanResources.Databases;

public partial class NpgsqlContext(DbContextOptions<NpgsqlContext> options) : DbContext(options)
{
    public DbSet<User> Entity1 { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    
    }
}