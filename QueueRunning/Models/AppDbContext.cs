using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QueueRunning.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<QueueNumber> QueueNumbers { get; set; }
}
