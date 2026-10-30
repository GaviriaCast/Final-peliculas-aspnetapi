using Microsoft.AspNetCore.Diagnostics;

namespace PeliculasApi.Errors;

/// <summary>
/// Manejador global de excepciones (lo que hacen los exception filters de NestJS).
/// Cualquier excepcion que salga de un controlador o servicio termina aqui.
/// </summary>
public class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ErrorResponse respuesta;

        if (exception is ApiException apiException)
        {
            respuesta = ErrorResponse.Create(apiException.StatusCode, apiException.Message);
        }
        else
        {
            // Error no esperado: se registra en consola y no se le muestra el detalle al cliente
            logger.LogError(exception, "Error no controlado");
            respuesta = ErrorResponse.Create(StatusCodes.Status500InternalServerError, "Internal server error");
        }

        httpContext.Response.StatusCode = respuesta.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(respuesta, cancellationToken);
        return true;
    }
}
