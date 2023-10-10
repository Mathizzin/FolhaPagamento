using Microsoft.EntityFrameworkCore;
using MinhaWebAPI.Models;


namespace MinhaWebAPI.data
{
    public class FolhaContext : DbContext
    {
        public FolhaContext(DbContextOptions<FolhaContext> options)
            : base(options)
        {
        }

        public DbSet<FolhaModel> Folhas { get; set; }
        public DbSet<FuncionarioModel> Funcionarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FolhaModel>()
                .HasKey(f => f.FolhaId);
        }
    }
}