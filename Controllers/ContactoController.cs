using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pg1.Data;
using Pg1.ML;
using Pg1.Models;

namespace Pg1.Controllers
{
    // Este controlador maneja las acciones relacionadas con la página de contacto.
    // Aquí, los usuarios pueden enviar un mensaje o ver información de contacto.
    public class ContactoController : Controller
    {
        private readonly ILogger<ContactoController> _logger;
        private readonly ApplicationDbContext _context;

        public ContactoController(ILogger<ContactoController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(Contacto contacto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Datos de entrada no válidos";
                return View("Index", contacto);
            }

            try
            {
                // Tenemos que validar el mensaje que va predecir.
                if (string.IsNullOrWhiteSpace(contacto.Mensaje))
                {
                    ViewData["Message"] = "El mensaje no puede estar vacío";
                    return View("Index", contacto);
                }

                // Preparamos en input para el modelo sentimental
                var sampleData = new MLModelSentimentalAnalysis.ModelInput()
                {
                    Comentario = contacto.Mensaje
                };

                // Ejecutamos la prediccion del modelo
                var result = MLModelSentimentalAnalysis.Predict(sampleData);

                if (result == null)
                {
                    ViewData["Message"] = "No se pudo obtener una predicción válida";
                    return View("Index", contacto);
                }

                var predictedLabel = result.PredictedLabel;
                var scorePositive = result.Score.Length > 0 ? result.Score[0] : 0f;
                var scoreNegative = result.Score.Length > 1 ? result.Score[1] : 0f;

                // Asignamos etiqueta y la prediccion si sera positivo o negativo segun la data entrenada
                contacto.Etiqueta = predictedLabel == 1 ? "Positivo" : "Negativo";
                contacto.Puntuacion = predictedLabel == 1 ? scorePositive : scoreNegative;

                // Guardamos en contacto en la base de datos con la tabla t_contacto
                _context.DbSetContactos.Add(contacto);
                _context.SaveChanges();

                _logger.LogInformation("Se registró el contacto correctamente.");
                ViewData["Message"] = "Se registró el contacto correctamente";
            }
            catch (DllNotFoundException dllEx)
            {
                // Capturar errores especificos
                _logger.LogError(dllEx, "Error al cargar las librerías nativas necesarias para ML");
                ViewData["Message"] = "Error interno: problema con librerías del modelo ML. Contacta al administrador.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al registrar el contacto");
                ViewData["Message"] = "Error al registrar el contacto: " + ex.Message;
            }

            return View("Index", contacto);
        }
        //Si el mensaje no se llega enviar saldra un mensaje de error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
