namespace Todo.Api.Configurations;

public static class CorsConfiguration
{
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
        => services.AddCors(options =>
        {
            options.AddPolicy(name: "clientApp",
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                }
            );
        });
    
}
