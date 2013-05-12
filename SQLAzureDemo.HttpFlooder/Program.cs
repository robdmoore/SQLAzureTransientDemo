using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SQLAzureDemo.HttpFlooder
{
    class Program
    {
        private static readonly Regex Regex = new Regex(@"There are <span class=""label label-info"">(\d+)</span> that were returned with an average creation year of <span class=""label label-info"">(\d+)</span>");
        static void Main()
        {
            const string url = "http://mscloudperthdemo.azurewebsites.net/Home/Transient?q={0}";
            ServicePointManager.DefaultConnectionLimit = 500;
            ServicePointManager.MaxServicePointIdleTime = 5*60*1000;
            var random = new Random();
            var tasks = new List<Task>();

            for (var i = 0; i < 500; i++)
            {
                var searchTerm = GetRandomSearchString(random);
                var stopwatch = Stopwatch.StartNew();
                Console.WriteLine("Beginning search for {0}", searchTerm);
                tasks.Add(
                    SendHttpRequest(url, searchTerm)
                        .ContinueWith(async r => await ProcessHttpResponse(stopwatch, searchTerm, r.Result)).Unwrap()
                        .ContinueWith(s => Console.WriteLine(s.Result))
                );
            }
            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException e)
            {
                foreach (var exception in e.InnerExceptions)
                    Console.WriteLine(exception);
            }
            Console.WriteLine("Finished. Press any key...");
            Console.ReadLine();
        }

        private static async Task<string> ProcessHttpResponse(Stopwatch stopwatch, string searchTerm, HttpResponseMessage response)
        {
            stopwatch.Stop();

            var s = new StringBuilder();
            var statusCode = response.StatusCode;
            var content = await response.Content.ReadAsStringAsync();
            var match = Regex.Match(content);
            
            s.Append(string.Format("Finished search for {0} with HTTP status code of {1} in {2}s.", searchTerm, response.StatusCode, stopwatch.Elapsed.TotalSeconds));

            if (statusCode == HttpStatusCode.OK)
                if (match.Success)
                    s.Append(string.Format(" {0} Results with average creation year of {1}.", match.Groups[1].Value, match.Groups[2].Value));
                else
                    s.Append("No results returned.");

            return s.ToString();
        }

        public static async Task<HttpResponseMessage> SendHttpRequest(string url, string searchTerm)
        {
            var client = new HttpClient();
            return await client.GetAsync(string.Format(url, searchTerm));
        }

        private static string GetRandomSearchString(Random random)
        {
            var searchTerms = new[]
            {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
            };
            var searchTerm = new StringBuilder();
            var numLetters = random.Next(1, 6);
            for (var j = 0; j < numLetters; j++)
                searchTerm.Append(searchTerms[random.Next(0, searchTerms.Length)]);
            return searchTerm.ToString();
        }
    }
}
