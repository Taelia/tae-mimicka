using System;
using System.Collections.Generic;
using System.Globalization;

namespace Mimicka
{
    public class User
    {
        public string Name;
        public int MessageCount;
        public int VisitCount;
        public int KappaCount;
        public int HeartCount;
        public int CharacterCount;
        public DateTime FirstSeen = DateTime.Parse("2014-01-01");
        public DateTime LastSeen = DateTime.Parse("2014-01-01");
        public DateTime LastSpoke = DateTime.Parse("2014-01-01");
        public string FirstGame = "None";
        public string LastGame = "None";

        public User(string name, Dictionary<string, string> data = null)
        {
            Name = name;

            if (data != null)
                Initialize(data);
        }

        private void Initialize(Dictionary<string, string> data)
        {
            foreach (var pair in data)
            {
                switch (pair.Key)
                {
                    case "messageCount": MessageCount = int.Parse(data["messageCount"]); break;
                    case "visitCount": VisitCount = int.Parse(data["visitCount"]); break;
                    case "kappaCount": KappaCount = int.Parse(data["kappaCount"]); break;
                    case "heartCount": HeartCount = int.Parse(data["heartCount"]); break;
                    case "characterCount": CharacterCount = int.Parse(data["characterCount"]); break;
                    case "firstSeen": FirstSeen = DateTime.Parse(data["firstSeen"]); break;
                    case "lastSeen": LastSeen = DateTime.Parse(data["lastSeen"]); break;
                    case "lastSpoke": LastSpoke = DateTime.Parse(data["lastSpoke"]); break;
                    case "firstGame": FirstGame = data["firstGame"]; break;
                    case "lastGame": LastGame = data["lastGame"]; break;
                }
            }

            //If its updating an existing user, set a default charactercount.
            if (CharacterCount == 0) CharacterCount = MessageCount * 30;
        }

        public Dictionary<string, string> ToDictionary()
        {
            var data = new Dictionary<string, string>();
            data.Add("messageCount", MessageCount.ToString(CultureInfo.InvariantCulture));
            data.Add("visitCount", VisitCount.ToString(CultureInfo.InvariantCulture));
            data.Add("kappaCount", KappaCount.ToString(CultureInfo.InvariantCulture));
            data.Add("heartCount", HeartCount.ToString(CultureInfo.InvariantCulture));
            data.Add("characterCount", CharacterCount.ToString(CultureInfo.InvariantCulture));
            data.Add("firstSeen", FirstSeen.ToString("s"));
            data.Add("lastSeen", LastSeen.ToString("s"));
            data.Add("lastSpoke", LastSpoke.ToString("s"));
            data.Add("firstGame", FirstGame);
            data.Add("lastGame", LastGame);

            return data;
        }
    }
}