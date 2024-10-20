using Microsoft.EntityFrameworkCore;
using WTM.Models;

namespace WTM.Client;

public class ClientContext : DbContext
{
    public ClientContext()
    {
    }

    public ClientContext(DbContextOptions<ClientContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Company>();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Company> Companies { get; set; }
}