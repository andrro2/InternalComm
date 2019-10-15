using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace InternalComm.Client
{
    class ChatClient
    {
        private IPEndPoint endPoint;
        private IPAddress iPAddress;
        byte[] bytes = new byte[1024];

        public ChatClient(IPAddress iPAddress, IPEndPoint endPoint)
        {
            this.iPAddress = iPAddress;
            this.endPoint = endPoint;
        }

        public void pingAvalibleUsers(ref Dictionary<string, IPAddress> avalibleClients)
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
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }
    }
}
