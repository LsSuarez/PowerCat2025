using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pg1.Data;
using Pg1.Models;
using Pg1.Extensions;

namespace Pg1.Controllers
{
    public class CarritoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string SessionKeyName = "_Carrito";
        private readonly ILogger<CarritoController> _logger;

        public CarritoController(ApplicationDbContext context, ILogger<CarritoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var carrito = GetCarrito();
            return View(carrito);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarAlCarrito(int idProducto, int cantidad = 1)
        {
            var producto = await _context.Productos.FindAsync(idProducto);

            if (producto == null || producto.Stock < cantidad)
            {
                _logger.LogWarning("Producto no encontrado o sin stock suficiente.");
                return NotFound();
            }

            var carrito = GetCarrito();
            var item = carrito.Items.FirstOrDefault(i => i.Producto.IdProducto == idProducto);

            if (item != null)
            {
                item.Cantidad += cantidad;
            }
            else
            {
                carrito.Items.Add(new CarritoItem
                {
                    Producto = producto,
                    Cantidad = cantidad,
                    PrecioUnitario = producto.Precio
                });
            }

            SaveCarrito(carrito);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult ActualizarCantidad(int idProducto, int cantidad)
        {
            var producto = _context.Productos.Find(idProducto);
            if (producto == null)
            {
                _logger.LogWarning($"Producto con ID {idProducto} no encontrado.");
                return NotFound();
            }

            var carrito = GetCarrito();
            var item = carrito.Items.FirstOrDefault(i => i.Producto.IdProducto == idProducto);

            if (item != null)
            {
                if (cantidad <= 0)
                {
                    carrito.Items.Remove(item);
                }
                else if (cantidad <= producto.Stock) // Verificar stock disponible
                {
                    item.Cantidad = cantidad;
                }
                else
                {
                    TempData["ErrorStock"] = $"No hay suficiente stock. Disponible: {producto.Stock}";
                }
            }

            SaveCarrito(carrito);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoverDelCarrito(int idProducto)
        {
            var carrito = GetCarrito();
            var item = carrito.Items.FirstOrDefault(i => i.Producto.IdProducto == idProducto);

            if (item != null)
            {
                carrito.Items.Remove(item);
                SaveCarrito(carrito);
                TempData["SuccessMessage"] = "Producto eliminado del carrito";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Importante para seguridad para poder comprobar que esta en el carrito los productos seleccionados
        public IActionResult VaciarCarrito()
        {
            // Elimina el carrito de la sesión
            HttpContext.Session.Remove(SessionKeyName);
            
            // Opcional: Mensaje de confirmación y podra ver todos los productos correctamente 
            TempData["Mensaje"] = "El carrito se ha vaciado correctamente";
            
            // Redirige de vuelta al índice del carrito
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Checkout()
        {
            var carrito = GetCarrito();
            if (!carrito.Items.Any())
            {
                _logger.LogWarning("El carrito está vacío.");
                return RedirectToAction("Index");
            }

            return View(new PedidoViewModel { Carrito = carrito });
        }

        [HttpPost, ActionName("Checkout")]
        public async Task<IActionResult> CheckoutPost()
        {
            var carrito = GetCarrito();
            if (!carrito.Items.Any())
            {
                _logger.LogWarning("El carrito está vacío al intentar hacer checkout.");
                return RedirectToAction("Index");
            }
            //Creamos un pedido en el carrito
            // Crear el pedido usando el carrito, sin depender de un modelo enviado por el formulario.
            var pedido = new Pedido
            {
                FechaPedido = DateTime.UtcNow,
                Estado = "Pendiente",
                Total = carrito.Items.Sum(i => i.Subtotal),
                IdCliente = 1, // Temporal
                Detalles = carrito.Items.Select(i => new DetallePedido
                {
                    IdProducto = i.Producto.IdProducto,
                    Cantidad = i.Cantidad,
                    PrecioUnitario = i.PrecioUnitario,
                    Subtotal = i.Subtotal
                }).ToList()
            };

            _context.Pedidos.Add(pedido);

            foreach (var item in carrito.Items)
            {
                var producto = await _context.Productos.FindAsync(item.Producto.IdProducto);
                if (producto != null)
                    producto.Stock -= item.Cantidad;
            }

            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32("UltimoPedidoId", pedido.IdPedido);

            _logger.LogInformation("Pedido creado con éxito. ID: {IdPedido}", pedido.IdPedido);

            return RedirectToAction("Pago");
        }
        //Una vez seleccionado los productos el carrito vacio va intentar acceder ala pagina de pago.
        public IActionResult Pago()
        {
            var carrito = GetCarrito();
            if (!carrito.Items.Any())
            {
                _logger.LogWarning("El carrito está vacío al intentar acceder a la página de pago.");
                return RedirectToAction("Index");
            }

            return View(new PedidoViewModel { Carrito = carrito });
        }
        //Saldra un error de pago si no hay un pedido o producto seleccionado
        public IActionResult ConfirmacionPaypal(string orderId)
        {
            var idPedido = HttpContext.Session.GetInt32("UltimoPedidoId");
            if (idPedido == null)
            {
                ViewBag.Error = "No se encontró el pedido para registrar el pago.";
                return View();
            }
            // El pedido no registra en la base de datos por eso tiene condicion if
            
            var pedido = _context.Pedidos.FirstOrDefault(p => p.IdPedido == idPedido.Value);
            if (pedido == null)
            {
                ViewBag.Error = "No se encontró el pedido en la base de datos.";
                return View();
            }
            // Vemos que el paypal ya este en estado completado y listo.
            var pago = new Pago
            {
                IdPedido = pedido.IdPedido,
                FechaPago = DateTime.UtcNow,
                Monto = pedido.Total,
                MetodoPago = "PayPal",
                EstadoPago = "Completado"
            };
            _context.Pagos.Add(pago);

            pedido.Estado = "Completado";
            _context.SaveChanges();

            HttpContext.Session.Remove("_Carrito");
            HttpContext.Session.Remove("UltimoPedidoId");

            ViewBag.OrderId = orderId;
            return View();
        }

        private Carrito GetCarrito()
        {
            var carrito = HttpContext.Session.Get<Carrito>(SessionKeyName);
            if (carrito == null)
            {
                carrito = new Carrito();
                SaveCarrito(carrito);
            }
            return carrito;
        }
        //Una vez realizado el pago el carrito queda sin producto.
        private void SaveCarrito(Carrito carrito)
        {
            HttpContext.Session.Set(SessionKeyName, carrito);
        }
    }
}