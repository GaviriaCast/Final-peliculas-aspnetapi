using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace PeliculasApi.OpenApi;

/// <summary>
/// Declara en el documento OpenAPI el esquema "Bearer" (JWT) y lo marca como requerido
/// en los endpoints con [Authorize]. Asi Scalar muestra el candado y deja pegar el token.
/// </summary>
public static class BearerSecurityTransformer
{
    public const string SchemeName = "Bearer";

    public static Task TransformDocument(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Pega el access_token que devuelve POST /api/auth/login",
        };

        return Task.CompletedTask;
    }

    public static Task TransformOperation(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var requiereToken = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<IAuthorizeData>()
            .Any();

        if (requiereToken)
        {
            operation.Security ??= [];
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(SchemeName, context.Document)] = [],
            });
        }

        return Task.CompletedTask;
    }
}
