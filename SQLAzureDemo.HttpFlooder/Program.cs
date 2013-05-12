using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SQLAzureDemo.HttpFlooder
{
    class Program
    {
        static void Main()
        {
            const string url = "http://mscloudperthdemo.azurewebsites.net/Home/Transient?q={0}";
            ServicePointManager.DefaultConnectionLimit = 20;
            var random = new Random();
            var tasks = new List<Task>();

            for (var i = 0; i < 20; i++)
            {
                var searchTerm = GetRandomSearchString(random);
                var stopwatch = Stopwatch.StartNew();
                Console.WriteLine("Beginning search for {0}", searchTerm);
                tasks.Add(
                    SendHttpRequest(url, searchTerm).ContinueWith(t =>
                        {
                            stopwatch.Stop();
                            Console.WriteLine("Finished search for {0} with HTTP status code of {1} in {2}s", searchTerm, t.Result.StatusCode, stopwatch.Elapsed.TotalSeconds);
                        }
                    )
                );
            }

            Task.WaitAll(tasks.ToArray());
            Console.ReadLine();
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
