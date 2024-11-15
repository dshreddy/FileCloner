/******************************************************************************
 * Filename    = Server.cs
 *
 * Author      = Sai Hemanth Reddy
 *
 * Project     = FileCloner
 *
 * Description = Manages server-side communication for the FileCloner application,
 *               handling client connections, message broadcasting, and targeted
 *               communication between clients. The Server class maintains a list
 *               of connected clients and ensures messages are delivered to the
 *               appropriate recipients.
 *****************************************************************************/

using Networking.Communication;
using System.Net.Sockets;
using System.Net;
using Networking;
using Networking.Serialization;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace FileCloner.Models.NetworkService
{
    /// <summary>
    /// The Server class manages incoming and outgoing communication with clients,
    /// handling data routing, client management, and logging for the FileCloner application.
    /// </summary>
    public class Server : INotificationHandler
    {
        // Instance of the server communicator for managing connections
        private static CommunicatorServer server =
            (CommunicatorServer)CommunicationFactory.GetCommunicator(isClientSide: false);

        // Counter for assigning unique IDs to clients as they join
        private static int clientid = 0;

        // Dictionary to store the mapping of client IP addresses to their unique IDs
        private static Dictionary<string, string> clientList = new();

        // Serializer for handling message serialization and deserialization
        private static ISerializer serializer = new Serializer();

        // Delegate for logging actions, e.g., writing to UI or console
        private Action<string> logAction;

        private static Server _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Initializes the server, starts listening on the specified port,
        /// and subscribes to the message handler for the module.
        /// </summary>
        /// <param name="logAction">Delegate for logging status updates and errors.</param>
        private Server()
        {
            server.Subscribe(Constants.moduleName, this, false);
        }

        /// <summary>
        /// Gets the singleton instance of the Server class, optionally updating the log action.
        /// </summary>
        /// <param name="logAction">Delegate for logging status updates and errors (optional).</param>
        /// <returns>The singleton instance of the Server class.</returns>
        public static Server GetServerInstance(Action<string> logAction = null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new Server();
                }

                // Update the logAction if a new one is provided
                if (logAction != null)
                {
                    _instance.logAction = logAction;
                }
            }

            return _instance;
        }

        public void SetUser(string clientId, TcpClient socket)
        {
            string clientIpAddress = ((IPEndPoint)socket.Client.RemoteEndPoint).Address.ToString();
            clientList.Add(clientIpAddress, clientId);
            if (logAction != null)
            {
                logAction.Invoke($"[Server] {clientIpAddress} Joined");
            }
        }

        /// <summary>
        /// Handles data received from clients, determines if it's a broadcast or
        /// directed message, and routes it accordingly.
        /// </summary>
        /// <param name="serializedData">The serialized data received from a client.</param>
        public void OnDataReceived(string serializedData)
        {
            try
            {
                // Deserialize the message to process its details
                Message message = serializer.Deserialize<Message>(serializedData);

                if (message == null)
                {
                    return;
                }

                // Check if the message is a broadcast
                if (message.To == Constants.broadcast)
                {
                    // Send to all connected clients if it's a broadcast
                    server.Send(serializedData, Constants.moduleName, null);
                }
                else
                {
                    // Targeted message; find and send to the specified client
                    string targetClientId = clientList[message.To];
                    server.Send(serializedData, Constants.moduleName, targetClientId);
                }
            }
            catch (Exception e)
            {
                logAction.Invoke("[Server] Error in sending data: " + e.Message);
            }
        }

        /// <summary>
        /// Removes a client from the client list when they disconnect.
        /// </summary>
        /// <param name="clientId">The unique ID of the client that left.</param>
        public void OnClientLeft(string clientId)
        {
            // Find the client in the dictionary by clientId
            var clientEntry = clientList.FirstOrDefault(entry => entry.Value == clientId);
            if (!string.IsNullOrEmpty(clientEntry.Key))
            {
                logAction.Invoke($"[Server] {clientList[clientEntry.Key]} Left");
                clientList.Remove(clientEntry.Key);
            }
        }

        /// <summary>
        /// Stops the server and terminates all client connections.
        /// </summary>
        public void Stop()
        {
            server.Stop();
        }
    }
}