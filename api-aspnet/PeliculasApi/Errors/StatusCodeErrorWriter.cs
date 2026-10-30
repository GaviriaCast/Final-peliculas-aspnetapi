using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;

namespace PeliculasApi.Errors;

/// <summary>
/// Da cuerpo JSON a las respuestas de error que ASP.NET devuelve vacias,
/// por ejemplo una ruta que no existe. NestJS responde:
///   { "message": "Cannot GET /api/peliculas", "error": "Not Found", "statusCode": 404 }
/// </summary>
public static class StatusCodeErrorWriter
{
    public static async Task Escribir(StatusCodeContext context)
    {
        var http = context.HttpContext;
        var statusCode = http.Response.StatusCode;

        // Igual que NestJS: una ruta o un verbo que no existen son 404 con "Cannot VERBO /ruta"
        if (statusCode is StatusCodes.Status404NotFound or StatusCodes.Status405MethodNotAllowed)
        {
            http.Response.StatusCode = StatusCodes.Status404NotFound;
            var mensaje = $"Cannot {http.Request.Method} {http.Request.Path}";
            await http.Response.WriteAsJsonAsync(ErrorResponse.Create(StatusCodes.Status404NotFound, mensaje));
            return;
        }

        var respuesta = ErrorResponse.Create(statusCode, ReasonPhrases.GetReasonPhrase(statusCode));
        await http.Response.WriteAsJsonAsync(respuesta);
    }
}
