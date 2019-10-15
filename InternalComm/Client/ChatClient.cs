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
        private Dictionary<string, IPAddress> avalibleClients;

        public ChatClient(IPAddress iPAddress, IPEndPoint endPoint, ref Dictionary<string, IPAddress> avalibleClients)
        {
            this.avalibleClients = avalibleClients;
            this.iPAddress = iPAddress;
            this.endPoint = endPoint;
            _clientThread = new Thread(pingAvalibleUsers);
        }

        public void startPingingClients() 
        {
            Console.WriteLine(_clientThread.ThreadState);
            if (!_clientThread.IsAlive) 
            {
                Console.WriteLine("client thread started");
                _clientThread.Start();
            }
        }



        public void pingAvalibleUsers()
        {
            Socket client = new Socket(iPAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client.Connect(endPoint);
                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Encode the data string into a byte array.  
                byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

                // Send the data through the socket.  
                int bytesSent = client.Send(msg);

                // Receive the response from the remote device.  
                int bytesRec = client.Receive(bytes);
                Console.WriteLine("Echoed test = {0}",
                    Encoding.ASCII.GetString(bytes, 0, bytesRec));

                // Release the socket.  
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch (ArgumentNullException ane)
            {
                
            }
            catch (SocketException se)
            {
                
            }
            catch (Exception e)
            {
                
            }
            Console.WriteLine("client thread finished");
        }
    }
}
