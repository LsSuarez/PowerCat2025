using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pg1.Models

//Creacion del model producto para poder ver los productos que tenemos en nuestro proyecto ggstore.
{
    public class Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdProducto { get; set; }
        //El nombre del producto es obligatorio y no puede tener mas de 100 caracteres.

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = null!;
        //La descripcion del producto no puede superar los 500 caracteres.

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }
        // El precio del producto es obligatorio y debe ser mayor a cero.
        [Column(TypeName = "decimal(18,2)")]
        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero.")]
        public decimal Precio { get; set; }
        //El stock del producto es obligatorio ya que segun esto vemos las cantidades que tenemos disponibles, el stock no puede ser negativo.

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }
        // Podemos ingresar url para agregar productos o tambien escoger desde el local.

        [Url(ErrorMessage = "Debe ingresar una URL válida para la imagen.")]
        public string? ImagenUrl { get; set; }
        //Categoria del producto es obligatoria porque con eso filtramos la informacion.

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida.")]
        public int IdCategoria { get; set; }

        // Propiedad de navegación hacia la categoría
        public Categoria? Categoria { get; set; }

        // Nueva propiedad para indicar si el producto es recomendado
        public bool IsRecommended { get; set; } = false;
    }
}
