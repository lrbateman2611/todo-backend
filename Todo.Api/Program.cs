using Todo.Api.Configurations;
using Todo.Api.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddTransient<IBitwardenService, BitwardenService>()
    .ConfigureSupabase(builder.Configuration)
    .AddDependencies()
    .AddAuthenticationService()
    .AddControllerService()
    .AddCorsConfiguration()
    .ConfigureOpenApiDocuments();

// Add health checks
builder.Services.AddHealthChecks();

WebApplication app = builder.Build();

app.MapOpenApi();

// Add root endpoint
app.MapGet("/", () => Results.Ok(new 
{ 
    status = "healthy",
    service = "Todo API",
    version = "1.0.0",
    timestamp = DateTime.UtcNow
}));

// Map health check
app.MapHealthChecks("/health");

app.UseCors("clientApp");

app
    .ConfigureScalar(builder.Configuration, builder.Services)
    .ConfigureControllers()
    .UseAuthentication()
    .UseAuthorization();

await app.RunAsync();
