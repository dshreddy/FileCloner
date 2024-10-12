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
        /// <summary>
        /// The port number where the server listens for incoming requests.
        /// </summary>
        static int listeningPortNumber = 8080;

        /// <summary>
        /// Listens for incoming connection requests from other clients on the specified IP address and port.
        /// </summary>
        /// <param name="ipAddress">The IP address on which to listen for incoming requests.</param>
        /// <remarks>
        /// This method sets up a TCP listener on the provided IP address and port number, accepting incoming 
        /// connection requests from clients. Once a connection is accepted, the client socket is closed, 
        /// simulating the handling of a request.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ipAddress"/> is null or empty.</exception>
        /// <exception cref="SocketException">Thrown if there is an error in setting up or handling the TCP listener.</exception>
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

                    // (Additional handling of the client request can go here)

                    client.Close();
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Error while listening for requests: {ex.Message}");
                    break;
                }
            }
        }

        /// <summary>
        /// Sends a reply message to the specified IP address over a TCP connection.
        /// </summary>
        /// <param name="message">The message to be sent to the client.</param>
        /// <param name="ipAddress">The IP address of the client to send the reply to.</param>
        /// <remarks>
        /// This method establishes a TCP connection to the client at the given IP address and port, 
        /// then sends the provided message. The connection is closed after the message is sent.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="ipAddress"/> is null or empty.
        /// </exception>
        /// <exception cref="SocketException">
        /// Thrown if there is an error in establishing the TCP connection or sending the message.
        /// </exception>
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
