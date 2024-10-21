using Microsoft.EntityFrameworkCore;
using WTM.Models;

namespace WTM.ReaderDAL;

public class ReaderContext : DbContext
{
    public ReaderContext()
    {
    }

    public ReaderContext(DbContextOptions<ReaderContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>();
        modelBuilder.Entity<GroupMembers>();
        modelBuilder.Entity<Company>();
    }

    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMembers> GroupMembers { get; set; }
    public DbSet<Company> Companies { get; set; }
}