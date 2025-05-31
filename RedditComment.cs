using Newtonsoft.Json.Linq;
using System.Xml;

namespace gaslighter_no_gaslighting
{
    public class RedditComment
    {
        public static RedditComment FromApiJson(JObject apiJson)
        {
            var id = apiJson["name"]!.ToString().Split('_')[1];
            var threadPermalink = new Uri(apiJson["link_permalink"]!.ToString());
            var permalink = new Uri(threadPermalink, id);
            var threadTitle = apiJson["link_title"]!.ToString();
            var parentId = apiJson["parent_id"]?.ToString()?.Split('_')?[1];
            Uri? parentPermalink = null;
            if (parentId != null)
                parentPermalink = new Uri(threadPermalink, parentId);
            var creationTime = DateTime.UnixEpoch.AddSeconds((int)apiJson["created_utc"]!);
            var bodyHTML = apiJson["body_html"]!.ToString();
            return new RedditComment(id, permalink, threadPermalink, parentPermalink, creationTime, threadTitle, bodyHTML);
        }

        public static RedditComment Deserialize(JObject json)
        {
            var id = json["id"]!.ToString();
            var permalink = new Uri(json["permalink"]!.ToString());
            var threadPermalink = new Uri(json["threadPermalink"]!.ToString());
            Uri? parentPermalink = null;
            if (json.ContainsKey("parentPermalink"))
                parentPermalink = new Uri(json["parentPermalink"]!.ToString());
            var creationTime = DateTime.UnixEpoch.AddSeconds((int)json["creationTime"]!);
            var threadTitle = json["threadTitle"]!.ToString();
            var bodyHTML = json["bodyHTML"]!.ToString();
            return new RedditComment(id, permalink, threadPermalink, parentPermalink, creationTime, threadTitle, bodyHTML);
        }

        public string Id { get; }
        public Uri Permalink { get; }
        public Uri ThreadPermalink { get; }
        public Uri? ParentPermalink { get; }
        public DateTime CreationTime { get; }
        public string ThreadTitle { get; }
        public string BodyHTML { get; }

        private RedditComment(string id, Uri permalink, Uri threadPermalink, Uri? parentPermalink, DateTime creationTime, string threadTitle, string bodyHTML)
        {
            Id = id;
            Permalink = permalink;
            ThreadPermalink = threadPermalink;
            ParentPermalink = parentPermalink;
            CreationTime = creationTime;
            ThreadTitle = threadTitle;
            BodyHTML = bodyHTML;
        }

        public JObject Serialize()
        {
            var obj = new JObject()
            {
                { "id", Id },
                { "permalink", Permalink.ToString() },
                { "threadPermalink", ThreadPermalink.ToString() },
                { "creationTime", (int)Math.Round((CreationTime - DateTime.UnixEpoch).TotalMilliseconds) },
                { "threadTitle", ThreadTitle },
                { "bodyHTML", BodyHTML }
            };
            if (ParentPermalink != null)
                obj.Add("parentPermalink", ParentPermalink.ToString());
            return obj;
        }

        /*public XmlElement Render(XmlDocument document)
        {
            var commentElement = document.CreateElement("div");
            commentElement.SetAttribute("class", "comment");
            var paragraphElement = document.CreateElement("p");
            paragraphElement.InnerXml = $"<b>{CreationTime} / <a href=\"{Permalink}\"></a></b>";
        }*/
    }
}
