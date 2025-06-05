using BulkyWebRazor.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyWebRazor.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
    }
}
