using Microsoft.EntityFrameworkCore;
using WTM.Models;

namespace WTM.AssistanceDAL;

public class AssistanceContext : DbContext
{
    public AssistanceContext()
    {
    }

    public AssistanceContext(DbContextOptions<AssistanceContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Incident>();
    }

    public DbSet<Group> Incidents { get; set; }
}