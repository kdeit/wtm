﻿using Microsoft.EntityFrameworkCore;
using WTM.Models;

namespace WTM.LogDAL;

public class Log2024Context : DbContext
{
    public Log2024Context()
    {
    }

    public Log2024Context(DbContextOptions<Log2024Context> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Log>();
    }

    public DbSet<Log> Logs { get; set; }
}