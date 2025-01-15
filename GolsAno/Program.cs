using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args) 
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = await getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = await getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint-Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static async Task<int> getTotalScoredGoals(string team, int year)
    {
        int totalGoals = 0;
        totalGoals += await getGoalsForTeam(team, year, "team1");
        totalGoals += await getGoalsForTeam(team, year, "team2");
        return totalGoals;
    }

    private static async Task<int> getGoalsForTeam(string team, int year, string teamType)
    {
        int totalGoals = 0;
        int page = 1;
        int totalPage = 1;

        while (page <= totalPage)
        {
            string url = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&{teamType}={team}&page={page}";
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject json = JObject.Parse(responseBody);
                foreach (var match in json["data"])
                {
                    if (teamType == "team1")
                    {
                        totalGoals += int.Parse((string)match["team1goals"]);
                    }
                    else if (teamType == "team2")
                    {
                        totalGoals += int.Parse((string)match["team2goals"]);
                    }
                }

                totalPage = (int)json["total_pages"];
                page++;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                break;
            }
        }

        return totalGoals;
    }
}
