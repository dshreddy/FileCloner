﻿/******************************************************************************
 * Filename    = Client.cs
 *
 * Author      = Sai Hemanth Reddy
 * 
 * Project     = FileCloner
 *
 * Description = Client class handles network communication for the FileCloner 
 *               application. It manages requests, responses, and file transfers 
 *               with other nodes. Implements INotificationHandler for message 
 *               handling and integrates logging for communication status updates.
 *****************************************************************************/

using Networking.Communication;
using Networking;
using System.Windows.Forms;
using Networking.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.ComponentModel;

namespace FileCloner.Models.NetworkService
{
    /// <summary>
    /// The Client class handles network communication, including sending requests, receiving responses,
    /// and managing file transfers. Implements INotificationHandler to handle incoming messages.
    /// </summary>
    public class Client : INotificationHandler
    {
        // Static fields for client communicator and request ID
        private static ICommunicator client;
        private static int requestID = 0;

        // Action delegate for logging messages, serializer for data, and flags for tracking state
        private readonly Action<string> logAction;
        private readonly ISerializer serializer;
        private bool isSummarySent = false;
        private List<string> responders = new List<string>();

        /// <summary>
        /// Constructor initializes the client, sets up communication, and subscribes to the server.
        /// </summary>
        /// <param name="logAction">Action delegate to log messages to UI or console.</param>
        /// <param name="ipAddress">IP address of the server to connect to.</param>
        /// <param name="port">Port of the server to connect to.</param>
        public Client(Action<string> logAction, string ipAddress, string port)
        {
            this.logAction = logAction;
            client = CommunicationFactory.GetCommunicator(isClientSide: true);
            serializer = new Serializer();

            // Start the client connection
            string result = client.Start(ipAddress, port);
            if (result == Constants.success)
            {
                logAction?.Invoke("[Client] Connected to server!");
                client.Subscribe(Constants.moduleName, this, false); // Subscribe to receive notifications
            }
            else
            {
                logAction?.Invoke("[Client] Failed to connect to server.");
            }
        }

        /// <summary>
        /// Sends a broadcast request to initiate the file cloning process.
        /// </summary>
        public void SendRequest()
        {
            try
            {
                isSummarySent = false;
                Message message = new Message
                {
                    Subject = Constants.request,
                    RequestID = requestID,
                    From = Constants.IPAddress,
                    To = Constants.broadcast
                };

                // Send serialized message to server
                client.Send(serializer.Serialize<Message>(message), Constants.moduleName, null);
                logAction?.Invoke("[Client] Request Sent");
            }
            catch (Exception ex)
            {
                logAction?.Invoke("[Client] Request Failed : " + ex.Message);
            }
        }

