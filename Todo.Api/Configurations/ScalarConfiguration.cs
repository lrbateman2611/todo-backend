using Scalar.AspNetCore;
using Todo.Api.Constants;
using Todo.Api.Models;
using Todo.Api.Services;

namespace Todo.Api.Configurations;

public static class ScalarConfiguration
{
    public static WebApplication ConfigureScalar(this WebApplication app, IConfiguration configuration, IServiceCollection services)
    {
        if (!app.Environment.IsProduction())
        {
            IBitwardenService bitwarden = app.Services.GetRequiredService<IBitwardenService>();
            Auth0Secret? auth0Details = bitwarden.GetSecret<Auth0Secret>(SecretIds.Auth0);

            app.MapScalarApiReference(options =>
            {
                options
                    .HideModels()
                    .ExpandAllTags()
                    .HideClientButton()
                    .AddDocument("v1")
                    .WithTitle("Todo API")
                    .WithJsonDocumentDownload()
                    .AddOAuth2Flows("Auth0", options =>
                    {
                        options.AuthorizationCode = new AuthorizationCodeFlow
                        {
                            ClientId = auth0Details?.ClientId,
                            AuthorizationUrl = $"{auth0Details?.Domain}authorize?audience={Uri.EscapeDataString(auth0Details?.Audience ?? "")}",
                            TokenUrl = $"{auth0Details?.Domain}oauth/token",
                            SelectedScopes = ["openId", "profile", "email"],
                            Pkce = Pkce.Sha256
                        };
                    });
            });
        }

        return app;
    }
}
