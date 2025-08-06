using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations; //Necesario para el uso de [Key]
using System.ComponentModel.DataAnnotations.Schema; //Necesario para el uso de [DatabaseGenerated]

namespace Pg1.Models
// Creacion de la tabla detalle pedido par apoder organizar los id gets y sets.
{
    public class DetallePedido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

//Metodo public int par apoder identificar el id del pedido.
            public int IdDetalle { get; set; }

            public int IdPedido { get; set; }
            // El pedido debe ser obligatorio
            public Pedido Pedido { get; set; }

            public int IdProducto { get; set; }
            //El producto debe ser obligatorio con el metodo get y set.
            public Producto Producto { get; set; }

            public int Cantidad { get; set; }
            public decimal PrecioUnitario { get; set; }
            public decimal Subtotal { get; set; }
    }
}