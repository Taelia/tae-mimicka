using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tomestone.Chatting;
using System.Windows;
using TomeLib.Irc;
using TomeLib.Db;
using Meebey.SmartIrc4net;
using TomeLib.Twitch;
using System.Windows.Threading;
using Tomestone.Models;

namespace Tomestone.Chatting
{
    public partial class TomeChat : IChat
    {
        public History SentMessages { get; set; }
        public History ReceivedMessages { get; set; }

        private TwitchConnection _twitch;
        private Database _db { get { return Main.Db; } }

        public UserDatabase Users;

        public TomeChat()
        {
            SentMessages = new History();
            ReceivedMessages = new History();

            _twitch = new TwitchConnection();

            Users = new UserDatabase(_db, _twitch);
        }

        public void OnMessage(Channel channel, IrcUser from, string message)
        {
            var obj = new MessageObject(from, message);
            if (from.Nick == "tomestone") return;

            UpdateOnMessage(from.Nick, message);
        }

        public void OnAction(Channel channel, IrcUser from, string message)
        {
            //Treat the same as messages.
            OnMessage(channel, from, message);
        }

        public void OnJoin(Channel channel, string from)
        {
            if (from == "tomestone") return;

            SendLastVisited(from);
            UpdateOnJoin(from);
        }

        //Part and Quit don't work properly on Twitch
        public void OnPart(Channel channel, string from)
        {
        }
        public void OnQuit(Channel channel, string from)
        {
            //Treat the same as parts.
            OnPart(channel, from);
        }

        public void UpdateOnMessage(string from, string message)
        {
            var user = Users.GetUser(from);

            var data = new Dictionary<string, string>();

            if (message.Contains("<3"))
                data.Add("heartCount", (user.HeartCount + 1).ToString());
            if (message.Contains("Kappa"))
                data.Add("kappaCount", (user.KappaCount + 1).ToString());

            data.Add("messageCount", (user.MessageCount + 1).ToString());
            data.Add("lastSpoke", DateTime.Now.ToString("yyyy-MM-dd"));

            var stream = _twitch.GetTwitchStream(Main.chatMain.Substring(1));
            if (stream != null)
                data.Add("lastGame", stream.game);

            Users.Update(from, data);
        }

        private void UpdateOnJoin(string from)
        {
            var user = Users.GetUser(from);

            var data = new Dictionary<string, string>();

            data.Add("lastSeen", DateTime.Now.ToString("yyyy-MM-dd"));
            var stream = _twitch.GetTwitchStream(Main.chatMain.Substring(1));
            if (stream != null)
                data.Add("lastGame", stream.game);

            if (!(DateTime.Now.Day == user.LastSeen.Day && DateTime.Now.Month == user.LastSeen.Month && DateTime.Now.Year == user.LastSeen.Year))
                data.Add("visitCount", (user.VisitCount + 1).ToString());

            Users.Update(from, data);
        }

        public void SendLastVisited(string from)
        {
            var user = Users.GetUser(from);

            var seenText = (user.LastSeen == DateTime.Parse("2014-01-01")) ? "Unknown" : DaysAgoText((DateTime.Now - user.LastSeen + TimeSpan.FromHours(4)).Days);
            var spokeText = (user.LastSpoke == DateTime.Parse("2014-01-01")) ? "Unknown" : DaysAgoText((DateTime.Now - user.LastSpoke + TimeSpan.FromHours(4)).Days);
            var duringGame = user.LastGame;

            if (duringGame == "None" || duringGame == "") SendMessage("#taelia_welcome", "New viewer: " + from);
            else SendMessage("#taelia_welcome", from + "| Seen: " + seenText + " during [" + duringGame + "] | Spoke: " + spokeText);
        }

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
