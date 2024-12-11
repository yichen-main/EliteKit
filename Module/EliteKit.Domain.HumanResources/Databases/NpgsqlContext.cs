using EliteKit.Domain.HumanResources.Databases.Users;
using Microsoft.EntityFrameworkCore;

namespace EliteKit.Domain.HumanResources.Databases;
public partial class NpgsqlContext(DbContextOptions<NpgsqlContext> options) : DbContext(options)
{
    public DbSet<UserRegistration> UserRegistrations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}