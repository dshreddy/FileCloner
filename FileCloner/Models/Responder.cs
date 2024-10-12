using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileCloner.Models
{
    public class Responder : IResponder
    {
        static int listeningPortNumber = 8080;
        public void ListenForRequests(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                throw new ArgumentNullException(nameof(ipAddress));

            var listener = new TcpListener(IPAddress.Parse(ipAddress), listeningPortNumber);
            listener.Start();
            Console.WriteLine($"Listening for requests on {ipAddress}:{listeningPortNumber}...");

            while (true)
            {
                try
                {
                    // Wait for incoming connection
                    var client = listener.AcceptTcpClient();
                    Console.WriteLine("Received a connection request.");
                    // You can read data from the client here (not included for now)
                    client.Close();
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Error while listening for requests: {ex.Message}");
                    break;
                }
            }
        }

        public void ReplyTo(string message, string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                throw new ArgumentNullException(nameof(ipAddress));

            try
            {
                // Create a socket to send a reply to the given IP address
                var client = new TcpClient(ipAddress, 8080);
                var data = Encoding.UTF8.GetBytes(message);

                // Send the message to the connected client
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
                Console.WriteLine($"Replied to {ipAddress} with message: {message}");

                stream.Close();
                client.Close();
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Failed to send reply to {ipAddress}: {ex.Message}");
            }
        }
    }
}
