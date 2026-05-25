using Supabase;
using Todo.Api.Constants;
using Todo.Api.Models;
using Todo.Api.Services;

namespace Todo.Api.Configurations;

public static class DatabaseConfiguration
{
    public static IServiceCollection ConfigureSupabase(this IServiceCollection services, IConfiguration configuration)
        => services.AddSingleton(provider =>
            {
                IBitwardenService bitwarden = provider.GetRequiredService<IBitwardenService>();
                SupabaseSecret? connectionDetails = bitwarden.GetSecret<SupabaseSecret>(SecretIds.Supabase);

                if (connectionDetails == null)
                {
                    throw new ArgumentNullException(nameof(connectionDetails));
                }

                return new Client(
                    connectionDetails.Url,
                    connectionDetails.Key,
                    new SupabaseOptions
                    {
                        AutoRefreshToken = true,
                        AutoConnectRealtime = true,
                    }
                );
            }
        );
}
