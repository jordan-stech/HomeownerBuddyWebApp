using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HOB_WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Thread Created
            Thread t = new Thread(new ThreadStart(TimerThreadProc));
            t.Start();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void TimerThreadProc()
        {
            int timeval = 1800 * 1000;
            System.Timers.Timer timer1 = new System.Timers.Timer
            {
                Interval = timeval
            };
            timer1.Elapsed += OnTimedEvent;
            timer1.AutoReset = true;
            timer1.Enabled = true;
        }

        private static async void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            await ApiCall();

        }

        public static async Task ApiCall()
        {
            // Set up new HttpClientHandler and its credentials so we can perform the web request
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            // Create new httpClient using our client handler created above
            HttpClient httpClient = new HttpClient(clientHandler);

            String apiUrl = "https://habitathomeownerbuddy.azurewebsites.net/api/BackgroundAPI";
            //String postApiUrl = "https://habitathomeownerbuddy.azurewebsites.net/api/BackgroundAPI";

            // Create new URI with the API url so we can perform the web request
            var uri = new Uri(string.Format(apiUrl, string.Empty));

            var getResponse = await httpClient.GetAsync(uri);

            if(getResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Success");
            }
        }
    }
}
