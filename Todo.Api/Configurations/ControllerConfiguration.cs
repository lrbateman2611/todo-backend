using Asp.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Todo.Api.Configurations;

public static class ControllerConfiguration
{
    public static IServiceCollection AddControllerService(this IServiceCollection services) =>
        services
        .AddJsonSerializerOptions()
        .AddEndpointsApiExplorer()
        .Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        })
        .AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = ApiVersion.Default;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        })
        .Services;

    public static IServiceCollection AddJsonSerializerOptions(this IServiceCollection services)
        => services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        })
        .Services;

    public static WebApplication ConfigureControllers(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.MapControllers();

        return app;
    }
}

