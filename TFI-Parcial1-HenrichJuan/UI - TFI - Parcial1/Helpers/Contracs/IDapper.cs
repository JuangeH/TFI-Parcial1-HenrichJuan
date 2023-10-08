using System.Data.Common;
using System.Data;
using Dapper;

namespace UI___TFI___Parcial1.Helpers.Contracs
{
    public interface IDapper
    {
        DbConnection GetConnection();
        T Get<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text);
        List<T> GetAll<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text);
        int Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text);
        T Insert<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text);
        T Update<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text);
    }
}
