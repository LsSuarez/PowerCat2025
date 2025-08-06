using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pg1.Models
//Creacion de la tabla rating donde podemos almacenar los datos de rating
{
    [Table("t_rating")]
    public class Rating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? UserId { get; set; }

        // Clave foránea explícita para la tabla.
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        // Direccional a producto.
        public Producto? Product { get; set; }

        public int RatingValue { get; set; }

        public DateTime FechaRating { get; set; } = DateTime.Now;
    }
}
