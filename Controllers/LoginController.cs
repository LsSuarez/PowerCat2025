using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pg1.Data;        // Asegúrate que esté el namespace de tu DbContext
using Pg1.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pg1.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public LoginController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Ingresar()
        {
            return View();
        }
        // Debemos agregar el metodo post para poder crear el login, para que el usuario pueda ingresar ala web con su cuenta creada.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ingresar(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
            // Completar todos los campos del formulario para poder acceder ala web.
                ViewBag.Mensaje = "Por favor, complete todos los campos.";
                return View();
            }
            
            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
            // Si el usuario o contraseña esta mal ingresada saldra un aviso que los datos ingresados son erroneos
                ViewBag.Mensaje = "Usuario o contraseña incorrectos.";
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
            // Si el usuario o contraseña esta mal ingresada saldra un aviso que los datos ingresados son erroneos
                ViewBag.Mensaje = "Usuario o contraseña incorrectos.";
                return View();
            }
            // Asignacion de roles como Usuario
            var roles = await _userManager.GetRolesAsync(user);

            HttpContext.Session.SetString("Usuario", email);
            HttpContext.Session.SetString("Rol", roles.Count > 0 ? roles[0] : "Usuario");

            if (roles.Contains("Administrador"))
            {
                return RedirectToAction("Panel", "Admin");
            }

            return RedirectToAction("Index", "Home");
        }

        // Aca podemos mostrar el formulario de registro
        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        // Porceso de registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(Cliente model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Verificacion si el correo ya existe saldra como correo electronico registrado.
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                ViewBag.Mensaje = "El correo electrónico ya está registrado.";
                return View(model);
            }

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ViewBag.Mensaje = string.Join("; ", result.Errors.Select(e => e.Description));
                return View(model);
            }

            // Asignamos el rol de usuario ya que tenemos otro rol administrador
            await _userManager.AddToRoleAsync(user, "Usuario");

            // Guardar datos de cliente en la tabla Clientes
            model.FechaRegistro = DateTime.Now;

            // Si quieres guardar el Id del usuario Identity en Cliente, añade un campo y asígnalo
            // model.IdentityUserId = user.Id;  (crea ese campo en Cliente si quieres)

            _context.Clientes.Add(model);
            await _context.SaveChangesAsync();

            // Iniciar sesión automáticamente
            await _signInManager.SignInAsync(user, isPersistent: false);

            HttpContext.Session.SetString("Usuario", user.Email);
            HttpContext.Session.SetString("Rol", "Usuario");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarSesion()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return View("LogoutConfirmation");
        }
    }
}
