using System.Security.Cryptography.X509Certificates;
using TomeLib.Db;

namespace Mimicka.Updates
{
    public interface IVersion
    {
        int Update(Database db);
    }
}