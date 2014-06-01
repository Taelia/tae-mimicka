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
            // possible cases:
            // 1. user does not exist in database (was in the channel when program was started)
            // 2. user exists in database

            // get the information for the user
            // - will initialize the user if not found
            var user = Users.GetUser(from);

            var data = new Dictionary<string, string>();

            // update user information on the database based on contents of message
            if (message.Contains("<3"))
                data.Add("heartCount", (user.HeartCount + 1).ToString());
            if (message.Contains("Kappa"))
                data.Add("kappaCount", (user.KappaCount + 1).ToString());

            data.Add("messageCount", (user.MessageCount + 1).ToString());
            data.Add("lastSpoke", DateTime.Now.ToString("s"));

            // records what game is being streamed
            var stream = _twitch.GetTwitchStream(Main.chatMain.Substring(1));
            if (stream != null)
                data.Add("lastGame", stream.game);

            // handle the case where a user who was previously not in the database and was in the 
            // chat channel at program startup did not have the lastSeen field updated,
            // resulting in the next join displaying "Seen: 735376 days ago"
            if (user.LastSeen == DateTime.Parse("2014-01-01")) data.Add("lastSeen", DateTime.Now.ToString("s"));

            // write to the database
            Users.Update(from, data);
        }

        private void UpdateOnJoin(string from)
        {
            // update user visit data when they join
            var user = Users.GetUser(from);

            var data = new Dictionary<string, string>();

            data.Add("lastSeen", DateTime.Now.ToString("s"));
            var stream = _twitch.GetTwitchStream(Main.chatMain.Substring(1));
            if (stream != null)
                data.Add("lastGame", stream.game);

            if ( (DateTime.Now + TimeSpan.FromHours(2)).Date != (user.LastSeen + TimeSpan.FromHours(2)).Date )
                data.Add("visitCount", (user.VisitCount + 1).ToString());

            Users.Update(from, data);
        }

        public void SendLastVisited(string from)
        {
            var user = Users.GetUser(from);
            
            //dont need to do anything if the difference is less than 5mins
            //if ( (DateTime.Now - user.LastSeen) < TimeSpan.FromMinutes(5)) return;
            //Console.WriteLine(user.LastSeen);
            //Console.WriteLine(user.LastSpoke);
            //Console.WriteLine(user.LastGame);




            var seenText = (user.LastSeen == DateTime.Parse("2014-01-01")) ? "Unknown" : DaysAgoText(((DateTime.Now + TimeSpan.FromHours(2)).Date - (user.LastSeen + TimeSpan.FromHours(2)).Date).Days);
            var spokeText = (user.LastSpoke == DateTime.Parse("2014-01-01")) ? "Unknown" : DaysAgoText(((DateTime.Now + TimeSpan.FromHours(2)).Date - (user.LastSpoke + TimeSpan.FromHours(2)).Date).Days);
            var duringGame = user.LastGame;


            //Console.WriteLine(seenText);
            //Console.WriteLine(spokeText);


            //System.IO.StreamWriter file = new System.IO.StreamWriter("test.txt", true);

            if (duringGame == "None" || duringGame == "") SendMessage("#taelia_welcome", "New viewer: " + from + "(test#2)");
            else SendMessage("#taelia_welcome", from + "| Seen: " + seenText + " during [" + duringGame + "] | Spoke: " + spokeText + "(test#2)");

            //file.Close();
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
