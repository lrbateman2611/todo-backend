using Todo.Api.Configurations;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
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
