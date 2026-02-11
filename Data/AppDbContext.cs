using ApiMovies.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiMovies.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies => Set<Movie>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Movie>(entity =>
        {
            // Nome da tabela
            entity.ToTable("movies");

            // Chave primária
            entity.HasKey(m => m.Id);

            // Propriedades
            entity.Property(m => m.Id)
                  .HasColumnName("id");

            entity.Property(m => m.Title)
                  .HasColumnName("title")
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(m => m.Director)
                  .HasColumnName("director")
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(m => m.ReleaseYear)
                  .HasColumnName("release_year")
                  .IsRequired();
        });
    }
}
