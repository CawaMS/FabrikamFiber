using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;

namespace FabrikamFiber.Web.AIExtensions
{

    public class StaticContentFilter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        // Link processors to each other in a chain.
        public StaticContentFilter(ITelemetryProcessor next)
        {
            this.Next = next;
        }

        public void Process(ITelemetry item)
        {
            var request = item as RequestTelemetry;

            if (request != null)
            {
                if (request.Url.AbsolutePath.StartsWith("/Content") ||
                    request.Url.AbsolutePath.StartsWith("/Scripts"))
                {
                    // To filter out an item, just terminate the chain: 
                    return;
                }
            }

            this.Next.Process(item);
        }

    }
}