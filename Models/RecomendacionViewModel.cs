using System.Collections.Generic;

namespace Pg1.Models
//Creacion de la clase de vista modelo recomendacion aca podemos observar productos comprados recomendados y el rating
{
    public class RecomendacionViewModel
    {
        public List<Producto> ProductosComprados { get; set; } = new();
        public List<Producto> ProductosRecomendados { get; set; } = new();
        public List<ProductRatingViewModel> Ratings { get; set; } = new();
    }
}
