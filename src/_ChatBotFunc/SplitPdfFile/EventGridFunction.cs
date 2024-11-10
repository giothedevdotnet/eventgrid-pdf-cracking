// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using Newtonsoft.Json;

namespace ChatBotFunc.SplitPdfFile
{
    public static class EventGridFunction
    {
        [FunctionName("EventGridFunction")]
        public static void Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());

            var blobEvent = JsonConvert.DeserializeObject<BlobEventData>(eventGridEvent.Data.ToString());
        }
    }
}
