using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TomeLib.Db;
using TomeLib.Twitch;
using Tomestone.Models;

namespace Tomestone
{
    public class UserDatabase
    {
        private Database _db;
        private TwitchConnection _twitch;

        private Dictionary<string, User> _users = new Dictionary<string, User>();

        public class User
        {
            public string Name;
            public int MessageCount;
            public int VisitCount;
            public int KappaCount;
            public int HeartCount;
            public int MessageLength;
            public DateTime FirstSeen = DateTime.Parse("2014-01-01");
            public DateTime LastSeen = DateTime.Parse("2014-01-01");
            public DateTime LastSpoke = DateTime.Parse("2014-01-01");
            public string FirstGame = "None";
            public string LastGame = "None";

            public void Update(Dictionary<string, string> data)
            {
                foreach (var pair in data)
                {
                    switch (pair.Key)
                    {
                        case "messageCount": MessageCount = int.Parse(data["messageCount"].ToString()); break;
                        case "visitCount": VisitCount = int.Parse(data["visitCount"].ToString()); break;
                        case "kappaCount": KappaCount = int.Parse(data["kappaCount"].ToString()); break;
                        case "heartCount": HeartCount = int.Parse(data["heartCount"].ToString()); break;
                        case "messageLength": MessageLength = int.Parse(data["messageLength"].ToString()); break;
                        case "firstSeen": FirstSeen = DateTime.Parse(data["firstSeen"].ToString()); break;
                        case "lastSeen": LastSeen = DateTime.Parse(data["lastSeen"].ToString()); break;
                        case "lastSpoke": LastSpoke = DateTime.Parse(data["lastSpoke"].ToString()); break;
                        case "firstGame": FirstGame = data["firstGame"].ToString(); break;
                        case "lastGame": LastGame = data["lastGame"].ToString(); break;
                    }
                }
            }
        }

        public UserDatabase(Database db, TwitchConnection twitch)
        {
            _db = db;
            _twitch = twitch;
        }

        private void InitializeUser(string username)
        {
            //Get the message to be edited
            var parms = new Dictionary<string, string>();
            parms.Add("@TableName", "users");
            parms.Add("@User", "user");
            parms.Add("@Name", username);

            var results = _db.Query("SELECT * FROM @TableName WHERE @User = '@Name'", parms);
            if (results.Rows.Count == 0)
            {
                _users[username] = NewUser(username);
                return;
            }

            var result = results.Rows[0];

            var user = new User();
            user.Name = result["user"].ToString();

            //Convert datarow into dictionary to easily update a user.
            var data = new Dictionary<string, string>();
            for (int i = 0; i < result.ItemArray.Count(); i++)
                data[result.Table.Columns[i].ToString()] = result.ItemArray[i].ToString();

            _users[username] = user;
            user.Update(data);
        }

        private User NewUser(string username)
        {
            var data = new Dictionary<string, string>();
            data.Add("user", username);
            data.Add("firstSeen", DateTime.Now.ToString("s"));
            var stream = _twitch.GetTwitchStream(Main.chatMain.Substring(1));
            if (stream != null)
                data.Add("firstGame", stream.game);

            var results = _db.Insert("users", data);

            var user = new User();
            user.Name = username;
            user.Update(data);

            return user;
        }

        public User GetUser(string username)
        {
            if (!_users.ContainsKey(username)) InitializeUser(username);
            return _users[username];
        }

        public void Update(string username, Dictionary<string, string> data)
        {
            if (!_users.ContainsKey(username)) InitializeUser(username);

            _users[username].Update(data);

            var parms = new Dictionary<string, string>();
            parms.Add("@TableName", "users");
            parms.Add("@User", "user");
            parms.Add("@UserName", username);

            var results = _db.Update("users", data, "@User = '@UserName'", parms);
        }

        public void RemoveUser(string username)
        {
            if (!_users.ContainsKey(username)) return;

            _users.Remove(username);
            return;
        }

        public Boolean ContainsUser(string username)
        {
            return _users.ContainsKey(username);
        }
    }
}
