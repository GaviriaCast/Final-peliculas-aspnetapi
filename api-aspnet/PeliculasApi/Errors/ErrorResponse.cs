using Microsoft.AspNetCore.WebUtilities;

namespace PeliculasApi.Errors;

/// <summary>
/// Forma estandar de los errores de NestJS, que es la que leen Vue e Ionic:
///   { "message": "No existe la pelicula con id 9", "error": "Not Found", "statusCode": 404 }
/// En errores de validacion, message es un arreglo de textos.
/// </summary>
public record ErrorResponse(object Message, string Error, int StatusCode)
{
    public static ErrorResponse Create(int statusCode, object message) =>
        new(message, ReasonPhrases.GetReasonPhrase(statusCode), statusCode);
}
