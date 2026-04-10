using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Configuration;
using FoodBook_SS.Domain.Entities.User;

namespace FoodBook_SS.Domain.Entities.Reservation
{
    public static class EstadoReserva
    {
        public const string Pendiente = "Pendiente";
        public const string Confirmada = "Confirmada";
        public const string Cancelada = "Cancelada";
        public const string Completada = "Completada";
        public const string NoShow = "NoShow";
    }

    public static class EstadoOrden
    {
        public const string Pendiente = "Pendiente";
        public const string Confirmada = "Confirmada";
        public const string EnPreparacion = "EnPreparacion";
        public const string Lista = "Lista";
        public const string Entregada = "Entregada";
        public const string Completada = "Completada";
        public const string Cancelada = "Cancelada";
    }

    public static class EstadoPago
    {
        public const string Pendiente = "Pendiente";
        public const string Aprobado = "Aprobado";
        public const string Rechazado = "Rechazado";
        public const string Reembolsado = "Reembolsado";
    }

    public class Reserva : BaseEntity
    {
        public int ClienteId { get; set; }
        public int RestauranteId { get; set; }
        public int? MesaId { get; set; }
        public DateOnly FechaReserva { get; set; }
        public TimeOnly HoraReserva { get; set; }
        public int NumeroPersonas { get; set; }
        public string Estado { get; set; } = EstadoReserva.Pendiente;
        public string? NotasEspeciales { get; set; }
        public string? CodigoConfirmacion { get; set; }

        public Mesa? Mesa { get; set; }
        public Restaurante? Restaurante { get; set; }
        public Usuario? Cliente { get; set; }
    }
}
