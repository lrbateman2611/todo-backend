namespace Todo.Api.Configurations;

public static class CorsConfiguration
{
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IHostEnvironment environment)
        => services.AddCors(options =>
        {
            options.AddPolicy(name: "clientApp",
                builder =>
                {
                    builder
                        .SetIsOriginAllowed(origin => IsOriginAllowed(origin, environment))
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
            );
        });

    private static bool IsOriginAllowed(string origin, IHostEnvironment environment)
    {
        if (origin == "https://lrbateman-todo.netlify.app")
            return true;

        if (environment.IsDevelopment())
        {
            if (origin.StartsWith("http://localhost"))
                return true;

            if (origin.StartsWith("https://deploy-preview-") && origin.Contains("--lrbateman-todo.netlify.app"))
                return true;
        }

        return false;
    }
}
