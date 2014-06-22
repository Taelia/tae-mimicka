using System;
using System.Collections.Generic;
using System.Globalization;
using Meebey.SmartIrc4net;
using Mimicka.Models;
using TomeLib.Db;
using TomeLib.Irc;
using TomeLib.Twitch;

namespace Mimicka.Chatting
{
    public class Chat : ChatBase
    {
        private readonly Statistics _statistics;
        private Database _db { get { return Main.Db; } }

        private readonly UserDatabase _userDatabase;

        public const string MainChannel = "#taelia_";

        public Chat()
            : base(new Irc("Tomestone", "oauth:npafwpg44j0a5iullxo2dt385n5jeco", new[] { MainChannel }))
        {
            var twitch = new TwitchConnection();

            _userDatabase = new UserDatabase(_db, twitch);
            _statistics = new Statistics(_userDatabase, twitch);
        }

        protected override void OnMessage(Channel channel, IrcUser from, string message)
        {
            //Due to Twitch architecture, a user can send a message before a JOIN.
            if (!_userDatabase.ContainsUser(from.Nick))
                OnJoin(channel, from.Nick);

            _statistics.UpdateOnMessage(from.Nick, message);
        }

        protected override void OnAction(Channel channel, IrcUser from, string message)
        {
            //Treat the same as messages.
            OnMessage(channel, from, message);
        }

        protected override void OnJoin(Channel channel, string from)
        {
            //Ignore if user already sent a message prior to the JOIN event.
            if (_userDatabase.ContainsUser(from)) return;

            SendLastVisited(from);
            _statistics.UpdateOnJoin(from);
        }

        //Part and Quit don't work properly on Twitch
        protected override void OnPart(Channel channel, string from)
        {
            _userDatabase.RemoveUser(from);
        }
        protected override void OnQuit(Channel channel, string from)
        {
            //Treat the same as parts.
            OnPart(channel, from);
        }

        private void SendLastVisited(string from)
        {
            var user = _userDatabase.GetUser(from);

            //dont need to do anything if the difference is less than 5mins
            if ((DateTime.Now - user.LastSeen) < TimeSpan.FromMinutes(5)) return;

            var seenText = (user.LastSeen == DateTime.Parse("2014-01-01")) ? "Unknown" : DaysAgoText(((DateTime.Now + TimeSpan.FromHours(2)).Date - (user.LastSeen + TimeSpan.FromHours(2)).Date).Days);
            var spokeText = (user.LastSpoke == DateTime.Parse("2014-01-01")) ? "Unknown" : DaysAgoText(((DateTime.Now + TimeSpan.FromHours(2)).Date - (user.LastSpoke + TimeSpan.FromHours(2)).Date).Days);
            var duringGame = user.LastGame;

            if (duringGame == "None" || duringGame == "") SendMessage("#taelia_welcome", "New viewer: " + from);
            else SendMessage("#taelia_welcome", from + "| Seen: " + seenText + " during [" + duringGame + "] | Spoke: " + spokeText);
        }

        //Utility
        private string DaysAgoText(int days)
        {
            var daysAgoText = "";
            if (days == 0) daysAgoText = "today";
            else if (days == 1) daysAgoText = "yesterday";
            else daysAgoText = days + " days ago";

            return daysAgoText;
        }
    }
}
