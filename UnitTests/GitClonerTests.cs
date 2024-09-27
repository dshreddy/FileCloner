namespace UnitTests
{
    [TestClass]
    public class GitClonerTests
    {
        private string _sourceDirectory;
        private string _targetDirectory;

        // Runs before each test to set up necessary directories
        [TestInitialize]
        public void SetUp()
        {
            _sourceDirectory = Path.Combine(Path.GetTempPath(), "TestSourceFolder");
            _targetDirectory = Path.Combine(Path.GetTempPath(), "TestTargetFolder");

            // Create source directory and add a test file
            if (!Directory.Exists(_sourceDirectory))
            {
                Directory.CreateDirectory(_sourceDirectory);
                File.WriteAllText(Path.Combine(_sourceDirectory, "TestFile.txt"), "Sample file content.");
            }

            // Clean the target directory if it already exists
            if (Directory.Exists(_targetDirectory))
            {
                Directory.Delete(_targetDirectory, true);
            }
        }

        // Runs after each test to clean up the created directories
        [TestCleanup]
        public void CleanUp()
        {
            // Clean up source and target directories
            if (Directory.Exists(_sourceDirectory))
            {
                Directory.Delete(_sourceDirectory, true);
            }

            if (Directory.Exists(_targetDirectory))
            {
                Directory.Delete(_targetDirectory, true);
            }
        }

        // Test: Folder cloning should successfully copy files and directories and return Success status
        [TestMethod]
        public void CloneFolder_ShouldCopyFilesAndDirectories_Successfully()
        {
            // Arrange
            var fileCloner = new GitCloner.GitCloner();

            // Act
            var status = fileCloner.CloneFolder(_sourceDirectory, _targetDirectory);

            // Assert
            Assert.AreEqual(GitCloner.CloneStatus.Success, status, "CloneFolder should return Success when cloning is successful.");
            Assert.IsTrue(Directory.Exists(_targetDirectory), "Target directory should exist after cloning.");
            Assert.IsTrue(File.Exists(Path.Combine(_targetDirectory, "TestFile.txt")), "Test file should be copied to the target directory.");
        }

        // Test: Attempting to clone from a non-existent source folder should return SourceDirectoryNotFound
        [TestMethod]
        public void CloneFolder_SourceDirectoryNotFound_ShouldReturnSourceDirectoryNotFound()
        {
            // Arrange
            var fileCloner = new GitCloner.GitCloner();
            string invalidSourcePath = Path.Combine(Path.GetTempPath(), "NonExistentFolder");

            // Act
            var status = fileCloner.CloneFolder(invalidSourcePath, _targetDirectory);

            // Assert
            Assert.AreEqual(GitCloner.CloneStatus.SourceDirectoryNotFound, status, "CloneFolder should return SourceDirectoryNotFound when the source directory does not exist.");
            Assert.IsFalse(Directory.Exists(_targetDirectory), "Target directory should not be created when source directory does not exist.");
        }

        // Test: Cloning should return Success and create a target directory if it doesn't exist
        [TestMethod]
        public void CloneFolder_ShouldCreateTargetDirectoryAndReturnSuccess()
        {
            // Arrange
            var fileCloner = new GitCloner.GitCloner();

            // Act
            var status = fileCloner.CloneFolder(_sourceDirectory, _targetDirectory);

            // Assert
            Assert.AreEqual(GitCloner.CloneStatus.Success, status, "CloneFolder should return Success when cloning is successful and the target directory is created.");
            Assert.IsTrue(Directory.Exists(_targetDirectory), "Target directory should be created if it doesn't exist.");
        }

        // Test: Ensure subdirectories are cloned and status is Success
        [TestMethod]
        public void CloneFolder_ShouldCopySubdirectoriesAndReturnSuccess()
        {
            // Arrange
            var fileCloner = new GitCloner.GitCloner();
            string subDirectory = Path.Combine(_sourceDirectory, "SubFolder");
            Directory.CreateDirectory(subDirectory);
            File.WriteAllText(Path.Combine(subDirectory, "SubFile.txt"), "Sub folder content.");

            // Act
            var status = fileCloner.CloneFolder(_sourceDirectory, _targetDirectory);

            // Assert
            Assert.AreEqual(GitCloner.CloneStatus.Success, status, "CloneFolder should return Success when subdirectories are successfully copied.");
            string targetSubDirectory = Path.Combine(_targetDirectory, "SubFolder");
            Assert.IsTrue(Directory.Exists(targetSubDirectory), "Subdirectory should be copied to the target directory.");
            Assert.IsTrue(File.Exists(Path.Combine(targetSubDirectory, "SubFile.txt")), "Sub file should be copied to the target directory.");
        }

        // Test: Check if TargetDirectoryCreationFailed is returned when target directory creation fails
        [TestMethod]
        public void CloneFolder_TargetDirectoryCreationFailed_ShouldReturnTargetDirectoryCreationFailed()
        {
            // Arrange
            var fileCloner = new GitCloner.GitCloner();
            string invalidTargetPath = Path.Combine("Z:\\", "InvalidTarget"); // Invalid path to simulate failure

            // Act
            var status = fileCloner.CloneFolder(_sourceDirectory, invalidTargetPath);

            // Assert
            Assert.AreEqual(GitCloner.CloneStatus.TargetDirectoryCreationFailed, status, "CloneFolder should return TargetDirectoryCreationFailed when the target directory cannot be created.");
        }
    }
}
