using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CoreServer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<Startup>();
            })
            .Build();
        await host.RunAsync();
    }
}