        /// <summary>
        /// Sends a summary of the cloned files to each responder.
        /// </summary>
        public void SendSummary()
        {
            foreach (string responder in responders)
            {
                try
                {
                    // Generate path for the summary file specific to each responder
                    string filePath = Path.Combine(Constants.senderFilesFolderPath, $"{responder}.txt");

                    // Read file content for summary
                    string fileContent;
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        fileContent = reader.ReadToEnd();
                    }

                    // Create message containing the summary content
                    Message message = new Message
                    {
                        Subject = Constants.summary,
                        RequestID = requestID,
                        From = Constants.IPAddress,
                        To = responder,
                        Body = fileContent
                    };

                    // Send the message
                    client.Send(serializer.Serialize<Message>(message), Constants.moduleName, "");
                    logAction?.Invoke($"[Client] Summary Sent to {responder}");
                }
                catch (Exception ex)
                {
                    logAction?.Invoke($"[Client] Failed to send summary to {responder} : {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Sends a response to a received request message, including input file content.
        /// </summary>
        public void SendResponse(Message data)
        {
            try
            {
                Message message = new Message
                {
                    Subject = Constants.response,
                    RequestID = data.RequestID,
                    From = Constants.IPAddress,
                    To = data.From,
                    Body = string.Join(Environment.NewLine, File.ReadAllLines(Constants.inputFilePath))
                };

                client.Send(serializer.Serialize<Message>(message), Constants.moduleName, "");
                logAction?.Invoke($"[Client] Response Sent to {data.From}");
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"[Client] Failed to send response to {data.From} : {ex.Message}");
            }
        }

        /// <summary>
        /// Sends a file to the requester for cloning, specifying both local and requester paths.
        /// </summary>
        public void SendFileForCloning(string from, string path, string requesterPath)
        {
            try
            {
                //Message message = new Message
                //{
                //    Subject = Constants.cloning,
                //    RequestID = requestID,
                //    From = Constants.IPAddress,
                //    MetaData = requesterPath,
                //    To = from,
                //    Body = string.Join(Environment.NewLine, File.ReadAllLines(path))
                //};

                //client.Send(serializer.Serialize<Message>(message), Constants.moduleName, "");

                Thread senderThread = new Thread(() =>
                {
                    SendFilesInChunks(from, path, requesterPath);
                });
                senderThread.Start();


                logAction?.Invoke($"[Client] Response Sent to {from}");
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"[Client] Failed to send response to from {from} : {ex.Message}");
            }
        }

        /// <summary>
        /// Function to send files in chunks rather than at once
        /// this function can send any type of file over the network
        /// </summary>
        /// <param name="from"></param>
        /// <param name="path"></param>
        /// <param name="requesterPath"></param>
        public void SendFilesInChunks(string from, string path, string requesterPath)
        {
            using FileStream fileStream = new(path, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[Constants.FileChunkSize];
            int bytesRead = 0;
            int numberOfChunksSent = 0;
            FileInfo fileInfo = new(path);
            long fileSizeInBytes = fileInfo.Length;

            long numberOfTransmissionsRequired = (long)Math.Ceiling(
                (double)fileSizeInBytes / (double)Constants.FileChunkSize
            );

            try
            {
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Message message = new Message
                    {
                        Subject = Constants.cloning,
                        RequestID = requestID,
                        From = Constants.IPAddress,
                        MetaData = requesterPath,
                        To = from,
                        Body = $"{numberOfChunksSent}/{numberOfTransmissionsRequired}:" + serializer.Serialize(buffer)
                    };

                    client.Send(serializer.Serialize<Message>(message), Constants.moduleName, "");
                    ++numberOfChunksSent;
                    logAction?.Invoke($"[Client] Response Sent to {from}");
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke(
                    $"[Client] Exception occured while sending {numberOfChunksSent} : {ex.Message}"
                );
            }


        }

        /// <summary>
        /// Stops the cloning process by incrementing request ID to track new requests.
        /// </summary>
        public void StopCloning()
        {
            requestID++;
            isSummarySent = false;
        }

        /// <summary>
        /// Handles incoming data and processes it based on the message subject.
        /// </summary>
        public void OnDataReceived(string serializedData)
        {
            Message data = serializer.Deserialize<Message>(serializedData);
            string subject = data.Subject;
            string from = data.From;

            // Prevent processing self-sent messages
            if (from != Constants.IPAddress || requestID != data.RequestID)
            {
                logAction?.Invoke($"[Client] Received {subject} from {from}");

                switch (subject)
                {
                    case Constants.request:
                        SendResponse(data);
                        break;
                    case Constants.response:
                        OnResponseReceived(data);
                        break;
                    case Constants.summary:
                        OnSummaryReceived(data);
                        break;
                    case Constants.cloning:
                        OnFileForCloningReceived(data);
                        break;
                }
            }
        }

        /// <summary>
        /// Processes a response message by saving the received file locally.
        /// </summary>
        public void OnResponseReceived(Message data)
        {
            try
            {
                responders.Add(data.From);
                string savePath = Path.Combine(Constants.receivedFilesFolderPath, $"{data.From}.json");
                File.WriteAllText(savePath, data.Body);
            }
            catch (Exception e)
            {
                logAction?.Invoke($"[Client] Failed to save response from {data.From} : {e}");
            }
        }

        /// <summary>
        /// Processes a summary message by extracting file paths and sending files for cloning.
        /// </summary>
        public void OnSummaryReceived(Message data)
        {
            try
            {
                // Parse each line containing local and requester paths
                var lines = data.Body.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var paths = line.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                    if (paths.Length == 2)
                    {
                        string localPath = paths[0].Trim();
                        string requesterPath = paths[1].Trim();

                        // Send file for cloning using the specified paths
                        SendFileForCloning(data.From, localPath, requesterPath);
                    }
                    else
                    {
                        logAction?.Invoke($"[Client] Invalid path format in summary data: {line}");
                    }
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"[Client] Failed to process summary: {ex.Message}");
            }
        }

        /// <summary>
        /// Processes a cloning file message by saving the received file to the specified path.
        /// </summary>
        public void OnFileForCloningReceived(Message data)
        {
            long chunkNumber = 0;
            long numberOfTransmissionsRequired = 0;
            try
            {
                // Extract the save path from message metadata
                string requesterPath = data.MetaData;

                // Ensure directory exists for the requester path
                Directory.CreateDirectory(Path.GetDirectoryName(requesterPath));

                // Write the file content to the specified path
                //using (StreamWriter writer = new StreamWriter(requesterPath))
                //{
                //    writer.Write(data.Body);
                //}

                string messageBody = data.Body;
                string[] messageBodyList = messageBody.Split(':', 2);
                if (messageBodyList.Length != 2)
                {
                    return;
                }

                string[] chunkNumberByNumberOfTransmissionsRequired = messageBodyList[0].Split('/', 2);
                if (chunkNumberByNumberOfTransmissionsRequired.Length != 2) 
                {
                    // code should never reach here
                    logAction?.Invoke("Sending Files in Wrong Format");
                    // throw new Exception("Sending Files in Wrong Format");
                    return;
                }

                chunkNumber = long.Parse(chunkNumberByNumberOfTransmissionsRequired[0]);
                numberOfTransmissionsRequired = long.Parse(chunkNumberByNumberOfTransmissionsRequired[1]);


                string serializedFileContent = messageBodyList[1];
                FileMode fileMode = chunkNumber == 0 ? FileMode.Create : FileMode.Append;
                byte[] buffer = serializer.Deserialize<byte[]>(serializedFileContent);

                using FileStream fileStream = new FileStream(requesterPath, fileMode, FileAccess.Write);
                if (buffer != null)
                {
                    fileStream.Write(buffer, 0, buffer.Length);
                }

                logAction?.Invoke(
                    $"[Client] File chunk : {chunkNumber}/{numberOfTransmissionsRequired} from"
                    + $" {data.From} and saved to {requesterPath}"
                );

                if (chunkNumber == numberOfTransmissionsRequired)
                {
                    logAction?.Invoke($"[Client] File received from {data.From} and saved to {requesterPath}");
                }
            }
            catch (Exception ex)
            {
                // try to log it some file regarding how many chunks have been saved
                logAction?.Invoke($"[Client] Failed to save received file from {data.From}: {ex.Message}");
            }
        }

        /// <summary>
        /// Stops the client communicator.
        /// </summary>
        public void Stop()
        {
            client.Stop();
        }
    }
}
