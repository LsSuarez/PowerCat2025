using System.Collections.Generic;

namespace Pg1.Models
//Creacion de la lista productos
    // Creacion de la lista categorias
{
    public class AdminPanelViewModel
    {
        public List<Producto> Productos { get; set; } = new List<Producto>();

        public ProductoCreateViewModel NuevoProducto { get; set; } = new ProductoCreateViewModel();

        public Producto? ProductoEdit { get; set; }

        public List<Categoria> Categorias { get; set; } = new List<Categoria>();
    }
}
