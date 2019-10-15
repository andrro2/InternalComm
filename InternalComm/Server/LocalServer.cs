using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InternalComm.Server
{
    class LocalServer
    {
        private Thread _serverThread;
        private IPAddress iPAddress;
        private IPEndPoint endPoint;
        private Socket listener;
        private bool isServerRunning = false;
        private string name;


        public LocalServer(IPAddress iPAddress,IPEndPoint endpoint, string name) 
        {
            this.iPAddress = iPAddress;
            this.endPoint = endpoint;
            _serverThread = new Thread(startServer);
            this.name = name;
        }

        public void Start() 
        {
            isServerRunning = true;
            _serverThread.Start();
        }

        public void Stop()
        {
            isServerRunning = false;
            listener.Close();
        }

        private void startServer()
        {
            listener = new Socket(iPAddress.AddressFamily,
                 SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(endPoint);
                listener.Listen(10);
                while (isServerRunning)
                {
                    Console.WriteLine("Waiting connection ... ");
                    Socket clientSocket = listener.Accept();
                    byte[] bytes = new Byte[1024];
                    string data = null;
                    while (true)
                    {

                        int numByte = clientSocket.Receive(bytes);

                        data += Encoding.ASCII.GetString(bytes,
                                                   0, numByte);

                        if (data.IndexOf("<EOF>") > -1)
                            break;
                    }

                    Console.WriteLine("Text received -> {0} ", data);
                    byte[] message = Encoding.ASCII.GetBytes(name);
                    clientSocket.Send(message);
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
            }


        }


    }
}
