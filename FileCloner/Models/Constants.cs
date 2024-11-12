/******************************************************************************
 * Filename    = Constants.cs
 *
 * Author      = Sai Hemanth Reddy
 *
 * Product     = PlexShare
 * 
 * Project     = FileCloner
 *
 * Description = Defines project-level constants for icon paths, file paths
 *****************************************************************************/

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FileCloner.Models
{
    /// <summary>
    /// A class in which all project-level constants are declared as properties.
    /// </summary>
    public class Constants
    {
        // Icon Paths
        public static readonly string loadingIconPath = Path.GetFullPath(Path.Combine("..", "..", "..", "Assets", "Images", "loading.png"));
        public static readonly string driveIconPath = Path.GetFullPath(Path.Combine("..", "..", "..", "Assets", "Images", "drive.png"));
        public static readonly string fileIconPath = Path.GetFullPath(Path.Combine("..", "..", "..", "Assets", "Images", "file.png"));
        public static readonly string folderIconPath = Path.GetFullPath(Path.Combine("..", "..", "..", "Assets", "Images", "folder.png"));
        public static readonly string errorIconPath = Path.GetFullPath(Path.Combine("..", "..", "..", "Assets", "Images", "error.png"));

        // File & Folder Paths
        public static readonly string configFilePath = Path.GetFullPath(Path.Combine("..", "..", "..", "Assets", "Files", "config.txt"));
        public static readonly string defaultFolderPath = Path.GetFullPath(Path.Combine("..", "..", "..", "Assets"));
        public static readonly string inputFilePath = Path.GetFullPath(Path.Combine("..", "..", "..", "Assets", "Files", "input.json"));
        public static readonly string outputFilePath = Path.GetFullPath(Path.Combine("..", "..", "..", "Assets", "Files", "output.json"));
        public static readonly string receivedFilesFolderPath = Path.GetFullPath(Path.Combine("..", "..", "..", "Assets", "Files", "ReceivedFiles"));
        public static readonly string senderFilesFolderPath = Path.GetFullPath(Path.Combine("..", "..", "..", "Assets", "Files", "SenderFiles"));

        // Network Service Constants
        public const string success = "success";
        public const string moduleName = "FileCloner";
        public const string request = "request";
        public const string response = "response";
        public const string summary = "summary";
        public const string cloning = "cloning";
        public const string broadcast = "BroadCast";
        public static string IPAddress = GetIP();

        // Size of FileChunk to be sent over network
        public static int FileChunkSize = 4096;

        private static string GetIP()
        {
            try
            {
                // Get the IP address of the machine
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

                // Iterate through the IP addresses and return the IPv4 address that does not end with "1"
                foreach (IPAddress ipAddress in host.AddressList)
                {
                    if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        string address = ipAddress.ToString();
                        if (!address.EndsWith(".1"))
                        {
                            return address;
                        }
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                return "";
            }
            return "";
        }
    }
}
