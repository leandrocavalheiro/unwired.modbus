using Microsoft.Extensions.DependencyInjection;
using Unwired.ModBus.Tcp.Enumarators;
using Unwired.ModBus.Tcp.Implementations;
using Unwired.ModBus.Tcp.Interfaces;

namespace Unwired.ModBus.Tcp.Configurations;

public static class ModBusConfiguration
{
    public static void AddUnwiredModBusTcp(this IServiceCollection services, InjectionType injectionType = InjectionType.SINGLETON)
    {

        switch (injectionType)
        {
            case InjectionType.SCOPED:
                services.AddScoped<IUnwiredModBusClient, UnwiredModBusClient>();
                break;
            case InjectionType.SINGLETON:
                services.AddSingleton<IUnwiredModBusClient, UnwiredModBusClient>();
                break;
            default:
                services.AddTransient<IUnwiredModBusClient, UnwiredModBusClient>();
                break;
        }

    }
}
