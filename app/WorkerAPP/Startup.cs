using Microsoft.Extensions.DependencyInjection;
using WorkerAPP.Services;
using WorkerAPP.Services.Interface;

public class Startup
{
    public static void ConfigureServices(IServiceCollection serviceCollection) {
        serviceCollection.AddLogging();
        serviceCollection.AddTransient(typeof(ISQSService<>), typeof(SQSService<>));
    }
}