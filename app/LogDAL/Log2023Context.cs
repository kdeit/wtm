using Microsoft.EntityFrameworkCore;
using WTM.Models;

namespace WTM.LogDAL;

public class Log2023Context : DbContext
{
    public Log2023Context()
    {
    }

    public Log2023Context(DbContextOptions<Log2023Context> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Log>();
    }

    public DbSet<Log> Logs { get; set; }
}