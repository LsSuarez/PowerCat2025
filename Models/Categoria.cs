using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pg1.Models
// Creando el id categoria
{
    public class Categoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCategoria { get; set; }
        // El nombre es obligatorio y no puede pasarse de 100 caracteres
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
        public string Nombre { get; set; } = null!;
        // La descripcion no puede pasar de 250 caracteres
        [MaxLength(250, ErrorMessage = "La descripción no puede exceder 250 caracteres.")]
        public string? Descripcion { get; set; }

        // Propiedad de navegación: Productos asociados ala categoria creada.
        // Habilitamos la carga diferida si esta activada.
        public virtual ICollection<Producto> Productos { get; set; } = new HashSet<Producto>();
    }
}
