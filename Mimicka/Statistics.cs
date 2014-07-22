using System;
using Mimicka.Chatting;
using TomeLib.Twitch;

namespace Mimicka
{
    public class Statistics
    {
        private readonly UserDatabase _userDatabase;
        private readonly GameDatabase _gameDatabase;
        private readonly TwitchConnection _twitch;

        public Statistics(UserDatabase userDatabase, GameDatabase gameDatabase, TwitchConnection twitch)
        {
            _userDatabase = userDatabase;
            _gameDatabase = gameDatabase;
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
            {
                //update game stats
                var game = _gameDatabase.GetGame(stream.game);
                
                game.MessageCount++;
                
                if ((DateTime.Now + TimeSpan.FromHours(2)).Date != (game.LastPlayed + TimeSpan.FromHours(2)).Date)
                {
                    game.DaysPlayed++;
                    game.LastPlayed = DateTime.Now;
                }

                // case where game played changes while person is still in channel (change without join event)
                if (user.LastGame.CompareTo(stream.game) != 0)
                {
                    game.UniqueMessageCount++;
                    game.VisitCount++;
                    user.UniqueMessageFlag = false;
                }

                // case where user speaks for the first time after join event in a new game
                if (user.UniqueMessageFlag )  
                {
                    game.UniqueMessageCount++;
                    user.UniqueMessageFlag = false;
                }

                _gameDatabase.Update(stream.game, game);

                user.LastGame = stream.game;
            }
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
            user.FirstMessageFlag = true;

            //Gets the game currently being streamed.
            var stream = _twitch.GetTwitchStream(Chat.MainChannel.Substring(1));
            if (stream != null)
            {
                var game = _gameDatabase.GetGame(stream.game);

                if ((DateTime.Now + TimeSpan.FromHours(2)).Date != (game.LastPlayed + TimeSpan.FromHours(2)).Date) 
                {
                    game.DaysPlayed++;
                    game.LastPlayed = DateTime.Now;
                }
                    
                if (user.LastGame.CompareTo(stream.game) != 0)
                {
                    //update game stats
                    game.VisitCount++;

                    // set boolean to trigger unique message count
                    user.UniqueMessageFlag = true;
                }
                
                _gameDatabase.Update(stream.game, game);

                user.LastGame = stream.game;
            }
            _userDatabase.Update(from, user);
        }
    }
}