using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileCloner.Models
{
    public class Requester : IRequester
    {

        /// <summary>
        /// The server's hostname (assumed to be localhost).
        /// </summary>
        static string ServerHostName = "127.0.0.1";

        /// <summary>
        /// The server's port number for communication.
        /// </summary>
        static int ServerPortNumber = 9090;

        /// <summary>
        /// Sends a request to the server to get the list of IP addresses for all connected clients.
        /// </summary>
        /// <returns>A list of strings representing the IP addresses received from the server.</returns>
        /// <remarks>
        /// This method connects to the server, sends a request message, and continuously reads the
        /// server's response until all data is received. The server is expected to send a 
        /// comma-separated list of IP addresses.
        /// </remarks>
        /// <exception cref="SocketException">
        /// Thrown when there is a problem with the underlying socket connection.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when a general exception occurs while connecting or communicating with the server.
        /// </exception>
        public List<string> GetAllIPAddresses()
        {
            var ipAddressList = new List<string>();
            try
            {
                // Create a socket and connect to the server (assuming localhost and port 9090)
                var clientSocket = new TcpClient(ServerHostName, ServerPortNumber);
                Console.WriteLine($"Connected to the server ({ServerHostName}:{ServerPortNumber}).");

                // Send a request message to the server
                string requestMessage = "Requesting all IP Addresses";
                byte[] requestBuffer = Encoding.UTF8.GetBytes(requestMessage);
                NetworkStream stream = clientSocket.GetStream();
                stream.Write(requestBuffer, 0, requestBuffer.Length);
                Console.WriteLine("Request sent: 'Requesting all IP Addresses'");

                // Receive the response from the server
                byte[] receiveBuffer = new byte[1024];  // Buffer to store incoming data
                int bytesRead;

                StringBuilder receivedData = new StringBuilder();

                // Keep reading from the stream until there's no more data
                while ((bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length)) > 0)
                {
                    receivedData.Append(Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead));
                }

                // Assuming the server sends IP addresses as a comma-separated string
                string[] ipAddresses = receivedData.ToString().Split(',');
                ipAddressList.AddRange(ipAddresses);
                Console.WriteLine($"Received IP addresses: {string.Join(", ", ipAddresses)}");

                // Close the stream and socket connection
                stream.Close();
                clientSocket.Close();
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"SocketException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            return ipAddressList;  // Return the list of IP addresses
        }

        /// <summary>
        /// Attempts to establish a connection with a client at the specified IP address and port number.
        /// </summary>
        /// <param name="ipAddress">The IP address of the client to connect to.</param>
        /// <param name="portNumber">The port number on the client to which the connection is attempted.</param>
        /// <returns>
        /// A <see cref="ConnectionStatus"/> indicating whether the connection was successful or failed.
        /// </returns>
        /// <remarks>
        /// This method creates a socket and attempts to connect to the given IP address on the specified
        /// port. If the connection fails, it returns <see cref="ConnectionStatus.ConnectionFailure"/>, 
        /// otherwise it returns <see cref="ConnectionStatus.ConnectionSuccess"/>.
        /// </remarks>
        /// <exception cref="SocketException">
        /// Thrown if the connection to the client fails due to a socket-related issue.
        /// </exception>
        public ConnectionStatus ConnectToClient(string ipAddress, int portNumber)
        {
            try
            {
                // Create a socket and attempt to connect to the given IP address
                var client = new TcpClient(ipAddress, portNumber); // Assuming port 8080 for now
                Console.WriteLine($"Connection to {ipAddress}:{portNumber} established.");
                client.Close();
                return ConnectionStatus.ConnectionSuccess;
            }
            catch (SocketException)
            {
                Console.WriteLine($"Failed to connect to {ipAddress}:{portNumber}.");
                return ConnectionStatus.ConnectionFailure;
            }
        }
    }
}
