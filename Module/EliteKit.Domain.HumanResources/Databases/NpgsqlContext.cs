using System.Reflection;
using EliteKit.Domain.HumanResources.Databases.Users;
using Microsoft.EntityFrameworkCore;

namespace EliteKit.Domain.HumanResources.Databases;
public partial class NpgsqlContext(DbContextOptions<NpgsqlContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //專案內有 IEntityTypeConfiguration<> 反射實作
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    public DbSet<UserRegistration> UserRegistrations { get; set; }
}