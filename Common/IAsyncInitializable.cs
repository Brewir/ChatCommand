using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CC.Common;

public interface IAsyncInitializable
{
    Task InitializeAsync(CancellationToken token);
}

internal class InitializerService<TService> : IHostedService
{
    private readonly TService _service;

    public InitializerService(TService service)
    {
        _service = service;
    }
    public Task StartAsync(CancellationToken token)
        => (_service as IAsyncInitializable)?.InitializeAsync(token) ?? Task.CompletedTask;

    public Task StopAsync(CancellationToken token) => Task.CompletedTask;
}
public static class HostingExtensions
{
    public static IServiceCollection AddInitializableSingleton<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
    {
        return services
                .AddSingleton<TService, TImplementation>()
                .AddHostedService<InitializerService<TService>>();
    }
}
