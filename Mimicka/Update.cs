using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TomeLib.Db;

namespace Tomestone
{
    public class Update
    {
        private string version = "100";
        private Database Db = null;

        public Update(Database Db)
        {
            this.Db = Db;
        }
        
        public Boolean UpdateDatabase()
        {
            Boolean result = false;
            string databaseVersion;

            var parms = new Dictionary<string, string>();
            parms.Add("@TableName", "meta");

            //query database for its current version
            var queryResult = Db.Query("SELECT * FROM @TableName", parms);

            if (queryResult.Rows.Count != 0) databaseVersion = queryResult.Rows[0]["version"].ToString();
            else databaseVersion = "0";
            
            
            // compare versions and update case by case

            switch (databaseVersion)
            {
                case "0": // previously uninitialized version
                    
                    // fix the database
                    parms = new Dictionary<string, string>();
                    parms.Add("@TableName", "users");
                    parms.Add("@newColumn", "characterCount");
                    Db.Query("ALTER TABLE @TableName ADD @newColumn NOT NULL DEFAULT ( '0' ) ;", parms); // TODO: handle a fail case where you try to add a column that already exists

                    // add new version entry to meta
                    var data = new Dictionary<string, string>();
                    data.Add("version", "100");
                    var results = Db.Insert("meta", data);

                    break;
                default:
                    break;
            }

            return result; 
        }

        private void WriteToDatabase(Dictionary<string, string> data, string oldVersion)
        {

            var parms = new Dictionary<string, string>();
            parms.Add("@TableName", "users");
            parms.Add("@Version", "version");
            parms.Add("@VersionNumber", oldVersion);

            var results = Db.Update("meta", data, "@Version = '@VersionNumber'", parms);
        }
    }
}
