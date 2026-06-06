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
        // Allow production domain in all environments
        if (origin == "https://lrbateman-todo.netlify.app")
            return true;

        // Only allow localhost and preview URLs in development
        if (environment.IsDevelopment())
        {
            // Allow localhost development
            if (origin.StartsWith("http://localhost"))
                return true;

            // Allow Netlify deploy preview URLs matching pattern: https://deploy-preview-*--lrbateman-todo.netlify.app
            if (origin.StartsWith("https://deploy-preview-") && origin.Contains("--lrbateman-todo.netlify.app"))
                return true;
        }

        return false;
    }
}
