namespace Pg1.Models
// Creacion del modelo donde veremos la puntuacion del producto de 1 a 5 estrellas
{
    public class ProductRatingViewModel
    {
        public int ProductId { get; set; }
        public float NormalizedScore { get; set; }
    }
}
