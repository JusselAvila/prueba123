namespace EventosAPI.Data; 

using Microsoft.EntityFrameworkCore;
using EventosAPI.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Evento> Eventos { get; set; }
    public DbSet<Inscripcion> Inscripciones { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {   

        modelBuilder.Entity<Evento>()
            .HasOne(e => e.Creador)
            .WithMany(u => u.EventosCreados)
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<Inscripcion>()
            .HasOne(i => i.Usuario)
            .WithMany(u => u.Inscripciones)
            .HasForeignKey(i => i.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Inscripcion>()
            .HasOne(i => i.Evento)
            .WithMany(e => e.Inscripciones)
            .HasForeignKey(i => i.EventoId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}