using System.Net;
using Sender.Tools;
using Sender.Datas;

public class Program
{
    public static void Main(string[] args)
    {
        bool isExit = false;
        IPAddress address = IPAddress.Parse("192.168.1.100");
        int port = 6000;
        IPEndPoint localEP = new IPEndPoint(address, 2001);
        IPEndPoint remoteEP = new IPEndPoint(address, port);
        udpControl sender = new udpControl(localEP);

        while (!isExit)
        {
            bool isCategory = false;
            int categoryCode = 0;
            while (!isCategory)
            {
                Console.WriteLine("Category (1=information, 2=warning, 3=error, 4=critical):");
                string category = Console.ReadLine();
                switch (category)
                {
                    case "1": //information
                        isCategory = true;
                        categoryCode = 1;
                        break;
                    case "2": //warning
                        isCategory = true;
                        categoryCode = 2;
                        break;
                    case "3": //error
                        isCategory = true;
                        categoryCode = 3;
                        break;
                    case "4": //critical
                        isCategory = true;
                        categoryCode = 4;
                        break;
                    case "exit":
                        isExit = true;
                        isCategory = true;
                        break;
                    default:
                        isCategory = false;
                        break;
                }
            }

            if(!isExit)
            {
                Console.WriteLine("Send Message:");
                string message = Console.ReadLine();
                sender.SendAsync(remoteEP, categoryCode + message);
            }
        }
    }
}