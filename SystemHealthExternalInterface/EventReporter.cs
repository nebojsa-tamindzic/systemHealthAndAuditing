﻿/****************************************************************************************
*	This code originates from the software development department at					*
*	swedish insurance and private loan broker Insplanet AB.								*
*	Full license available in license.txt												*
*	This text block may not be removed or altered.                                  	*
*	The list of contributors may be extended.                                           *
*																						*
*							Mikael Axblom, head of software development, Insplanet AB	*
*																						*
*	Contributors: Mikael Axblom															*
*****************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SystemHealthExternalInterface
{
    public class EventReporter
    {
        private string Send_EventHubConnectionstring { get; }
        private string EventHubPath { get; }
        private EventHubClient Client { get; }

        public EventReporter(string sendConnectionstring, string eventHubPath)
        {
            Send_EventHubConnectionstring = sendConnectionstring;
            EventHubPath = eventHubPath;
            Client = EventHubClient.CreateFromConnectionString(Send_EventHubConnectionstring, EventHubPath);
        }

        public async Task ReportEventAsync(SystemEvent @event)
        {
            await Client.SendAsync(new EventData(Encoding.UTF8.GetBytes(Serialise(@event))));
        }

        public async Task ReportEventBatchAsync(List<SystemEvent> events)
        {
            await Client.SendBatchAsync(events.Select(@event => new EventData(Encoding.UTF8.GetBytes(Serialise(@event)))).ToList());
        }

        private static string Serialise(SystemEvent @event)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            return JsonConvert.SerializeObject(@event, settings);
        }

    }
}
