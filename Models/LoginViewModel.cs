using System.ComponentModel.DataAnnotations;

namespace Pg1.Models
// Creacion del modelo login viewmodel para poder utilizar el correo y contraseña al ingresar.
{
    public class LoginViewModel
    {
        // Es obligatorio ingresar un email o correo para poder ingresar.
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        // Es obligatorio usar la contraseña para ingresar.
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
