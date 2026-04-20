using System;
using Microsoft.EntityFrameworkCore;
using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Infraestructura.Persistencia;

public class ApplicationDbContext : DbContext

{

    public DbSet<Usuario> Usuarios { get; set; }

    public DbSet<Reserva> Reservas { get; set; }

    public DbSet<Resena> Resenas { get; set; }

    public DbSet<FechaBloqueada> FechasBloqueadas { get; set; }

    public DbSet<Notificacion> Notificaciones { get; set; }

    public DbSet<Propiedad> Propiedades { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)


    {
        base.OnModelCreating(modelBuilder);
        

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique ();
        
        modelBuilder.Entity<Propiedad>()
            .Property(x=>x.PrecioPorNoche)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Propiedad>()
            .Property(x=>x.Version)
            .IsRowVersion();

        modelBuilder.Entity<Reserva>()
            .Property(x=>x.Estado)
            .HasConversion<string>();

        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Propiedad)
            .WithMany() // Usamos WithMany vacío ya que Propiedad no tiene colección de Reservas
            .HasForeignKey(r => r.PropiedadId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.UsuarioInvitado)
            .WithMany(u => u.Reservas) // Usamos la colección existente en Usuario
            .HasForeignKey(r => r.UsuarioInvitadoId)
            .OnDelete(DeleteBehavior.NoAction);   

    }

}
