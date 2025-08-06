namespace Pg1.Models;


public class ErrorViewModel

// Creacion del modelo de errores para poder saber si tenemos un error en el requestId.
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
