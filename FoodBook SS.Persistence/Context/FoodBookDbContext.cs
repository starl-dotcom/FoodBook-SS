using FoodBook_SS.Domain.Entities.Configuration;
using FoodBook_SS.Domain.Entities.Order;
using FoodBook_SS.Domain.Entities.Payment;
using FoodBook_SS.Domain.Entities.Reservation;
using FoodBook_SS.Domain.Entities.Review;
using FoodBook_SS.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
namespace FoodBook_SS.Persistence.Context
{
    public class FoodBookDbContext : DbContext
    {
        public FoodBookDbContext(DbContextOptions<FoodBookDbContext> options) : base(options) { }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; } 
        public DbSet<Restaurante> Restaurantes { get; set; }
        public DbSet<Mesa> Mesas { get; set; }
        public DbSet<HorarioRestaurante> Horarios { get; set; }
        public DbSet<CategoriaMenu> CategoriasMenu { get; set; }
        public DbSet<ProductoMenu> ProductosMenu { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Orden> Ordenes { get; set; }
        public DbSet<ItemOrden> ItemsOrden { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Resena> Resenas { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>().HasQueryFilter(e => !e.Eliminado);
            modelBuilder.Entity<Restaurante>().HasQueryFilter(e => !e.Eliminado);
            modelBuilder.Entity<Mesa>().HasQueryFilter(e => !e.Eliminado);
            modelBuilder.Entity<ProductoMenu>().HasQueryFilter(e => !e.Eliminado);
            modelBuilder.Entity<Reserva>().HasQueryFilter(e => !e.Eliminado);
            modelBuilder.Entity<Orden>().HasQueryFilter(e => !e.Eliminado);
            modelBuilder.Entity<Resena>().HasQueryFilter(e => !e.Eliminado);
            modelBuilder.Entity<Rol>().ToTable("Roles");
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Restaurante>().ToTable("Restaurantes");
            modelBuilder.Entity<Mesa>().ToTable("Mesas");
            modelBuilder.Entity<HorarioRestaurante>().ToTable("HorariosRestaurante");
            modelBuilder.Entity<CategoriaMenu>().ToTable("CategoriasMenu");
            modelBuilder.Entity<ProductoMenu>().ToTable("ProductosMenu");
            modelBuilder.Entity<Reserva>().ToTable("Reservas");
            modelBuilder.Entity<Orden>().ToTable("Ordenes");
            modelBuilder.Entity<ItemOrden>().ToTable("ItemsOrden");
            modelBuilder.Entity<Pago>().ToTable("Pagos");
            modelBuilder.Entity<Resena>().ToTable("Resenas");
            modelBuilder.Entity<Rol>().HasKey(e => e.Id);
            modelBuilder.Entity<Rol>().Property(e => e.Id).HasColumnName("RolId");
            modelBuilder.Entity<Usuario>().HasKey(e => e.Id);
            modelBuilder.Entity<Usuario>().Property(e => e.Id).HasColumnName("UsuarioId");
            modelBuilder.Entity<Usuario>().HasIndex(e => e.Email).IsUnique();
            modelBuilder.Entity<Restaurante>().HasKey(e => e.Id);
            modelBuilder.Entity<Restaurante>().Property(e => e.Id).HasColumnName("RestauranteId");
            modelBuilder.Entity<Mesa>().HasKey(e => e.Id);
            modelBuilder.Entity<Mesa>().Property(e => e.Id).HasColumnName("MesaId");
            modelBuilder.Entity<HorarioRestaurante>().HasKey(e => e.HorarioId);
            modelBuilder.Entity<HorarioRestaurante>()
                .HasIndex(e => new { e.RestauranteId, e.DiaSemana }).IsUnique();
            modelBuilder.Entity<CategoriaMenu>().HasKey(e => e.Id);
            modelBuilder.Entity<CategoriaMenu>().Property(e => e.Id).HasColumnName("CategoriaId");
            modelBuilder.Entity<ProductoMenu>().HasKey(e => e.Id);
            modelBuilder.Entity<ProductoMenu>().Property(e => e.Id).HasColumnName("ProductoId");
            modelBuilder.Entity<ProductoMenu>().Property(e => e.Precio).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Reserva>().HasKey(e => e.Id);
            modelBuilder.Entity<Reserva>().Property(e => e.Id).HasColumnName("ReservaId");
            modelBuilder.Entity<Reserva>().Property(e => e.Estado).HasMaxLength(20).HasDefaultValue("Pendiente");
            modelBuilder.Entity<Reserva>().HasIndex(e => e.CodigoConfirmacion).IsUnique();
            modelBuilder.Entity<Orden>().HasKey(e => e.Id);
            modelBuilder.Entity<Orden>().Property(e => e.Id).HasColumnName("OrdenId");
            modelBuilder.Entity<Orden>().Property(e => e.Subtotal).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Orden>().Property(e => e.Impuesto).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Orden>().Property(e => e.Total).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<ItemOrden>().HasKey(e => e.Id);
            modelBuilder.Entity<ItemOrden>().Property(e => e.Id).HasColumnName("ItemId");
            modelBuilder.Entity<ItemOrden>().Property(e => e.PrecioUnitario).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<ItemOrden>().Ignore(e => e.Subtotal);
            modelBuilder.Entity<Pago>().HasKey(e => e.Id);
            modelBuilder.Entity<Pago>().Property(e => e.Id).HasColumnName("PagoId");
            modelBuilder.Entity<Pago>().Property(e => e.Monto).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Resena>().HasKey(e => e.Id);
            modelBuilder.Entity<Resena>().Property(e => e.Id).HasColumnName("ResenaId");
            modelBuilder.Entity<Resena>().Ignore(e => e.Respuesta);
            modelBuilder.Entity<Resena>().Ignore(e => e.FechaRespuesta);
            modelBuilder.Entity<Resena>().Ignore(e => e.ModeradaPor);
            modelBuilder.Entity<Resena>().Ignore(e => e.FechaModeracion);
        }
    }
}

