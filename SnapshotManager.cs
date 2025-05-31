using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace gaslighter_no_gaslighting
{
    public class SnapshotManager
    {
        public SnapshotManager(string historyJsonPath, string targetUsername, string htmlTemplatePath, string htmlOutputPath, string cookieHeader)
        {
            _HistoryJsonPath = historyJsonPath;
            _TargetUsername = targetUsername;
            //_HtmlTemplate = File.ReadAllText(htmlTemplatePath);
            _HtmlOutputPath = htmlOutputPath;
            var cookieContainer = new CookieContainer();
            cookieContainer.SetCookies(new Uri("https://api.reddit.com"), cookieHeader);
            _Http = new HttpClient(new HttpClientHandler { CookieContainer = cookieContainer });
        }

        private List<RedditComment> _History = new List<RedditComment>();
        private HttpClient _Http = new HttpClient();
        private string _HistoryJsonPath;
        private string _TargetUsername;
        private string _HtmlTemplate;
        private string _HtmlOutputPath;

        public async Task SnapshotRoutineAsync(TimeSpan refreshInterval)
        {
            for (; ;)
            {
                LoadHistory();
                await AcquireNewComments();
                SaveHistory();
                RenderHTML();
                await Task.Delay(refreshInterval);
            }
        }

        private void RenderHTML()
        {
            /*var document = new XmlDocument();
            document.LoadXml(_HtmlTemplate);
            var historyElement = document.GetElementById("history");
            */
        }

        private void SaveHistory()
        {
            var historyJson = new JObject();
            var commentsJson = new JArray();
            foreach (var redditComment in _History)
                commentsJson.Add(redditComment.Serialize());
            historyJson.Add("comments", commentsJson);
            File.WriteAllText(_HistoryJsonPath, historyJson.ToString());
        }

        private void LoadHistory()
        {
            if (File.Exists(_HistoryJsonPath))
            {
                var historyJson = JObject.Parse(File.ReadAllText(_HistoryJsonPath));
                _History.AddRange(historyJson["comments"]!
                    .Cast<JObject>()
                    .Select(RedditComment.Deserialize));
            }
        }

        private async Task AcquireNewComments()
        {
            var latestStoredComment = _History.Count > 0 ? _History[0] : null;
            var latestComments = (await ApiReader.GetLatestComments(_Http, _TargetUsername))
                .TakeWhile(c => c.Id != latestStoredComment?.Id);
            _History.InsertRange(0, latestComments);
        }
    }
}
