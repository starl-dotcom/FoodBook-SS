
using Microsoft.EntityFrameworkCore;
using FoodBook_SS.Domain.Entities.Configuration;
using FoodBook_SS.Domain.Entities.Order;
using FoodBook_SS.Domain.Entities.Payment;
using FoodBook_SS.Domain.Entities.Reservation;
using FoodBook_SS.Domain.Entities.Review;
using FoodBook_SS.Domain.Entities.User;

namespace FoodBook_SS.Persistence.Context
{
    public class FoodBookDbContext : DbContext

    {
        public FoodBookDbContext(DbContextOptions<FoodBookDbContext> options) : base(options)
        {

        }

        // Configuration
        public DbSet<Restaurante> Restaurantes { get; set; }
        public DbSet<Mesa> Mesas { get; set; }

        // Reservation
        public DbSet<Reserva> Reservas { get; set; }

        // Order
        public DbSet<Orden> Ordenes { get; set; }
        public DbSet<ItemOrden> ItemsOrden { get; set; }

        // Payment
        public DbSet<Pago> Pagos { get; set; }

        // Review
        public DbSet<Reseña> Resenas { get; set; }

        // User
        public DbSet<Usuario> Usuarios { get; set; }

    }
}