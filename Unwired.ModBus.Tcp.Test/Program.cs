using Unwired.ModBus.Tcp.Implementations;
using Unwired.ModBus.Tcp.Interfaces;
using Unwired.ModBus.Tcp.Test;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IUnwiredModBusClient, UnwiredModBusClient>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
