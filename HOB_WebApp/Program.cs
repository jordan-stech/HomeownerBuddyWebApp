using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HOB_WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Thread Created
            Thread t = new Thread(new ThreadStart(FirebaseTimerThreadProc));
            t.Start();

            Thread t2 = new Thread(new ThreadStart(DueDateTimerThreadProc));
            t2.Start();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void FirebaseTimerThreadProc()
        {
            // Set to once per week
            int timeval = 604800000;
            System.Timers.Timer timer1 = new System.Timers.Timer
            {
                Interval = timeval
            };
            timer1.Elapsed += OnTimedEvent1;
            timer1.AutoReset = true;
            timer1.Enabled = true;
        }

        public static void DueDateTimerThreadProc()
        {
            // Set to once every hour
            int timeval = 10000;
            System.Timers.Timer timer1 = new System.Timers.Timer
            {
                Interval = timeval
            };
            timer1.Elapsed += OnTimedEvent2;
            timer1.AutoReset = true;
            timer1.Enabled = true;
        }

        private static async void OnTimedEvent1(Object source, System.Timers.ElapsedEventArgs e)
        {
            await FirebaseApiCall();

        }

        private static async void OnTimedEvent2(Object source, System.Timers.ElapsedEventArgs e)
        {
            await DueDateApiCall();

        }

        public static async Task FirebaseApiCall()
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

        public static async Task DueDateApiCall()
        {
            string userId = "noId";

            // Set up new HttpClientHandler and its credentials so we can perform the web request
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            // Create new httpClient using our client handler created above
            HttpClient httpClient = new HttpClient(clientHandler);

            String apiUrl = "https://habitathomeownerbuddy.azurewebsites.net/api/BackgroundAPI/" + userId;

            // Create new URI with the API url so we can perform the web request
            var uri = new Uri(string.Format(apiUrl, string.Empty));

            string JSONresult = JsonConvert.SerializeObject(userId);
            Console.WriteLine(JSONresult);

            var content = new StringContent(JSONresult, Encoding.UTF8, "application/json");

            var putResponse = await httpClient.PutAsync(uri, content);

            if (putResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Success");
            }
        }
    }
}
