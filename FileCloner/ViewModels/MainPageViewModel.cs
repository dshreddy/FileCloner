/******************************************************************************
 * Filename    = MainPageViewModel.cs
 *
 * Author      = Sai Hemanth Reddy
 * 
 * Project     = FileCloner
 *
 * Description = ViewModel for MainPage, handling directory loading, file structure 
 *               generation, and managing file/folder selection states. Implements 
 *               commands for sending requests, summarizing responses, and starting 
 *               file cloning.
 *****************************************************************************/

using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using FileCloner.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using FileCloner.Models.NetworkService;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace FileCloner.ViewModels
{
    /// <summary>
    /// ViewModel for the MainPage, handling file operations, commands, and UI bindings.
    /// </summary>
    public class MainPageViewModel : ViewModelBase
    {
        // Bindable property for the root directory path
        private string _rootDirectoryPath;
        public string RootDirectoryPath
        {
            get => _rootDirectoryPath;
            set
            {
                _rootDirectoryPath = value;
                OnPropertyChanged(nameof(RootDirectoryPath));
            }
        }

        private Node _selectedNode;
        public Node SelectedNode
        {
            get => _selectedNode;
            set
            {
                _selectedNode = value;
                OnPropertyChanged(nameof(SelectedNode));
            }
        }

        // Property to track the number of files in the directory
        private int _fileCount;
        public int FileCount
        {
            get => _fileCount;
            set
            {
                _fileCount = value;
                OnPropertyChanged(nameof(FileCount));
            }
        }

        // Property to track the number of folders in the directory
        private int _folderCount;
        public int FolderCount
        {
            get => _folderCount;
            set
            {
                _folderCount = value;
                OnPropertyChanged(nameof(FolderCount));
            }
        }

        // Property to track the count of selected folders in the UI
        private int _selectedFoldersCount;
        public int SelectedFoldersCount
        {
            get => _selectedFoldersCount;
            set
            {
                _selectedFoldersCount = value;
                OnPropertyChanged(nameof(SelectedFoldersCount));
            }
        }

        // Property to track the count of selected files in the UI
        private int _selectedFilesCount;
        public int SelectedFilesCount
        {
            get => _selectedFilesCount;
            set
            {
                _selectedFilesCount = value;
                OnPropertyChanged(nameof(SelectedFilesCount));
            }
        }

        // Property to track the total size of selected files in bytes
        private long _sumOfSelectedFilesSizeInBytes;
        public long SumofSelectedFilesSizeInBytes
        {
            get => _sumOfSelectedFilesSizeInBytes;
            set
            {
                _sumOfSelectedFilesSizeInBytes = value;
                OnPropertyChanged(nameof(SumofSelectedFilesSizeInBytes));
            }
        }

        // UI Commands for button actions
        public ICommand BrowseFoldersCommand { get; }
        public ICommand SendRequestCommand { get; }
        public ICommand SummarizeCommand { get; }
        public ICommand StartCloningCommand { get; }
        public ICommand StopCloningCommand { get; }

        // Collection to store log messages for display
        public ObservableCollection<string> LogMessages { get; set; } = new ObservableCollection<string>();

        // Tree structure of nodes representing files and directories
        public ObservableCollection<Node> Tree { get; set; }
        private readonly FileExplorerServiceProvider _fileExplorerServiceProvider;

        // Dictionary to store selected files, with address as key and list of file paths as value
        public static Dictionary<string, List<string>> _selectedFiles = new();

        // Client and Server instances for network communication
        private readonly Client _client;
        private readonly Server _server;

        // Singleton instance (assuming this pattern is suitable for your use case)
        private static MainPageViewModel _instance;

        /// <summary>
        /// Constructor for MainPageViewModel - initializes paths, commands, and communication setup.
        /// </summary>
        public MainPageViewModel()
        {
            _instance = this; // Set instance for static access

            // Set default root directory path
            RootDirectoryPath = Constants.defaultFolderPath;

            // Initialize FileExplorerServiceProvider to manage file operations
            _fileExplorerServiceProvider = new FileExplorerServiceProvider();

            // Initialize UI commands and their respective handlers
            SendRequestCommand = new RelayCommand(SendRequest);
            SummarizeCommand = new RelayCommand(SummarizeResponses);
            StartCloningCommand = new RelayCommand(StartCloning);
            BrowseFoldersCommand = new RelayCommand(BrowseFolders);
            StopCloningCommand = new RelayCommand(StopCloning);

            // Subscribe to CheckBoxClickEvent to update selection counts when a checkbox is clicked
            Node.CheckBoxClickEvent += UpdateCounts;

            // Initialize the Tree structure representing files and folders
            Tree = new ObservableCollection<Node>();
            TreeGenerator(_rootDirectoryPath);  // Load the initial tree structure

            // Initialize server and client for handling file transfer communication
            _server = new Server(UpdateLog);
            _client = new Client(UpdateLog, Constants.IPAddress, "8080");

            // Register for application exit event to ensure resources are released
            System.Windows.Application.Current.Exit += (sender, e) =>
            {
                _client.Stop();
                _server.Stop();
            };
        }

        /// <summary>
        /// Generates the initial tree structure of the directory specified in RootDirectoryPath.
        /// </summary>
        private void TreeGenerator(string filePath)
        {
            try
            {
                // Clear any existing nodes in the tree and reset counters
                Tree.Clear();
                ResetCounts();
                if(filePath == Constants.outputFilePath)
                {
                    RootGenerator(filePath);
                    return;
                }

                // Generate input file representing the structure of the root directory
                _fileExplorerServiceProvider.GenerateInputFile(RootDirectoryPath);

                // Parse the input file and create tree nodes
                RootGenerator(Constants.inputFilePath);
            }
            catch (Exception e)
            {
                // Show error if tree generation fails
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Parses the JSON file representing directory structure and generates the root node.
        /// </summary>
        private void RootGenerator(string filePath)
        {
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                var rootDictionary = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonContent);

                foreach (var root in rootDictionary)
                {
                    string color = root.Value.TryGetProperty("COLOR", out JsonElement colorProperty) ? colorProperty.GetString() : "";
                    // Create a root node for each directory
                    var rootNode = new Node
                    {
                        Name = root.Key,
                        IsFile = false,
                        IconPath = new Uri(Constants.folderIconPath, UriKind.Absolute),
                        Color = color,
                        RelativePath = root.Value.TryGetProperty("RELATIVE_PATH", out JsonElement relativePath) ? relativePath.GetString() : "",
                        IpAddress = root.Value.TryGetProperty("ADDRESS", out JsonElement address) ? address.GetString() : null,
                        FullFilePath = root.Value.TryGetProperty("FULL_PATH", out JsonElement fullFilePath) ? fullFilePath.GetString() : "PATH NOT GIVEN!",
                    };
                    // Add root node to the tree and increment folder count
                    Tree.Add(rootNode);
                    FolderCount++;
                    PopulateChildren(rootNode, (JsonElement)root.Value);  // Recursively populate child nodes
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Recursively populates child nodes for a given parent node.
        /// </summary>
        private void PopulateChildren(Node parentNode, JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty("CHILDREN", out JsonElement childrenElement))
            {
                foreach (var child in childrenElement.EnumerateObject())
                {
                    bool isFile = child.Value.TryGetProperty("SIZE", out JsonElement sizeElement);
                    string color = child.Value.TryGetProperty("COLOR", out JsonElement colorProperty) ? colorProperty.GetString() : "";
                    var childNode = new Node
                    {
                        Name = child.Name,
                        Color = color,
                        IpAddress = child.Value.TryGetProperty("ADDRESS", out JsonElement address) ? address.GetString() : "localhost",
                        FullFilePath = child.Value.TryGetProperty("FULL_PATH", out JsonElement fullFilePath) ? fullFilePath.GetString() : "PATH NOT GIVEN!",
                        IsFile = isFile,
                        Size = isFile ? sizeElement.GetInt32() : 0,
                        Parent = parentNode,
                        RelativePath = child.Value.TryGetProperty("RELATIVE_PATH", out JsonElement relativePath) ? relativePath.GetString() : "",
                        IconPath = new Uri(isFile ? Constants.fileIconPath : Constants.folderIconPath, UriKind.Absolute)
                    };
                    if(color == "GREEN" || color == "RED")
                    {
                        childNode.IsChecked = true;
                        childNode.CheckBoxClick();
                    }
                    parentNode.Children.Add(childNode);

                    // Increment counters based on node type
                    if (isFile)
                    {
                        FileCount++;
                    }
                    else
                    {
                        FolderCount++;
                        PopulateChildren(childNode, child.Value); // Recurse if it's a folder
                    }
                }
            }
        }

        /// <summary>
        /// Resets the file and folder counters and updates UI.
        /// </summary>
        public void ResetCounts()
        {
            FileCount = 0;
            FolderCount = 0;
            Node.SelectedFolderCount = 0;
            Node.SelectedFilesCount = 0;
            Node.SumOfSelectedFilesSizeInBytes = 0;
            UpdateCounts();
        }

        /// <summary>
        /// Updates the UI counters for selected files, folders, and their total size.
        /// </summary>
        public void UpdateCounts()
        {
            SelectedFoldersCount = Node.SelectedFolderCount;
            SelectedFilesCount = Node.SelectedFilesCount;
            SumofSelectedFilesSizeInBytes = Node.SumOfSelectedFilesSizeInBytes;
        }

        // Static method to retrieve the instance's RootDirectoryPath
        public static string GetRootDirectoryPath()
        {
            // You could retrieve the instance from a service locator or use a singleton pattern if appropriate.
            // For simplicity, let's assume a single instance is created somewhere and stored as a static reference.
            return _instance?._rootDirectoryPath ?? string.Empty;
        }

        /// <summary>
        /// Adds or removes files to/from _selectedFiles based on checkbox selection.
        /// </summary>
        public static void UpdateSelectedFiles(string address, string fullPath, string relativePath, bool isChecked)
        {
            if (address == Constants.IPAddress) return;

            string rootDirectoryPath = GetRootDirectoryPath();

            if (isChecked)
            {
                if (!_selectedFiles.ContainsKey(address))
                {
                    _selectedFiles[address] = new List<string>();
                }
                _selectedFiles[address].Add($"{fullPath}, {Path.Combine(rootDirectoryPath, relativePath)}");
            }
            else
            {
                if (_selectedFiles.ContainsKey(address))
                {
                    _selectedFiles[address].Add($"{fullPath}, {Path.Combine(rootDirectoryPath, relativePath)}");

                    // Remove entry if no files are left for the address
                    if (_selectedFiles[address].Count == 0)
                    {
                        _selectedFiles.Remove(address);
                    }
                }
            }
        }

        /// <summary>
        /// Opens a folder picker dialog for selecting a root directory.
        /// </summary>
        private void BrowseFolders()
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Title = "Select the Root Directory for File Cloning";

                // If a new directory is selected, update and regenerate the tree
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && dialog.FileName != RootDirectoryPath)
                {
                    RootDirectoryPath = dialog.FileName;
                    TreeGenerator(_rootDirectoryPath);
                }
            }
        }

        /// <summary>
        /// Sends a request to initiate the file cloning process.
        /// </summary>
        private void SendRequest()
        {
            try
            {
                _client.SendRequest();
                MessageBox.Show("Request sent successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Generates and displays a summary of responses.
        /// </summary>
        private void SummarizeResponses()
        {
            SummaryGenerator.GenerateSummary();
            Dispatcher.Invoke(() => {
                MessageBox.Show("Summary is generated");
            });
            TreeGenerator(Constants.outputFilePath);
        }

        /// <summary>
        /// Starts the cloning process by creating files from selected items.
        /// </summary>
        private void StartCloning()
        {
            // Ensure that the directory for sender files exists
            Directory.CreateDirectory(Constants.senderFilesFolderPath);

            foreach (var entry in _selectedFiles)
            {
                string key = entry.Key;
                List<string> value = entry.Value;
                string filePath = Path.Combine(Constants.senderFilesFolderPath, $"{key}.txt");

                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    foreach (string filePathInValue in value)
                    {
                        writer.WriteLine(filePathInValue);
                    }
                }
            }
            _client.SendSummary();
        }

        /// <summary>
        /// Stops the cloning process with a warning prompt.
        /// </summary>
        private void StopCloning()
        {
            string message = "If you stop cloning, all incoming files related to the current session will be ignored.";
            string title = "WARNING";
            MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
            DialogResult result = MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);

            if (result == DialogResult.OK)
            {
                _client.StopCloning();
            }
        }

        /// <summary>
        /// Adds a message to the log with timestamp for UI display.
        /// </summary>
        private void UpdateLog(string message)
        {
            Dispatcher.Invoke(() =>
            {
                LogMessages.Insert(0, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]-  {message}");
                OnPropertyChanged(nameof(LogMessages));
            });
        }
    }
}
