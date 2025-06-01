using Newtonsoft.Json.Linq;
using System.Xml;

namespace gaslighter_no_gaslighting
{
    public class RedditComment
    {
        public static RedditComment FromApiJson(JObject apiJson)
        {
            var id = apiJson["id"]!.ToString();
            var threadPermalink = new Uri(apiJson["link_permalink"]!.ToString());
            var permalink = new Uri(threadPermalink, id);
            var threadTitle = apiJson["link_title"]!.ToString();
            var parentId = apiJson["parent_id"]?.ToString()?.Split('_')?[1];
            Uri? parentPermalink = null;
            if (parentId != null)
                parentPermalink = new Uri(threadPermalink, parentId);
            var creationTime = DateTime.UnixEpoch.AddSeconds((int)apiJson["created_utc"]!).AddHours(-4); // dont ask me why its 4. it makes no sense
            var body = apiJson["body"]!.ToString();
            return new RedditComment(id, permalink, threadPermalink, parentPermalink, creationTime, threadTitle, body);
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
            var body = json["body"]!.ToString();
            return new RedditComment(id, permalink, threadPermalink, parentPermalink, creationTime, threadTitle, body);
        }

        public string Id { get; }
        public Uri Permalink { get; }
        public Uri ThreadPermalink { get; }
        public Uri? ParentPermalink { get; }
        public DateTime CreationTime { get; }
        public string ThreadTitle { get; }
        public string Body { get; }

        private RedditComment(string id, Uri permalink, Uri threadPermalink, Uri? parentPermalink, DateTime creationTime, string threadTitle, string body)
        {
            Id = id;
            Permalink = permalink;
            ThreadPermalink = threadPermalink;
            ParentPermalink = parentPermalink;
            CreationTime = creationTime;
            ThreadTitle = threadTitle;
            Body = body;
        }

        public JObject Serialize()
        {
            var obj = new JObject()
            {
                { "id", Id },
                { "permalink", Permalink.ToString() },
                { "threadPermalink", ThreadPermalink.ToString() },
                { "creationTime", (int)Math.Round((CreationTime - DateTime.UnixEpoch).TotalSeconds) },
                { "threadTitle", ThreadTitle },
                { "body", Body }
            };
            if (ParentPermalink != null)
                obj.Add("parentPermalink", ParentPermalink.ToString());
            return obj;
        }

        public string RenderMD()
        {
            return $"> {CreationTime.ToString()} [Permalink]({Permalink}) / [Context]({ParentPermalink ?? ThreadPermalink})\n\n{Body}";
        }
    }
}
