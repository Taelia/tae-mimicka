using System;
using Mimicka.Chatting;
using TomeLib.Twitch;

namespace Mimicka
{
    public class Statistics
    {
        private readonly UserDatabase _userDatabase;
        private readonly TwitchConnection _twitch;

        public Statistics(UserDatabase userDatabase, TwitchConnection twitch)
        {
            _userDatabase = userDatabase;
            _twitch = twitch;
        }

        public void UpdateOnMessage(string from, string message)
        {
            var user = _userDatabase.GetUser(from);

            if (message.Contains("<3"))
                user.HeartCount++;
            if (message.Contains("Kappa"))
                user.KappaCount++;

            user.MessageCount++;
            user.LastSpoke = DateTime.Now;

            user.CharacterCount += message.Length;

            //Gets the game currently being streamed.
            var stream = _twitch.GetTwitchStream(Chat.MainChannel.Substring(1));
            if (stream != null)
                user.LastGame = stream.game;

            // write to the database
            _userDatabase.Update(from, user);
        }

        public void UpdateOnJoin(string from)
        {
            // update user visit data when they join
            var user = _userDatabase.GetUser(from);

            if ((DateTime.Now + TimeSpan.FromHours(2)).Date != (user.LastSeen + TimeSpan.FromHours(2)).Date)
                user.VisitCount++;

            user.LastSeen = DateTime.Now;

            //Gets the game currently being streamed.
            var stream = _twitch.GetTwitchStream(Chat.MainChannel.Substring(1));
            if (stream != null)
                user.LastGame = stream.game;

            

            _userDatabase.Update(from, user);
        }
    }
}