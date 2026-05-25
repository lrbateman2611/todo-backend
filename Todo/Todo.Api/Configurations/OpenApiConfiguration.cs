using Microsoft.OpenApi;

namespace Todo.Api.Configurations;

public static class OpenApiConfiguration
{
    public static IServiceCollection ConfigureOpenApiDocuments(this IServiceCollection services)
        => services
            .AddOpenApi("v1", options =>
            {
                options.AddDocumentTransformer((document, _, _) =>
                {
                    document.Info = new()
                    {
                        Title = "Todo App API",
                        Version = "v1",
                        Description = "An API for managing todo items"
                    };

                    document.AddComponent("Auth0", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            AuthorizationCode = new OpenApiOAuthFlow
                            {
                                Scopes = new Dictionary<string, string>
                                {
                                    ["openid"] = "OpenID",
                                    ["profile"] = "Profile",
                                    ["email"] = "Email"
                                }
                            }
                        }
                    });

                    document.Security ??= [];
                    document.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Auth0", document)] = new List<string> { "openid", "profile", "email" }
                    });

                    return Task.CompletedTask;
                });
            });
}
