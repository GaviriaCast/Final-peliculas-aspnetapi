using Microsoft.AspNetCore.Mvc;

namespace PeliculasApi.Errors;

/// <summary>
/// Reemplaza la respuesta 400 por defecto de [ApiController] (ValidationProblemDetails,
/// con un diccionario "errors") por la de NestJS, que es la que leen Vue e Ionic:
///   { "message": ["El nombre es obligatorio", ...], "error": "Bad Request", "statusCode": 400 }
/// </summary>
public static class ValidationErrorFactory
{
    public static IActionResult Crear(ActionContext context)
    {
        var mensajes = new List<string>();

        foreach (var (campo, estado) in context.ModelState)
        {
            foreach (var error in estado.Errors)
            {
                mensajes.Add(campo.StartsWith('$')
                    ? MensajeJsonInvalido(campo)
                    : error.ErrorMessage);
            }
        }

        var respuesta = ErrorResponse.Create(StatusCodes.Status400BadRequest, mensajes.Distinct().ToList());
        return new BadRequestObjectResult(respuesta);
    }

    /// <summary>
    /// Errores del lector de JSON: llegan con la ruta del campo ("$.anio") cuando el tipo
    /// no coincide, por ejemplo "anio": "" o "anio": "abc". "$" solo es el cuerpo completo.
    /// </summary>
    private static string MensajeJsonInvalido(string ruta)
    {
        var campo = ruta.TrimStart('$', '.');
        return campo.Length == 0
            ? "El cuerpo de la peticion no es un JSON valido"
            : $"El campo {campo} tiene un valor invalido";
    }
}
