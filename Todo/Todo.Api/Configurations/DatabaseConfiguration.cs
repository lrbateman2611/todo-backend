using Supabase;

namespace Todo.Api.Configurations;

public static class DatabaseConfiguration
{
    public static IServiceCollection ConfigureSupabase(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddSingleton(provider =>
            new Client(configuration["Supabase:Url"]!,
            configuration["Supabase:Key"],
            new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
            }
            ));
    }
}
