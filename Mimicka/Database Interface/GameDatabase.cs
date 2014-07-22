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
    public class GameDatabase
    {
        private readonly Database _db;

        public GameDatabase(Database db, TwitchConnection twitch)
        {
            _db = db;
        }

        public Game GetGame(string gameName)
        {
            //Get the message to be edited
            var parms = new Dictionary<string, string>();
            parms.Add("@TableName", "games");
            parms.Add("@Game", "game");
            parms.Add("@Name", gameName);

            var results = _db.Query("SELECT * FROM @TableName WHERE @Game = '@Name'", parms);

            //If game isn't found, create a new game
            if (results.Rows.Count == 0)
                return NewGame(gameName);
            else //Otherwise, create a game and fill it with data from the database.
            {
                var game = new Game(gameName, DataRowToDictionary(results.Rows[0]));
                return game;
            }
        }

        private Game NewGame(string gameName)
        {
            var game = new Game(gameName);

            game.FirstPlayed = DateTime.Now;

            var data = game.ToDictionary();
            data.Add("game", game.Name);
            _db.Insert("games", data);

            return game;
        }

        public void Update(string gameName, Game game)
        {
            var parms = new Dictionary<string, string>();
            parms.Add("@TableName", "games");
            parms.Add("@Game", "game");
            parms.Add("@GameName", gameName);

            _db.Update("games", game.ToDictionary(), "@Game = '@GameName'", parms);
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
