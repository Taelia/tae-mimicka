using Mimicka.Chatting;
using Mimicka.Updates;
using Mimicka.ViewModels;
using TomeLib.Db;
using TomeLib.Irc;

namespace Mimicka.Models
{
    public class Main
    {
        public MainViewModel View;

        public Chat Chat;

        public static Database Db { get { return Database.GetDatabase("mimicka.db"); } }

        //This is the first model to be created.
        //From here, start the different parts of the screen.
        public Main(MainViewModel view)
        {
            View = view;

            InitializeDatabase();

            Chat = new Chat();
        }

        private void InitializeDatabase()
        {
            // create user table if it doesnt exist
            Db.SafeQuery("CREATE TABLE IF NOT EXISTS users ( userId INTEGER PRIMARY KEY AUTOINCREMENT, user NOT NULL UNIQUE, messageCount NOT NULL DEFAULT ( '0' ), characterCount NOT NULL DEFAULT ( '0' ), visitCount NOT NULL DEFAULT ( '0' ), kappaCount NOT NULL DEFAULT ( '0' ), heartCount NOT NULL DEFAULT ( '0' ), firstSeen DATE NOT NULL DEFAULT ( '2014-01-01' ), lastSeen DATE NOT NULL DEFAULT ( '2014-01-01' ), lastSpoke DATE NOT NULL DEFAULT ( '2014-01-01' ), firstGame NOT NULL DEFAULT ( 'None' ), lastGame NOT NULL DEFAULT ( 'None' ), uniqueMessageFlag NOT NULL DEFAULT ( 'false' ) );");
            // create game table if it doesnt exist
            Db.SafeQuery("CREATE TABLE IF NOT EXISTS games ( gameId INTEGER PRIMARY KEY AUTOINCREMENT, game NOT NULL UNIQUE, firstPlayed DATE NOT NULL DEFAULT ( '2014-01-01' ), lastPlayed DATE NOT NULL DEFAULT ( '2014-01-01' ), daysPlayed NOT NULL DEFAULT ( '0' ), visitCount NOT NULL DEFAULT ( '0' ), messageCount NOT NULL DEFAULT ( '0' ), uniqueMessageCount NOT NULL DEFAULT ( '0' ) );");
            // create version table
            Db.SafeQuery("CREATE TABLE IF NOT EXISTS meta ( version PRIMARY KEY NOT NULL DEFAULT ( '0' ));");

            new Version100().Update(Db);
            new Version110().Update(Db);
        }
    }
}
