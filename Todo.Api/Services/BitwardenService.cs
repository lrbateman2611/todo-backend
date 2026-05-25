using Bitwarden.Sdk;
using System.Text.Json;

namespace Todo.Api.Services;

public interface IBitwardenService
{
    BitwardenClient Client { get; }
    T? GetSecret<T>(Guid secretId);
}

public class BitwardenService : IBitwardenService
{
    public BitwardenClient Client {  get; set; }
    public BitwardenService(IConfiguration configuration)
    {
        Client = new BitwardenClient();

        Client.Auth.LoginAccessToken(configuration["Bitwarden:Token"]!);
    }

    public T? GetSecret<T>(Guid secretId)
    {
        var response = Client.Secrets.Get(secretId);
        return JsonSerializer.Deserialize<T>(response.Value);
    }
}
