using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pg1.Data;
using Pg1.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Pg1.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // Redirige /Admin a /Admin/Panel
        public IActionResult Index() => RedirectToAction(nameof(Panel));

        // GET: Panel con listado de productos y formulario para crear/editar
        public async Task<IActionResult> Panel(int? editId = null)
        {
            var productos = await _context.Productos.Include(p => p.Categoria).ToListAsync();
            var categorias = await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync();

            Producto productoEdit = null;
            if (editId.HasValue)
            {
                productoEdit = await _context.Productos.FindAsync(editId.Value);
                if (productoEdit == null)
                    return NotFound();
            }

            var model = new AdminPanelViewModel
            {
                Productos = productos,
                Categorias = categorias,
                ProductoEdit = productoEdit,
                NuevoProducto = new ProductoCreateViewModel()
            };

            return View(model);
        }

        // POST: Crear nuevo producto con subida de imagen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminPanelViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Productos = await _context.Productos.Include(p => p.Categoria).ToListAsync();
                model.Categorias = await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync();
                return View("Panel", model);
            }

            string? imagenUrl = null;

            if (model.NuevoProducto.ImagenFile != null && model.NuevoProducto.ImagenFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "img");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string extension = Path.GetExtension(model.NuevoProducto.ImagenFile.FileName);
                string uniqueFileName = Guid.NewGuid().ToString() + extension;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.NuevoProducto.ImagenFile.CopyToAsync(stream);
                }

                imagenUrl = uniqueFileName;
            }
            else if (!string.IsNullOrWhiteSpace(model.NuevoProducto.ImagenUrl))
            {
                imagenUrl = model.NuevoProducto.ImagenUrl.Trim();
            }

            var producto = new Producto
            {
                Nombre = model.NuevoProducto.Nombre.Trim(),
                Precio = model.NuevoProducto.Precio,
                Stock = model.NuevoProducto.Stock,
                Descripcion = model.NuevoProducto.Descripcion?.Trim(),
                ImagenUrl = imagenUrl,
                IdCategoria = model.NuevoProducto.IdCategoria
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Producto creado exitosamente.";
            return RedirectToAction(nameof(Panel));
        }

        // GET: Redirige a Panel para editar producto
        public IActionResult Editar(int id) => RedirectToAction(nameof(Panel), new { editId = id });

        // POST: Actualizamos el producto como administrador tambien podemos editarlo y borrarlo.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Producto producto)
        {
            if (id != producto.IdProducto)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                var modelError = new AdminPanelViewModel
                {
                    Productos = await _context.Productos.Include(p => p.Categoria).ToListAsync(),
                    Categorias = await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync(),
                    ProductoEdit = producto
                };
                return View("Panel", modelError);
            }

            try
            {
                producto.Nombre = producto.Nombre.Trim();
                producto.Descripcion = producto.Descripcion?.Trim();
                producto.ImagenUrl = producto.ImagenUrl?.Trim();

                _context.Productos.Update(producto);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Producto actualizado exitosamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Productos.AnyAsync(p => p.IdProducto == id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Panel));
        }

        // POST: Creamos un post para poder eliminar el producto de forma segura
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Producto eliminado exitosamente.";
            return RedirectToAction(nameof(Panel));
        }

        // Aca cerramos sesion de manera exitosa y es rederigido al home.
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View("LogoutConfirmation");
        }
    }
}
