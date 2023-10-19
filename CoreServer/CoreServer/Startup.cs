using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;
using Grpc.Core;
using System.Diagnostics;

using CoreServer.Tools;
using CoreServer.Datas;

namespace CoreServer
{
    internal class Startup : IHostedService
    {
        #region Member
        private IConfigurationRoot configs;
        private IPEndPoint serverEP;
        private DatabaseInfo databaseInfo;
        private mysqlControl database;
        private communicationControl comm;
        private magiconionControl magiconion;
        #endregion

        #region Constructor
        public Startup()
        {
            configs = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json")
                .Build();
        }
        #endregion

        #region Method
        // When the host starts
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Initialization();
            // Database
            database = new mysqlControl(databaseInfo);
            database.Open();
            // Communication
            comm = new communicationControl(serverEP);
            comm.Open();
            comm.propagation += OnReceive;
            // MagicOnion
            magiconion = new magiconionControl(
                url: configs["Server:Host"],
                roomName: "CoreServer",
                Address: configs["Server:Address"]);
            magiconion.Connect();

            ExecuteAsync(cancellationToken);

            return Task.CompletedTask;
        }

        // When the host quits
        public Task StopAsync(CancellationToken cancellationToken)
        {
            magiconion.Close();
            return Task.CompletedTask;
        }

        // When the host is running
        protected async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                if(magiconion.channelStatus() == ConnectivityState.Idle)
                {
                    magiconion.Close();
                    magiconion.Connect();
                }
                
                magiconion.Heartbeat();
                await Task.Delay(1000, cancellationToken);
            }
        }

        // Initialization
        private void Initialization()
        {
            // Set server IP
            try
            {
                serverEP = new IPEndPoint(IPAddress.Parse(configs["Server:Address"]), int.Parse(configs["Server:Port"]));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception($"Error: {DateTime.Now} - Configuration(appsettings.json) is illegal.");
            }
            if(!communicationControl.isServer(serverEP.Address))
            {
                throw new Exception($"Error: {DateTime.Now} - Configuration(appsettings.json) is incorrect.");
            }

            // Set Database
            try
            {
                databaseInfo = new DatabaseInfo()
                {
                    Address = configs["Database:Address"],
                    Port = int.Parse(configs["Database:Port"]),
                    UserID = configs["Database:UserID"],
                    Password = configs["Database:Password"],
                    DatabaseName = configs["Database:DatabaseName"],
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception($"Error: {DateTime.Now} - Configuration(appsettings.json) is illegal.");
            }
        }

        public void OnReceive(byte[] data)
        {
            if(magiconion != null) magiconion.Post(data);
            if (database != null)
            {
                string query = InsertQuery_records(data);
                database.Commit(query);
            }
            Console.WriteLine($"Info: {DateTime.Now} - Receive a message.");
        }

        public static string InsertQuery_records(byte[] data)
        {
            string src = Encoding.UTF8.GetString(data);
            CategoryType categoryType;
            switch (src.Substring(0, 1))
            {
                case "1":
                    categoryType = CategoryType.Information;
                    break;
                case "2":
                    categoryType = CategoryType.Warning;
                    break;
                case "3":
                    categoryType = CategoryType.Error;
                    break;
                case "4":
                    categoryType = CategoryType.Critical;
                    break;
                default:
                    categoryType = CategoryType.Error;
                    break;
            }
            message message = new message(DateTime.Now, src.Substring(1, src.Length - 1), categoryType);
            return $"INSERT INTO message.records(times, text, category) VALUES('{message.times}', '{message.text}', '{message.category}');";
        }
        #endregion
    }
}
