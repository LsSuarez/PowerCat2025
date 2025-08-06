using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Pg1.Models
// Creacion del producto view model, par apoder trabajar con la vista.
{
    public class ProductoCreateViewModel
    {
// El nombre es obligatorio y no puede pasar los 100 caracteres.
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;
//El precio es obligatorio y debe ser mayor que cero y no puede ser negativo 
        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero.")]
        public decimal Precio { get; set; }
//Stock obligatorio y debe ser mayor que cero.
        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser cero o más.")]
        public int Stock { get; set; }
    //La descripcion del producto no puede tener mas de 500 caracteres 

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }

        // Para subir imagen desde formulario
        public IFormFile? ImagenFile { get; set; }

        // URL alternativa para imagen
        [Url(ErrorMessage = "Debe ingresar una URL válida.")]
        public string? ImagenUrl { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida.")]
        public int IdCategoria { get; set; }
    }
}
