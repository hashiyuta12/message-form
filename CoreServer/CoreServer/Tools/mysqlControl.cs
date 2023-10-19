using System.Net;
using System.Net.NetworkInformation;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace CoreServer.Tools
{
    public class DatabaseInfo
    {
        public string Address { get; set; }
        public int Port { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
    }

    internal class mysqlControl
    {
        #region Member
        private DatabaseInfo info;
        private string connectionStrings;
        private MySqlConnection connection;
        #endregion

        #region Constructor
        public mysqlControl(DatabaseInfo info)
        {
            this.info = info;
            this.connectionStrings = 
                $"server={info.Address};" +
                $"port={info.Port};" +
                $"uid={info.UserID};" +
                $"pwd={info.Password};" +
                $"database={info.DatabaseName};";
        }
        #endregion

        #region Method
        public void Connect()
        {
            if(!polling(IPAddress.Parse(info.Address)))
            {
                Thread.Sleep(300);
                if (!polling(IPAddress.Parse(info.Address))) throw new Exception($"Error: {DateTime.Now} - Unable to connect to database.");
            }
            connection = new MySqlConnection(connectionStrings);
        }

        public void Open()
        {
            Connect();
            try
            {
                connection.Open();
                Debug.WriteLine($"mysqlControl.Open: {DateTime.Now} - OK to connect to database.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception($"Error: {DateTime.Now} - Unable to connect to database.");
            }
        }

        public void Dispose()
        {
            if(connection != null)
            {
                connection.Close();
                Debug.WriteLine($"mysqlControl.Dispose: {DateTime.Now} - OK to disconnect to database.");
            }
        }

        public void Commit(string query)
        {
            if(connection == null) return;
            MySqlTransaction transaction = connection.BeginTransaction();
            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.CommandTimeout = 36000;
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Debug.WriteLine(ex.Message);
                throw new Exception($"Error: {DateTime.Now} - Could not perform query.");
            }
        }

        public static bool polling(IPAddress address)
        {
            Ping ping = new Ping();
            PingReply reply = ping.Send(address);

            bool ret = false;
            if (reply.Status == IPStatus.Success) ret = true;
            return ret;
        }
        #endregion
    }
}
