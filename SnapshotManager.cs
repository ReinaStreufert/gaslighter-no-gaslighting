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
        public SnapshotManager(string historyJsonPath, string mdOutputPath)
        {
            _HistoryJsonPath = historyJsonPath;
            _MDlOutputPath = mdOutputPath;
        }

        private List<RedditComment> _History = new List<RedditComment>();
        private string _HistoryJsonPath;
        private string _MDlOutputPath;

        public void IncludeSnapshot(JObject latestSnapshot)
        {
            LoadHistory();
            var latestStoredComment = _History.Count > 0 ? _History[0] : null;
            var latestComments = latestSnapshot["data"]!["children"]!
                .Select(t => t["data"]!)
                .Cast<JObject>()
                .Select(RedditComment.FromApiJson)
                .TakeWhile(c => c.Id != latestStoredComment?.Id);
            _History.InsertRange(0, latestComments);
            SaveHistory();
            RenderMD();
        }

        private void RenderMD()
        {
            var text = string.Join("\n\n", _History.Select(c => c.RenderMD()));
            File.WriteAllText(_MDlOutputPath, text);
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
    }
}
