using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InternalComm.Client
{
    class ChatClient
    {
        private Thread _clientThread;
        private IPEndPoint endPoint;
        private IPAddress iPAddress;
        byte[] bytes = new byte[1024];
        private Dictionary<string, ChatClient> avalibleClients;
        private bool isClientAvalible = false;
        private string clientName = "";

        public IPEndPoint EndPoint { get => endPoint; set => endPoint = value; }
        public bool IsClientAvalible { get => isClientAvalible; set => isClientAvalible = value; }

        public ChatClient(IPAddress iPAddress, IPEndPoint endPoint, ref Dictionary<string, ChatClient> avalibleClients)
        {
            this.avalibleClients = avalibleClients;
            this.iPAddress = iPAddress;
            this.endPoint = endPoint;
            _clientThread = new Thread(pingAvalibleUsers);
        }

        public void startPingingClients() 
        {
            if (_clientThread.ThreadState == ThreadState.Unstarted) 
            {
                _clientThread.Start();
            }
            if (_clientThread.ThreadState == ThreadState.Stopped)
            {
                _clientThread = new Thread(pingAvalibleUsers);
                _clientThread.Start();
            }
        }



        public void pingAvalibleUsers()
        {
            Console.WriteLine(clientName);
            Socket client = new Socket(iPAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client.Connect(endPoint);
                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());
                isClientAvalible = true;

                // Encode the data string into a byte array.  
                byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

                // Send the data through the socket.  
                int bytesSent = client.Send(msg);

                // Receive the response from the remote device.  
                int bytesRec = client.Receive(bytes);
                Console.WriteLine("Echoed test = {0}",
                    Encoding.ASCII.GetString(bytes, 0, bytesRec));

                string TempName = Encoding.ASCII.GetString(bytes, 0, bytesRec).ToString();
                if(TempName != null) {
                    clientName = TempName;
                    avalibleClients.Add(clientName, this);
                }
                

                // Release the socket.  
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch (ArgumentNullException ane)
            {
                
            }
            catch (SocketException se)
            {
                isClientAvalible = false;
                Console.WriteLine(clientName);
                if (avalibleClients.Count > 0 && avalibleClients.ContainsKey(clientName))
                    avalibleClients.Remove(clientName);
            }
            catch (Exception e)
            {
                
            }
        }
    }
}
