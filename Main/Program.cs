using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Choose mode (1 for Server, 2 for Client):");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Server.StartServer();
            }
            else if (choice == "2")
            {
                Client.StartClient();
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }
    }

    class Server
    {
        private static List<TcpClient> connectedClients = new List<TcpClient>();
        private static TcpListener listener;

        public static void StartServer()
        {
            int port = 8080;
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            Console.WriteLine("Server started on port " + port);

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                connectedClients.Add(client);

                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        }

        private static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            string clientEndPoint = client.Client.RemoteEndPoint.ToString();

            Console.WriteLine("Client connected: " + clientEndPoint);
            BroadcastMessage("User connected: " + clientEndPoint);

            byte[] buffer = new byte[1024];
            int byteCount;
            try
            {
                while ((byteCount = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.ASCII.GetString(buffer, 0, byteCount);
                    Console.WriteLine("Received: " + message);

                    if (message.StartsWith("REQUEST_ACCESS"))
                    {
                        string folderPath = message.Substring("REQUEST_ACCESS ".Length);
                        HandleAccessRequest(client, folderPath);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Client disconnected: " + clientEndPoint);
                connectedClients.Remove(client);
                BroadcastMessage("User disconnected: " + clientEndPoint);
            }
        }

        private static void HandleAccessRequest(TcpClient client, string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                SendFolderContents(client, folderPath);
            }
            else
            {
                SendMessage(client, "ERROR: Folder not found.");
            }
        }

        private static void SendFolderContents(TcpClient client, string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath);

            foreach (string file in files)
            {
                byte[] fileNameBytes = Encoding.ASCII.GetBytes(Path.GetFileName(file) + "\n");
                client.GetStream().Write(fileNameBytes, 0, fileNameBytes.Length);

                byte[] fileBytes = File.ReadAllBytes(file);
                client.GetStream().Write(fileBytes, 0, fileBytes.Length);
            }

            SendMessage(client, "END_OF_FILES");
        }

        private static void SendMessage(TcpClient client, string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            client.GetStream().Write(buffer, 0, buffer.Length);
        }

        private static void BroadcastMessage(string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);

            foreach (TcpClient client in connectedClients)
            {
                if (client.Connected)
                {
                    NetworkStream stream = client.GetStream();
                    stream.Write(buffer, 0, buffer.Length);
                }
            }

            Console.WriteLine("Broadcast: " + message);
        }
    }

    class Client
    {
        private static TcpClient client;
        private static NetworkStream stream;
        private static string bufferFolderPath = "Buffer";

        public static void StartClient()
        {
            string serverIp = "127.0.0.1";
            int port = 8080;

            try
            {
                client = new TcpClient(serverIp, port);
                stream = client.GetStream();

                Console.WriteLine("Connected to server.");

                Thread readThread = new Thread(ReadMessages);
                readThread.Start();

                Console.WriteLine("Enter the full path of the folder you want to request access to:");
                string folderPath = Console.ReadLine();

                RequestFolderAccess(folderPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static void RequestFolderAccess(string folderPath)
        {
            string message = "REQUEST_ACCESS " + folderPath;
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }

        private static void ReadMessages()
        {
            if (!Directory.Exists(bufferFolderPath))
            {
                Directory.CreateDirectory(bufferFolderPath);
            }

            byte[] buffer = new byte[1024];
            int byteCount;

            try
            {
                while ((byteCount = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.ASCII.GetString(buffer, 0, byteCount);

                    if (message.StartsWith("ERROR"))
                    {
                        Console.WriteLine("Server: " + message);
                    }
                    else if (message == "END_OF_FILES")
                    {
                        Console.WriteLine("File transfer complete.");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Receiving file: " + message.Trim());
                        string fileName = message.Trim();
                        string filePath = Path.Combine(bufferFolderPath, fileName);

                        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            byteCount = stream.Read(buffer, 0, buffer.Length);
                            fs.Write(buffer, 0, byteCount);
                        }

                        Console.WriteLine("File saved to: " + filePath);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Disconnected from server.");
            }
        }
    }
}
