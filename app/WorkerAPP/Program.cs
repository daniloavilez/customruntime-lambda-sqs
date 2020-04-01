using System;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.Json;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkerAPP.Services;
using WorkerAPP.Services.Interface;

namespace WorkerAPP
{
    public class Program
    {
        private static ISQSService<SQSEvent.SQSMessage> _sqsService = null;

        public static void Main(string[] args)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME")))
            {
                CreateHostBuilder(args).Build().Run();
            }
            else
            {
                ServiceProvider serviceProvider = BuildServiceProvider();
                _sqsService = serviceProvider.GetService<ISQSService<SQSEvent.SQSMessage>>();
                // _sqsService.QueueUrl = "https://sqs.sa-east-1.amazonaws.com/265346282853/fila_teste";

                Action<SQSEvent, ILambdaContext> sqsFunctionHandler = ProcessIncoming;
                using (var handlerWrapper = HandlerWrapper.GetHandlerWrapper(sqsFunctionHandler, new JsonSerializer()))
                using (var bootstrap = new LambdaBootstrap(handlerWrapper))
                {
                    bootstrap.RunAsync().Wait();
                }
            }
        }

        private static ServiceProvider BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            Startup.ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        private static void ProcessIncoming(SQSEvent @event, ILambdaContext lambdaContext)
        {
            _sqsService.ProcessRecords(@event.Records);            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    Startup.ConfigureServices(services);

                    services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        
                    services.AddHostedService<Worker>();
                });
    }
}
