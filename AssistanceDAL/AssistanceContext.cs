using Microsoft.EntityFrameworkCore;
using WTM.Models;

namespace WTM.AssistanceDAL;

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
        modelBuilder.Entity<Incident>();
    }

    public DbSet<Group> Incidents { get; set; }
}