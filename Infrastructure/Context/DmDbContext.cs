using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class DmDbContext : DbContext
{
    public DmDbContext(DbContextOptions<DmDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
}