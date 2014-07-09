using System;
using System.Collections.Generic;
using System.Globalization;

namespace Mimicka
{ 
    public class Game
    {
	    public string Name;
        public DateTime FirstPlayed = DateTime.Parse("2014-01-01");
        public DateTime LastPlayed = DateTime.Parse("2014-01-01");
        public int DaysPlayed;
        public int VisitCount;
        public int MessageCount; // total messages sent during the game
        public int UniqueMessageCount; // count of unique users who spoke during the game
        
        public Game(string name, Dictionary<string, string> data = null)
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
                    case "firstPlayed": FirstPlayed = DateTime.Parse(data["firstPlayed"]); break;
                    case "lastPlayed": LastPlayed = DateTime.Parse(data["lastPlayed"]); break;
                    case "daysPlayed": DaysPlayed = int.Parse(data["daysPlayed"]); break;
                    case "visitCount": VisitCount = int.Parse(data["visitCount"]); break;
                    case "messageCount": MessageCount = int.Parse(data["messageCount"]); break;
                    case "uniqueMessageCount": UniqueMessageCount = int.Parse(data["uniqueMessageCount"]); break;
                }
            }
        }

        public Dictionary<string, string> ToDictionary()
        {
            var data = new Dictionary<string, string>();
            data.Add("firstPlayed", FirstPlayed.ToString("s"));
            data.Add("lastPlayed", LastPlayed.ToString("s"));
            data.Add("daysPlayed", DaysPlayed.ToString(CultureInfo.InvariantCulture));
            data.Add("visitCount", VisitCount.ToString(CultureInfo.InvariantCulture));
            data.Add("messageCount", MessageCount.ToString(CultureInfo.InvariantCulture));
            data.Add("uniqueMessageCount", UniqueMessageCount.ToString(CultureInfo.InvariantCulture));
            return data;
        }
    }
}