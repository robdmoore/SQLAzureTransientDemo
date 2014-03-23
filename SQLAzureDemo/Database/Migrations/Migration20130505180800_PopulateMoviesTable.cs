using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using DbUp.Engine;
using Newtonsoft.Json;
using SQLAzureDemo.Database.Models;

namespace SQLAzureDemo.Database.Migrations
{
    public class Migration20130505180800_PopulateMoviesTable : IScript
    {
        public string ProvideScript(IDbConnection connection)
        {
            var client = new HttpClient();
            var titleSearches = new[] {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
            var movies = new List<Movie>();
            foreach (var titleSearch in titleSearches)
            {
                var result = client.GetStringAsync(string.Format("http://omdbapi.org/?limit=10&title={0}&type=json", titleSearch)).Result;
                movies.AddRange(JsonConvert.DeserializeObject<Movie[]>(result));
            }

            using (var command = connection.CreateCommand())
            {
                var s = new StringBuilder();
                s.Append("INSERT INTO Movie (Title, Year) VALUES ");

                for (var i = 0; i < movies.Count; i++)
                {
                    if (i > 0)
                        s.Append(", ");
                    s.Append(string.Format("(@title{0}, @year{0})", i));
                    command.Parameters.Add(new SqlParameter(string.Format("title{0}", i), movies[i].Title));
                    command.Parameters.Add(new SqlParameter(string.Format("year{0}", i), movies[i].Year));
                }

                command.CommandText = s.ToString();

                connection.Open();
                command.ExecuteNonQuery();
            }

            return string.Empty;
        }
    }
}
