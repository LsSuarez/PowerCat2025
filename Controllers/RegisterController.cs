using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pg1.Data;
using Pg1.Models;

namespace Pg1.Controllers
        // Creando el registro controller para que el usuario pueda registrarse mediante un formulario sencillo
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterController> _logger;
        // Aplicamos la base de datos para registrar usuarios
        public RegisterController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<RegisterController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }
        // Metodo registro viewmodel para que tenga la validacion correcta al registrarse.
        [HttpGet]
        public IActionResult Registrar()
        {
            return View(new RegisterViewModel());
        }
        // Validamos si los usuarios estan registrados con el correo nuevo y si no es asi saldra que es existente o ya esta registrado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_context.Clientes.Any(c => c.Email == model.Email))
            {
                ModelState.AddModelError("", "El correo ya está registrado.");
                return View(model);
            }

            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                ModelState.AddModelError("", "El correo ya está registrado en el sistema de autenticación.");
                return View(model);
            }
            // Identidad para el usuario correo.
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", string.Join("; ", result.Errors.Select(e => e.Description)));
                return View(model);
            }

            // Crear cliente y guardamos sus datos en render postgresql
            var cliente = new Cliente
            {
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Email = model.Email,
                Telefono = model.Telefono,
                Direccion = model.Direccion,
                FechaRegistro = DateTime.UtcNow,
                IdentityUserId = user.Id
            };
            // Una vez creado el usuario nuevo saldra un mensaje que su registro ah sido exitoso
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            ViewBag.Mensaje = "¡Registro exitoso! Ya puedes iniciar sesión.";
            ModelState.Clear();

            return View(new RegisterViewModel());
        }
        // En caso de poner un correo falso no registrado le saldra error
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
