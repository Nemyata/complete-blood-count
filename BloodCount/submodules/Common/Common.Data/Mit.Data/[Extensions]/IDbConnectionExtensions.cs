using System.Data;

namespace Common.Data.SqlClient
{
    public static class IDbConnectionExtensions
    {
        public static void OpenSafe(this IDbConnection? connection)
        {
            if (connection != null && connection.State != ConnectionState.Open)
                connection.Open();
        }
    }
}
