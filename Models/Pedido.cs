using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations; //Necesario para el uso de [Key]
using System.ComponentModel.DataAnnotations.Schema; //Necesario para el uso de [DatabaseGenerated]

namespace Pg1.Models
// La tabla pedido es obligatorio ya que aqui es donde tenemos contacto con cliente detalles del pedido y los pagos.
{
    public class Pedido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }
        //El cliente debe ser obligatorio para la creacion de un pedido
        public Cliente Cliente { get; set; }
        //Debe ser oblitaroio la fecha del dia de pedido aca podemos ver el estado del pedido y el total del pedido.
        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; }
        public decimal Total { get; set; }

        public ICollection<DetallePedido> Detalles { get; set; }
        public ICollection<Pago> Pagos { get; set; }
            
        public enum EstadoPedido
        {
        Carrito, Pendiente, Completado, Cancelado
        }
    }
}