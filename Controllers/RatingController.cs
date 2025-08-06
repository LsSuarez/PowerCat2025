using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pg1.Data;
using Pg1.Models;
using System.Security.Claims;
using System.Linq;

namespace Pg1.Controllers
{
    public class RatingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RatingController> _logger;

        public RatingController(ILogger<RatingController> logger,
            ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Create([FromBody] RatingInputModel input)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Busca la calificación existente
            var existing = _context.DbSetRating
                .FirstOrDefault(r => r.Product != null && r.Product.IdProducto == input.ProductId && r.UserId == userId);

            if (existing != null)
            {
                existing.RatingValue = input.RatingValue;
            }
            else
            {
                var product = _context.Productos.FirstOrDefault(p => p.IdProducto == input.ProductId);

                if (product == null)
                    return NotFound("Producto no encontrado");

                var rating = new Rating
                {
                    Product = product,
                    UserId = userId,
                    RatingValue = input.RatingValue,
                    FechaRating = DateTime.UtcNow
                };

                _context.DbSetRating.Add(rating);
            }

            _context.SaveChanges();
            return Ok();
        }
        // Clase creada para el inputmodel
        public class RatingInputModel
        {
            public int ProductId { get; set; }
            public int RatingValue { get; set; }
        }
    }
}
