namespace PeliculasApi.Errors;

/// <summary>
/// Error con codigo HTTP, equivalente a NotFoundException, ConflictException, etc. de NestJS.
/// Se lanza desde los servicios y <see cref="ApiExceptionHandler"/> lo convierte en la respuesta JSON.
///
/// Ejemplos:
///   throw new ApiException(StatusCodes.Status404NotFound, "No existe la pelicula con id 9");
///   throw new ApiException(StatusCodes.Status409Conflict, "Ese correo ya esta registrado");
/// </summary>
public class ApiException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
