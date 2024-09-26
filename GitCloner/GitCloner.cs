using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitCloner
{
    public class GitCloner : IGitCloner
    {
        public GitCloner() { }

        public void CloneFolder(string sourcePath, string targetPath)
        {
            try
            {
                // Ensure the source directory exists
                if (!Directory.Exists(sourcePath))
                {
                    Console.WriteLine($"Source directory does not exist: {sourcePath}");
                    return;
                }

                // Create the target directory if it doesn't exist
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                // Copy all files and subdirectories
                DirectoryCopy(sourcePath, targetPath, true);

                Console.WriteLine($"Folder cloned successfully from {sourcePath} to {targetPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while cloning the folder: {ex.Message}");
            }
        }

        private void DirectoryCopy(string sourceDir, string destinationDir, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);

            // Get all files and copy them
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            // If copying subdirectories, copy them and their contents recursively
            if (copySubDirs)
            {
                DirectoryInfo[] subDirs = dir.GetDirectories();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    string newSubDirPath = Path.Combine(destinationDir, subDir.Name);
                    DirectoryCopy(subDir.FullName, newSubDirPath, copySubDirs);
                }
            }
        }
    }
}
