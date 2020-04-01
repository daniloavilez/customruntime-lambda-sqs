using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;

namespace WorkerAPP.Services.Interface
{
    public interface ISQSService<T>
    {
        void ProcessRecords(List<T> records);
    }
}