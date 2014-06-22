using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TomeLib.Db;

namespace Mimicka.Updates
{
    public class Version100 : IVersion
    {
        public int Update(Database db)
        {
            // Add column 'characterCount' to the database.
            try
            {
                var parms = new Dictionary<string, string>();
                parms.Add("@TableName", "users");
                parms.Add("@newColumn", "characterCount");
                db.Query("ALTER TABLE @TableName ADD @newColumn NOT NULL DEFAULT ( '0' )", parms);
            }
            catch
            { } //Ignore the database error when adding a column that was already added.

            //Insert meta row into database.
            try
            {
                var data = new Dictionary<string, string>();
                data.Add("version", "100");

                db.Insert("meta", data);
            }
            catch
            { }

            return 100;
        }
    }
}