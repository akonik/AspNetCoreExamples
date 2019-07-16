using AspNetCoreExamples.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreExamples.EntityFramework
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ApplicationUser> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {

        }
    }
}
