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


WebApplication app = builder.Build();

app.MapOpenApi();

app
    .ConfigureScalar(builder.Configuration, builder.Services)
    .ConfigureControllers()
    .UseAuthentication()
    .UseAuthorization();

await app.RunAsync();
