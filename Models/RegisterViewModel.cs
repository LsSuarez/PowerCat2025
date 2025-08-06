using System.ComponentModel.DataAnnotations;

namespace Pg1.Models
//Creacion de la vista del modelo registro
{
    public class RegisterViewModel
    //El nombre para registro es obligatorio
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; }
    //El apellido es obligatorio
        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100)]
        public string Apellido { get; set; }
    //El correo debe ser valido y obligatorio de lo contrario saldra un error al registrarse.
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
        public string Email { get; set; }
    //El telefono debe ser obligatorio y tener 9 digitos como minimo ya que es de Peru
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "El teléfono debe tener 9 dígitos.")]
        public string Telefono { get; set; }
    //La direccion es obligatoria un campo muy valioso.
        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(200)]
        public string Direccion { get; set; }
        //La contraseña es obligatoria debe tener minimo 6 caracteres
        //Si no coinciden las contraseñas al registrarse saldra un error
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Password { get; set; }
    //Contraseñas deben coincidir sino error
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }
    }
}
