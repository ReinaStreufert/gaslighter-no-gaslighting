
using gaslighter_no_gaslighting;
using Newtonsoft.Json.Linq;

//if (args.Length != 1)
//    Console.WriteLine($"usage: gaslighter-no-gaslighting latest-snapshot.json");

var manager = new SnapshotManager("persistence.json", "C:\\Users\\Reina\\Documents\\GitHub\\gaslighter-no-gaslighting\\comment-log.md");
manager.IncludeSnapshot(JObject.Parse(File.ReadAllText("reddit-latest.json")));
//manager.Clean();