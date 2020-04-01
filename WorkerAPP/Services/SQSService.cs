using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.Lambda.SQSEvents;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using WorkerAPP.Services.Interface;

namespace WorkerAPP.Services
{
    public class SQSService<T> : ISQSService<T>
    {
        private ILogger<SQSService<T>> _logger;

        public SQSService(ILogger<SQSService<T>> logger)
        {
            this._logger = logger;
        }

        public void ProcessRecords(List<T> records)
        {
            // Lambda process
            if (records is List<SQSEvent.SQSMessage>)
            {
                _logger.LogTrace("Verificando se há mensagens");

                if (records != null && records.Any())
                {
                    var messages = records as List<SQSEvent.SQSMessage>;
                    foreach (var message in messages)
                    {
                        _logger.LogInformation($"ID Mensagem: {message.MessageId}");
                        Console.WriteLine(message.Body);
                    }
                }
            }
            // SQS process
            else
            {
                _logger.LogTrace("Verificando se há mensagens");

                if (records != null && records.Any())
                {
                    var messages = records as List<Amazon.SQS.Model.Message>;
                    foreach (var message in messages)
                    {
                        _logger.LogInformation($"ID Mensagem: {message.MessageId}");
                        Console.WriteLine(message.Body);
                    }
                }
            }

        }
    }
}