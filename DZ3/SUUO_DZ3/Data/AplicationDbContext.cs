using Microsoft.EntityFrameworkCore;
using SUUO_DZ3.Models;

namespace SUUO_DZ3.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Narudzba> Narudzbe { get; set; }
        public DbSet<Konobar> Konobari { get; set; }
        public DbSet<Kuhar> Kuhari { get; set; }
        public DbSet<StavkaNarudzbe> StavkeNarudzbe { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Narudzba>(entity =>
            {
                entity.HasKey(n => n.NarudzbaId);
                entity.Property(n => n.NarudzbaId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Konobar>(entity =>
            {
                entity.HasKey(k => k.IdKonobar);
                entity.Property(k => k.IdKonobar).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Kuhar>(entity =>
            {
                entity.HasKey(k => k.IdKuhar);
                entity.Property(k => k.IdKuhar).ValueGeneratedOnAdd();

                // Specijaliteti se spremaju kao string odvojen ;
                entity.Property(k => k.Specijaliteti)
                    .HasConversion(
                        v => string.Join(";", v),
                        v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList());
            });

            modelBuilder.Entity<StavkaNarudzbe>(entity =>
            {
                entity.HasKey(s => s.StavkaNarudzbeId);
                entity.Property(s => s.StavkaNarudzbeId).ValueGeneratedOnAdd();
            });
        }
    }
}
