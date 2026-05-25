
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;
using Todo.Api.Constants;
using Todo.Api.Models;
using Todo.Api.Services;

namespace Todo.Api.Configurations;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddAuthenticationService(this IServiceCollection services)
        => services
            .AddHeaderPropagation(options =>
            {
                options.Headers.Add("Authorization");
            })
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer()
            .Services.AddAuthorization()
            .AddTransient<IConfigureOptions<JwtBearerOptions>>(serviceProvider =>
            new ConfigureNamedOptions<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    IConfiguration config = serviceProvider.GetRequiredService<IConfiguration>();
                    IBitwardenService bitwarden = serviceProvider.GetRequiredService<IBitwardenService>();

                    var secretResponse = bitwarden.Client.Secrets.Get(SecretIds.Auth0);
                    Auth0Secret auth0Details = JsonSerializer.Deserialize<Auth0Secret>(secretResponse.Value)!;

                    options.Authority = auth0Details.Domain;
                    options.Audience = auth0Details.Audience;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = ClaimTypes.NameIdentifier
                    };
                }));
}
