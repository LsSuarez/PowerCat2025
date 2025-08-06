using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pg1.Models
        //Creando la clase cliente par poder ver quien es el usuario.
{
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCliente { get; set; }
        // El nombre del cliente no puede tener mas de 100 caracteres y es obligatorio.
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
        public string Nombre { get; set; }
        // El apellido del cliente es obligatorio y no puede tener mas de 100 caracteres
        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, ErrorMessage = "El apellido no puede tener más de 100 caracteres.")]
        public string Apellido { get; set; }
        // El correo es obligatorio de lo contrario no podra acceder.
        //Debe ser un correo valido sino saldra error.
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        public string Email { get; set; }
        //El numero de celular es obligatorio y no puede tener mas de 9 digitos ya que estamos en Peru.
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "El teléfono debe tener 9 dígitos.")]
        public string Telefono { get; set; }
        // La direcciones obligatoria y no puede tener mas de 200 caracteres
        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(200, ErrorMessage = "La dirección no puede tener más de 200 caracteres.")]
        public string Direccion { get; set; }

        public DateTime FechaRegistro { get; set; }

        [NotMapped] // La contraseña no se debe almacenar de manera obligatoria
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Relacion con identidades de persona cliente con pedidos.
        public string IdentityUserId { get; set; }

        // Relacion de pedidos del proyecto ggstore
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
