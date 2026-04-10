using FoodBook_SS.Application.Base;
using System.ComponentModel.DataAnnotations;
namespace FoodBook_SS.Application.Dtos.Reservation
{
    public class ReservationDto : DtoBase
    {
        public string NombreCliente { get; set; } = string.Empty;
        public string NombreRestaurante { get; set; } = string.Empty;
        public DateOnly FechaReserva { get; set; }
        public TimeOnly HoraReserva { get; set; }
        public int NumeroPersonas { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? CodigoConfirmacion { get; set; }
        public string? NumeroMesa { get; set; }
        public int RestauranteId { get; set; }
    }
    public class SaveReservationDto
    {
        public int RestauranteId { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateOnly FechaReserva { get; set; }

        [Required(ErrorMessage = "La hora es obligatoria.")]
        public TimeOnly HoraReserva { get; set; }

        [Required(ErrorMessage = "Indica el número de personas.")]
        [Range(1, 20, ErrorMessage = "El número de personas debe estar entre 1 y 20.")]
        public int NumeroPersonas { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una mesa disponible.")]
        public int MesaId { get; set; }

        public string? NotasEspeciales { get; set; }
    }
    public class UpdateReservationDto
    {
        public DateOnly? FechaReserva { get; set; }
        public TimeOnly? HoraReserva { get; set; }
        public int? NumeroPersonas { get; set; }
        public string? NotasEspeciales { get; set; }
    }
}
