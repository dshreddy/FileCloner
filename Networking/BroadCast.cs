using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Networking
{
    public class BroadCast
    {
        public static void Main()
        {
            BroadCastMessage("File-File");
        }
        public static void BroadCastMessage(string fileDescription)
        {
            byte[] bytes = new byte[1024];
            int portNo = 11000;
            IPHostEntry host = Dns.GetHostEntry("localhost"); //replace these 2 lines with server's IP Address and port no: later on
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, portNo);
            Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sender.Connect(remoteEP);
                Console.WriteLine(String.Format("Socket connected to {0}", sender.RemoteEndPoint.ToString())); //replace this with a log write
                byte[] msg = Encoding.ASCII.GetBytes(String.Format("File Requested {0}\n", fileDescription));

                int bytesSent = sender.Send(msg);

                int bytesRec = sender.Receive(bytes); //do accordingly with the received message
                //Console.WriteLine("Received = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch
            {
                Console.WriteLine("Server is down or refused to accept connection!");
            }

        }
    }
}