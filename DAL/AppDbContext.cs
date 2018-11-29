using Barron_Amanda_HW5.Models;
using Microsoft.EntityFrameworkCore;

namespace Barron_Amanda_HW5.DAL
{
      public class AppDbContext : DbContext
    {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    //You need one db set for each model class. For example:
    public DbSet<Language> Languages { get; set; }
    public DbSet<Repository> Repositories { get; set; }
    }
}