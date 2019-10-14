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

namespace InternalComm.ViewModel
{
    class MainWindowViewModel
    {
        private ObservableCollection<IPAddress> az = new ObservableCollection<IPAddress>();
        private List<IPAddress> ipAdresses = new List<IPAddress>();
        private string ipBase = "192.168.100.";
        private IPAddress localIp = null;

        public ObservableCollection<IPAddress> Az { get => az; set => az = value; }

        public MainWindowViewModel() 
        {
            initLocalIp();
            initIpBase();
            


        }

        private void pingCompleted(object sender, PingCompletedEventArgs e) 
        {
            string ip = (string)e.UserState;
            if (e.Reply != null && e.Reply.Status == IPStatus.Success) 
            {
                ipAdresses.Add(IPAddress.Parse(ip));
            }
            /*if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                    string name;
                    try
                    {
                        IPHostEntry hostEntry = Dns.GetHostEntry(ip);
                        name = hostEntry.HostName;
                    }
                    catch (SocketException ex)
                    {
                        name = "?";
                    }
                    Console.WriteLine("{0} ({1}) is up: ({2} ms)", ip, name, e.Reply.RoundtripTime);
            }
            
            else if (e.Reply == null)
            {
                Console.WriteLine("Pinging {0} failed. (Null Reply object?)", ip);
            }*/
        }

        public void addToCollection()
        {
            for (int i = 1; i < 255; i++)
            {
                string ip = ipBase + i.ToString();
                Ping ping = new Ping();
                ping.PingCompleted += new PingCompletedEventHandler(pingCompleted);
                ping.SendAsync(ip, 100, ip);
            }
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
