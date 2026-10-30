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

        // Un id no numerico en la ruta (/movies/abc) en NestJS lo rechaza ParseIntPipe,
        // que responde message como texto y no como arreglo
        var soloRuta = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .All(e => context.RouteData.Values.ContainsKey(e.Key));

        object message = soloRuta && mensajes.Count > 0 ? mensajes[0] : mensajes.Distinct().ToList();

        return new BadRequestObjectResult(ErrorResponse.Create(StatusCodes.Status400BadRequest, message));
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
