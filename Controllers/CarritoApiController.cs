using Microsoft.AspNetCore.Mvc;
using Pg1.Data;
using Pg1.Models;
using System.Linq;

namespace Pg1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarritoApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarritoApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Confirma el pago de PAYPAL para poder proceder con el pago exitoso.
        /// <param name="orderId">ID de orden PayPal</param>
        /// <returns>Resultado de confirmación</returns>
        [HttpGet("ConfirmacionPaypal")]
        public IActionResult ConfirmacionPaypal([FromQuery] string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
                return BadRequest(new { message = "El parámetro orderId es obligatorio." });

            var pedido = _context.Pedidos
                .OrderByDescending(p => p.IdPedido)
                .FirstOrDefault();

            if (pedido == null)
                return NotFound(new { message = "No se encontró pedido para confirmar pago." });

            var pago = new Pago
            {
                IdPedido = pedido.IdPedido,
                FechaPago = System.DateTime.UtcNow,
                Monto = pedido.Total,
                MetodoPago = "PayPal",
                EstadoPago = "Completado"
            };

            _context.Pagos.Add(pago);
            pedido.Estado = "Completado";
            _context.SaveChanges();

            return Ok(new
            {
                message = "Pago confirmado exitosamente",
                orderId,
                pedidoId = pedido.IdPedido
            });
        }
    }
}
