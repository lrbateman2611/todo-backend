using Todo.Api.Data;
using Todo.Api.Services;

namespace Todo.Api.Configurations;

public static class DependencyConfiguration
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
        => services
            .AddHttpContextAccessor()
            .AddTransient<IBitwardenService, BitwardenService>()
            .AddScoped<ITodoService, TodoService>()
            .AddScoped<ITodoData, TodoData>()
            .AddScoped<IUserService, UserService>();
}
