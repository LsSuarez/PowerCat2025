using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pg1.Data;
using Pg1.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Pg1.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Filtro por categoria de Productos.
        public async Task<IActionResult> Index(string categoria, string busqueda)
        {
            var productosQuery = _context.Productos
                .Include(p => p.Categoria)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                productosQuery = productosQuery.Where(p => p.Categoria != null && p.Categoria.Nombre == categoria);
            }

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                productosQuery = productosQuery.Where(p =>
                    p.Nombre.Contains(busqueda) ||
                    (!string.IsNullOrEmpty(p.Descripcion) && p.Descripcion.Contains(busqueda)));
            }

            var productos = await productosQuery
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            ViewBag.Categorias = await _context.Categorias
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            return View(productos);
        }

        // Si el producto es individual
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null)
                return NotFound();

            return View(producto);
        }
    }
}
