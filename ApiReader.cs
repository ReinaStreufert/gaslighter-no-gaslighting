using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gaslighter_no_gaslighting
{
    public static class ApiReader
    {
        public static async Task<IEnumerable<RedditComment>> GetLatestComments(HttpClient httpClient, string username)
        {
            var responseText = await httpClient.GetStringAsync($"https://api.reddit.com/user/{username}/comments");
            var responseJson = JObject.Parse(responseText);
            return responseJson["data"]!["children"]!
                .Cast<JObject>()
                .Select(c => RedditComment.FromApiJson(c));
        }
    }
}
