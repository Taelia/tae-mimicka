using Tomestone.Chatting;
using Tomestone.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TomeLib.Db;
using TomeLib.Irc;

namespace Tomestone.Models
{
    public class Main
    {
        public MainViewModel View;

        public Irc Chat;

        public static Database Db { get { return Database.GetDatabase("mimicka.db"); } }

        private string botName = "Tomestone";
        private string botOauth = "oauth:e1dmmpdsq34uppmhxdbqmhbesja6osr";
        
        public static string chatMain = "#taelia_";
        public static string chatMods = "#taelia_welcome";

        //This is the first model to be created.
        //From here, start the different parts of the screen.
        public Main(MainViewModel view)
        {
            View = view;

            InitializeDatabase();

            Chat = new Irc(botName, botOauth, new string[] {chatMain, chatMods}, new TomeChat());
        }

        private void InitializeDatabase()
        {
            Main.Db.SafeQuery("CREATE TABLE IF NOT EXISTS users ( userId INTEGER PRIMARY KEY AUTOINCREMENT, user NOT NULL UNIQUE, messageCount NOT NULL DEFAULT ( '0' ), visitCount NOT NULL DEFAULT ( '0' ), kappaCount NOT NULL DEFAULT ( '0' ), heartCount NOT NULL DEFAULT ( '0' ), messageLength NOT NULL DEFAULT ( '30' ), firstSeen DATE NOT NULL DEFAULT ( '2014-01-01' ), lastSeen DATE NOT NULL DEFAULT ( '2014-01-01' ), lastSpoke DATE NOT NULL DEFAULT ( '2014-01-01' ), firstGame NOT NULL DEFAULT ( 'None' ), lastGame NOT NULL DEFAULT ( 'None' ) );");
        }
    }
}
