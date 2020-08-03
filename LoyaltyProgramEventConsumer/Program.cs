using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;

using static System.Console;


namespace LoyaltyProgramEventConsumer
{

    public class Program: ServiceBase
    {
        private EventSubscriber subscriber;

        public static void Main(string[] args) => new Program().Main();
         public void Main()
        {
            this.subscriber = new EventSubscriber("localhost:5000");
            OnStart(null);
            ReadLine();
        }
        protected override void OnStart(string[] args)
        {
            this.subscriber.Start();
        }

        protected override void OnStop()
        {
            this.subscriber.Stop();
        }
    }

    public class EventSubscriber
    {
        private readonly string loyaltyProgramHost;
        private long start = 0, chunkSize = 100;
        private readonly Timer timer;


        public EventSubscriber(string loyaltyProgramHost)
        {
            WriteLine("created");
            this.loyaltyProgramHost = loyaltyProgramHost;
            this.timer = new Timer(10 * 1000);
            this.timer.AutoReset = false;
            this.timer.Elapsed += (_, __) => SubscriptionCycleCallback().Wait();
        }

        private async Task SubscriptionCycleCallback()
        {
            var response = await ReadEvents();
            if (response.StatusCode == HttpStatusCode.OK)
               await HandleEvents(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            this.timer.Start();
        }

        private async Task<HttpResponseMessage> ReadEvents()
        {
            var startNumber = await ReadStartNumber() ;
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri($"http://{this.loyaltyProgramHost}");
                var response = await httpClient.GetAsync($"/events/?start={this.start}&end={this.start + this.chunkSize}")
                    .ConfigureAwait(false);
                PrettyPrintResponse(response);
                return response;
            }
        }

        private async Task<long>  ReadStartNumber()
        {
            return await  Task.FromResult(10L);
        }

        private async Task HandleEvents(string content)
        {
            var lastSucceededEvents = 0L; 
            var events = JsonConvert.DeserializeObject<IEnumerable<Event>>(content); 
            foreach (var ev in events)
            { 
                dynamic eventData = ev.Content;
                if (ShouldSendNotification(eventData))
                {
                   var noticificationSucceeded= await SendNoticification(eventData).ConfiguraAwait(false);
                    if (!noticificationSucceeded)
                    {
                        return;
                    }
                }
                lastSucceededEvents = ev.SequenceNumber + 1; 
            }
              WriteStartNumber(lastSucceededEvents);
        }

        private  void WriteStartNumber(long lastSucceededEvents)
        {
            throw new NotImplementedException();
        }

        private object SendNoticification(dynamic eventData)
        {
            throw new NotImplementedException();
        }

        private bool ShouldSendNotification(dynamic eventData)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            this.timer.Start();
        }

        public void Stop()
        {
            this.timer.Stop();
        }

        private static async void PrettyPrintResponse(HttpResponseMessage response)
        {
            WriteLine("Status code: " + response?.StatusCode.ToString() ?? "command failed");
            WriteLine("Headers: " + response?.Headers.Aggregate("", (acc, h) => acc + "\n\t" + h.Key + ": " + h.Value) ?? "");
            WriteLine("Body: " + await response?.Content.ReadAsStringAsync() ?? "");
        }

    }
}
