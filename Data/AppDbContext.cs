using Microsoft.EntityFrameworkCore;
using TreeNodes.Models;

namespace TreeNodes.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Tree> Trees { get; set; }
        public DbSet<Node> Nodes { get; set; }
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {         
            modelBuilder.Entity<Tree>()       
                .HasMany(t => t.Nodes)       
                .WithOne(n => n.Tree)       
                .HasForeignKey(n => n.TreeId)     
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
