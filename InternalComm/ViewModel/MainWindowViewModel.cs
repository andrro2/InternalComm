using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Threading;
using System.Threading;
using InternalComm.Server;
using InternalComm.Client;

namespace InternalComm.ViewModel
{
    class MainWindowViewModel
    {
        private ObservableCollection<string> connectedUsers = new ObservableCollection<string>();
        private List<IPAddress> ipAdresses = new List<IPAddress>();
        private Dictionary<string, ChatClient> avalibleClients = new Dictionary<string, ChatClient>();
        private List<ChatClient> possibleClients = new List<ChatClient>();
        private string ipBase = "192.168.100.";
        private IPAddress localIp = null;
        private IPEndPoint endpoint = null;
        private LocalServer server;
        private string name = "Rozner";

        public ObservableCollection<string> ConnectedUsers { get => connectedUsers; set => connectedUsers = value; }

        public MainWindowViewModel() 
        {
            initLocalIp();
            initIpBase();
            server = new LocalServer(localIp, endpoint, name);
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Tick += new EventHandler(addToCollection);
            timer.Start();
            server.Start();

        }

        public void ShutDown() 
        {
            server.Stop();
        }

        private void pingCompleted(object sender, PingCompletedEventArgs e) 
        {
            string ip = (string)e.UserState;
            if (e.Reply != null && e.Reply.Status == IPStatus.Success) 
            {
                if (!ipAdresses.Contains(IPAddress.Parse(ip)) && !IPAddress.Parse(ip).Equals(localIp))
                {
                    ipAdresses.Add(IPAddress.Parse(ip));
                    return;
                }
                return;

            }
            else if(e.Reply.Status == IPStatus.TimedOut && ipAdresses.Contains(IPAddress.Parse(ip))) {
                Console.WriteLine("removed");
                ipAdresses.Remove(IPAddress.Parse(ip)); }

        }

        public void addToCollection(Object sender, EventArgs e)
        {
            Console.WriteLine("Avalible clients: " + avalibleClients.Count());
            if (avalibleClients.Count() > 0)
            {
                foreach (KeyValuePair<string, ChatClient> keyValuePair in avalibleClients)
                {
                    connectedUsers.Add(keyValuePair.Key);
                }
            }
            if (possibleClients.Count() > 0)
            {
                foreach (ChatClient chatClient in possibleClients)
                {
                    chatClient.startPingingClients();
                }
            }

                for (int i = 1; i < 255; i++)
                {
                    string ip = ipBase + i.ToString();
                    Ping ping = new Ping();
                    ping.PingCompleted += new PingCompletedEventHandler(pingCompleted);
                    ping.SendAsync(ip, 100, ip);
                }

                if (ipAdresses.Count() > 0)
                {
                    Console.WriteLine("ipAdresses: " + ipAdresses.Count());
                    foreach (IPAddress iPAddress in ipAdresses)
                    {
                        IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, 8000);
                        ChatClient client = new ChatClient(localIp, iPEndPoint, ref avalibleClients);
                        if (possibleClients.Count() > 0)
                        {
                            int counter = 0;
                            foreach (ChatClient chatClient in possibleClients)
                            {
                                if (chatClient.EndPoint.Equals(iPEndPoint)) { counter++; }
                            }
                            if (counter == 0) { possibleClients.Add(client); }

                        }
                        else { possibleClients.Add(client); }
                    }
                }

                if (possibleClients.Count > 0)
                {
                    List<ChatClient> templist = possibleClients.ToList();
                    foreach (ChatClient chatClient in possibleClients)
                    {
                        if (!ipAdresses.Contains(chatClient.EndPoint.Address) && !chatClient.IsClientAvalible)
                        {
                            templist.Remove(chatClient);
                        }
                    }
                    possibleClients = templist.ToList();
                }
                Console.WriteLine(possibleClients.Count());

            }


        private void initLocalIp() {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ipAddress in host.AddressList)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIp = ipAddress;
                }
            }
            endpoint = new IPEndPoint(localIp, 8000);
        }

        private void initIpBase() {
            string stringIp = localIp.ToString();
            string[] ipArray = stringIp.Split('.');
            string[] ipBaseStringArray = new string[ipArray.Length-1];
            System.Array.Copy(ipArray, 0, ipBaseStringArray, 0, 3);
            ipBase = string.Join(".", ipBaseStringArray) + ".";
            Console.WriteLine(ipBase);
        }
    }
}
