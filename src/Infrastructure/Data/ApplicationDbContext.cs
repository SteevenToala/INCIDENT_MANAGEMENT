using Microsoft.EntityFrameworkCore;
using IncidentManagement.Domain.Entities;

namespace IncidentManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Rol> Roles { get; set; }
    public DbSet<Facultad> Facultades { get; set; }
    public DbSet<Laboratorio> Laboratorios { get; set; }
    public DbSet<Computadora> Computadoras { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<CatalogoServicio> CatalogoServicios { get; set; }
    public DbSet<ContratoSLA> ContratosSLA { get; set; }
    public DbSet<Incidente> Incidentes { get; set; }
    public DbSet<AsignacionIncidente> AsignacionesIncidentes { get; set; }
    public DbSet<ActualizacionIncidente> ActualizacionesIncidentes { get; set; }
    public DbSet<BaseConocimiento> BaseConocimientos { get; set; }
    public DbSet<Notificacion> Notificaciones { get; set; }
    public DbSet<SuscripcionPush> SuscripcionesPush { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Roles
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(e => e.RolID);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        // Configuración de Facultades
        modelBuilder.Entity<Facultad>(entity =>
        {
            entity.ToTable("Facultades");
            entity.HasKey(e => e.FacultadID);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");
        });

        // Configuración de Laboratorios
        modelBuilder.Entity<Laboratorio>(entity =>
        {
            entity.ToTable("Laboratorios");
            entity.HasKey(e => e.LaboratorioID);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");
            
            entity.HasOne(e => e.Facultad)
                .WithMany(f => f.Laboratorios)
                .HasForeignKey(e => e.FacultadID)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Configuración de Computadoras
        modelBuilder.Entity<Computadora>(entity =>
        {
            entity.ToTable("Computadoras");
            entity.HasKey(e => e.ComputadoraID);
            entity.Property(e => e.CodigoEquipo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.FechaActualizacion).HasDefaultValueSql("GETDATE()");
            entity.HasIndex(e => e.CodigoEquipo).IsUnique();

            entity.HasOne(e => e.Laboratorio)
                .WithMany(l => l.Computadoras)
                .HasForeignKey(e => e.LaboratorioID)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Configuración de Usuarios
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(e => e.UsuarioID);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.EsAsignador).HasDefaultValue(false);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("GETDATE()");
            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasOne(e => e.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(e => e.RolID)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.Facultad)
                .WithMany(f => f.Usuarios)
                .HasForeignKey(e => e.FacultadID)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de CatalogoServicios
        modelBuilder.Entity<CatalogoServicio>(entity =>
        {
            entity.ToTable("CatalogoServicios");
            entity.HasKey(e => e.ServicioID);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.FechaActualizacion).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.ResponsableUsuario)
                .WithMany(u => u.ServiciosResponsable)
                .HasForeignKey(e => e.ResponsableUsuarioID)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de ContratosSLA
        modelBuilder.Entity<ContratoSLA>(entity =>
        {
            entity.ToTable("ContratosSLA");
            entity.HasKey(e => e.SLA_ID);
            entity.Property(e => e.Disponibilidad).HasDefaultValue(99);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Servicio)
                .WithMany(s => s.ContratosSLA)
                .HasForeignKey(e => e.ServicioID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de Incidentes
        modelBuilder.Entity<Incidente>(entity =>
        {
            entity.ToTable("Incidentes");
            entity.HasKey(e => e.IncidenteID);
            entity.Property(e => e.CodigoIncidente).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Prioridad).HasDefaultValue("Media");
            entity.Property(e => e.Estado).HasDefaultValue("Abierto");
            entity.Property(e => e.Eliminado).HasDefaultValue(false);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.FechaActualizacion).HasDefaultValueSql("GETDATE()");
            entity.HasIndex(e => e.CodigoIncidente).IsUnique();

            entity.HasOne(e => e.Computadora)
                .WithMany(c => c.Incidentes)
                .HasForeignKey(e => e.ComputadoraID)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.UsuarioReportador)
                .WithMany(u => u.IncidentesReportados)
                .HasForeignKey(e => e.UsuarioReportadorID)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.Facultad)
                .WithMany(f => f.Incidentes)
                .HasForeignKey(e => e.FacultadID)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Laboratorio)
                .WithMany(l => l.Incidentes)
                .HasForeignKey(e => e.LaboratorioID)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Servicio)
                .WithMany(s => s.Incidentes)
                .HasForeignKey(e => e.ServicioID)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de AsignacionesIncidentes
        modelBuilder.Entity<AsignacionIncidente>(entity =>
        {
            entity.ToTable("AsignacionesIncidentes");
            entity.HasKey(e => e.AsignacionID);
            entity.Property(e => e.EstadoAsignacion).HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaAsignacion).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Incidente)
                .WithMany(i => i.Asignaciones)
                .HasForeignKey(e => e.IncidenteID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.UsuarioAsignado)
                .WithMany(u => u.Asignaciones)
                .HasForeignKey(e => e.UsuarioAsignadoID)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de ActualizacionesIncidentes
        modelBuilder.Entity<ActualizacionIncidente>(entity =>
        {
            entity.ToTable("ActualizacionesIncidentes");
            entity.HasKey(e => e.ActualizacionID);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Incidente)
                .WithMany(i => i.Actualizaciones)
                .HasForeignKey(e => e.IncidenteID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.Actualizaciones)
                .HasForeignKey(e => e.UsuarioID)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Configuración de BaseConocimientos
        modelBuilder.Entity<BaseConocimiento>(entity =>
        {
            entity.ToTable("BaseConocimientos");
            entity.HasKey(e => e.SolucionID);
            entity.Property(e => e.Efectividad).HasDefaultValue(0);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.FechaActualizacion).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.IncidenteRelacionado)
                .WithMany(i => i.Conocimientos)
                .HasForeignKey(e => e.IncidenteRelacionadoID)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.UsuarioCreador)
                .WithMany(u => u.ConocimientosCreados)
                .HasForeignKey(e => e.UsuarioCreadorID)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Configuración de Notificaciones
        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.ToTable("Notificaciones");
            entity.HasKey(e => e.NotificacionID);
            entity.Property(e => e.Leida).HasDefaultValue(false);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.Notificaciones)
                .HasForeignKey(e => e.UsuarioID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Incidente)
                .WithMany(i => i.Notificaciones)
                .HasForeignKey(e => e.IncidenteID)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de SuscripcionesPush
        modelBuilder.Entity<SuscripcionPush>(entity =>
        {
            entity.ToTable("SuscripcionesPush");
            entity.HasKey(e => e.SuscripcionID);
            entity.Property(e => e.Activa).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.SuscripcionesPush)
                .HasForeignKey(e => e.UsuarioID)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
