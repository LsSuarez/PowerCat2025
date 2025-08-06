using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pg1.Data;
using Pg1.Models;
using System.Security.Claims;

namespace Pg1.Controllers
{
    public class RecomendacionController : Controller
    {
        private readonly ILogger<RecomendacionController> _logger;
        private readonly ApplicationDbContext _context;

        public RecomendacionController(ILogger<RecomendacionController> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userRatings = _context.DbSetRating
                .Where(r => r.UserId == userId && r.Product != null)
                .Select(r => new { r.ProductId, r.RatingValue })
                .ToList();

            var productosComprados = userRatings
                .Select(r => _context.Productos.FirstOrDefault(p => p.IdProducto == r.ProductId))
                .Where(p => p != null)
                .ToList();

            var productosRecomendados = GetProductRecomendados();

            if (productosRecomendados.Count == 0)
                productosRecomendados = _context.Productos.ToList();

            var avgRatings = _context.DbSetRating
                .GroupBy(r => r.ProductId)
                .Select(g => new ProductRatingViewModel
                {
                    ProductId = g.Key,
                    NormalizedScore = (float)g.Average(r => r.RatingValue) * 20
                })
                .ToList();

            var viewModel = new RecomendacionViewModel()
            {
                ProductosComprados = productosComprados,
                ProductosRecomendados = productosRecomendados,
                Ratings = avgRatings
            };

            return View(viewModel);
        }

        private List<Producto> GetProductRecomendados()
        {
            return _context.Productos
                .Where(p => p.IsRecommended)
                .ToList();
        }
        //Vaildaremos los rating segun el datetime
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddRating(int productId, int ratingValue)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            if (ratingValue < 1 || ratingValue > 5)
                return BadRequest("Rating inválido");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingRating = _context.DbSetRating
                .FirstOrDefault(r => r.ProductId == productId && r.UserId == userId);

            if (existingRating != null)
            {
                existingRating.RatingValue = ratingValue;
                existingRating.FechaRating = DateTime.UtcNow;
            }
            else
            {
                var rating = new Rating
                {
                    ProductId = productId,
                    UserId = userId,
                    RatingValue = ratingValue,
                    FechaRating = DateTime.UtcNow
                };
                _context.DbSetRating.Add(rating);
            }
        // Si todo sale bien saldra un mensaje agradeciendo la recomendacion dada en cuanto a estrellas
            _context.SaveChanges();

            TempData["Mensaje"] = "¡Gracias por tu recomendación! ;)";

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
