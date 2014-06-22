using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using TomeLib.Db;

namespace Mimicka.Updates
{
    public class Update
    {
        private int _updateVersion;
        private readonly Database _db;

        public Update(Database db)
        {
            _db = db;
        }

        private bool PerformUpdate()
        {
            switch (_updateVersion)
            {
                case 0:
                    _updateVersion = new Version100().Update(_db);
                    return true;
                
                //Add calls to new Versions here.

                default:
                    return false;
            }
        }

        public void UpdateDatabase()
        {
            var parms = new Dictionary<string, string>();
            parms.Add("@TableName", "meta");

            var queryResult = _db.Query("SELECT * FROM @TableName", parms);

            //If the database contains a version number, set it. Otherwise, leave at 0.
            if (queryResult.Rows.Count != 0)
                _updateVersion = int.Parse(queryResult.Rows[0]["version"].ToString());

            //Loop PerformUpdate until our version number equals the latest update.
            while (PerformUpdate()) { }

            // add new version entry to meta
            var data = new Dictionary<string, string>();
            data.Add("version", _updateVersion.ToString(CultureInfo.InvariantCulture));
            _db.Update("meta", data);
        }
    }
}
