using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Pg1.Models
// Explicando la tabla contacto en la cual se van a registrar los comentarios para poder predecirlos en la base de datos
{
    // Creacion de la tabla contacto
    [Table("t_contacto")]
    public class Contacto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
// Los nombres para poder comentar son obligatorios
        public int Id { get; set; }
        [Required(ErrorMessage = "Nombres son obligatorios.")]
        public string? Nombres { get; set; }
        [NotNull]
// El correo tambien es obligatorio para dejar un comentario
        public string? Email { get; set; }
        [NotNull]
// El mensaje tambien es obligatorio
        public string? Mensaje { get; set; }

// La etiqueta del producto debe ser obligatorio
        public string? Etiqueta { get; set; }
// La puntuacion de 1 a 5 estrellas debe ser obligatoria
        public float Puntuacion { get; set; }



    }
}
