using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Mimicka.Chatting;
using Mimicka.Models;
using TomeLib.Db;
using TomeLib.Twitch;

namespace Mimicka
{
    public class UserDatabase
    {
        private readonly Database _db;
        private readonly TwitchConnection _twitch;

        private readonly Dictionary<string, User> _users = new Dictionary<string, User>();

        public UserDatabase(Database db, TwitchConnection twitch)
        {
            _db = db;
            _twitch = twitch;
        }

        public User GetUser(string username)
        {
            if (!_users.ContainsKey(username)) InitializeUser(username);
            return _users[username];
        }

        private void InitializeUser(string username)
        {
            //Get the message to be edited
            var parms = new Dictionary<string, string>();
            parms.Add("@TableName", "users");
            parms.Add("@User", "user");
            parms.Add("@Name", username);

            var results = _db.Query("SELECT * FROM @TableName WHERE @User = '@Name'", parms);

            //If user isn't found, create a new user
            if (results.Rows.Count == 0)
                _users[username] = NewUser(username);
            else //Otherwise, create a user and fill it with data from the database.
            {
                var user = new User(username, DataRowToDictionary(results.Rows[0]));
                _users[username] = user;
            }
        }

        private User NewUser(string username)
        {
            var user = new User(username);
            user.FirstSeen = DateTime.Now;
            var stream = _twitch.GetTwitchStream(Chat.MainChannel.Substring(1));
            if (stream != null) user.FirstGame = stream.game;

            _db.Insert("users", user.ToDictionary());

            return user;
        }

        public void Update(string username, User user)
        {
            var parms = new Dictionary<string, string>();
            parms.Add("@TableName", "users");
            parms.Add("@User", "user");
            parms.Add("@UserName", username);

            _db.Update("users", user.ToDictionary(), "@User = '@UserName'", parms);
        }

        public void RemoveUser(string username)
        {
            if (_users.ContainsKey(username))
                _users.Remove(username);
        }

        public bool ContainsUser(string username)
        {
            return _users.ContainsKey(username);
        }

        //Utility
        private Dictionary<string, string> DataRowToDictionary(DataRow result)
        {
            var data = new Dictionary<string, string>();
            for (int i = 0; i < result.ItemArray.Count(); i++)
                data[result.Table.Columns[i].ToString()] = result.ItemArray[i].ToString();
            return data;
        }
    }
}
