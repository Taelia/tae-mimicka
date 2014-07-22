using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TomeLib.Db;

namespace Mimicka.Updates
{
    public class Version110 : IVersion
    {
        public int Update(Database db)
        {
            // Add column 'uniqueMessageFlag' to the database.
            try
            {
                var parms = new Dictionary<string, string>();
                parms.Add("@TableName", "users");
                parms.Add("@newColumn", "uniqueMessageFlag");
                db.Query("ALTER TABLE @TableName ADD @newColumn NOT NULL DEFAULT ( 'false' )", parms);
            }
            catch
            { } //Ignore the database error when adding a column that was already added.

            //Insert meta row into database.
            try
            {
                var data = new Dictionary<string, string>();
                data.Add("version", "110");

                db.Insert("meta", data);
            }
            catch
            { }

            return 110;
        }
    }
}