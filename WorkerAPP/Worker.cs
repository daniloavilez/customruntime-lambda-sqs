using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkerAPP.Services.Interface;

namespace WorkerAPP
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly ISQSService<Amazon.SQS.Model.Message> _sqsService;

        public Worker(ILogger<Worker> logger, ISQSService<Amazon.SQS.Model.Message> sQSService)
        {
            _logger = logger;
            _sqsService = sQSService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var dynamoDbClient = new AmazonDynamoDBClient();

                var sqsConfig = new AmazonSQSConfig();

                sqsConfig.ServiceURL = "http://sqs.sa-east-1.amazonaws.com";

                AmazonSQSClient sqsClient = new AmazonSQSClient(sqsConfig);

                var receiveMessageRequest = new ReceiveMessageRequest();

                receiveMessageRequest.QueueUrl = "https://sqs.sa-east-1.amazonaws.com/265346282853/fila_teste";

                var receiveMessageResponse = sqsClient.ReceiveMessageAsync(receiveMessageRequest);

                _sqsService.ProcessRecords(receiveMessageResponse.Result.Messages);
                
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
