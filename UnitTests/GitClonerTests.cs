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

        // Test: Folder cloning should successfully copy files and directories
        [TestMethod]
        public void CloneFolder_ShouldCopyFilesAndDirectories_Successfully()
        {
            // Arrange
            var fileCloner = new GitCloner.GitCloner();

            // Act
            fileCloner.CloneFolder(_sourceDirectory, _targetDirectory);

            // Assert
            Assert.IsTrue(Directory.Exists(_targetDirectory), "Target directory should exist after cloning.");
            Assert.IsTrue(File.Exists(Path.Combine(_targetDirectory, "TestFile.txt")), "Test file should be copied to the target directory.");
        }

        // Test: Attempting to clone from a non-existent source folder should not create the target
        [TestMethod]
        public void CloneFolder_SourceDirectoryNotFound_ShouldNotCreateTarget()
        {
            // Arrange
            var fileCloner = new GitCloner.GitCloner();
            string invalidSourcePath = Path.Combine(Path.GetTempPath(), "NonExistentFolder");

            // Act
            fileCloner.CloneFolder(invalidSourcePath, _targetDirectory);

            // Assert
            Assert.IsFalse(Directory.Exists(_targetDirectory), "Target directory should not be created when source directory does not exist.");
        }

        // Test: Cloning should create a target directory if it doesn't exist
        [TestMethod]
        public void CloneFolder_ShouldCreateTargetDirectory()
        {
            // Arrange
            var fileCloner = new GitCloner.GitCloner();

            // Act
            fileCloner.CloneFolder(_sourceDirectory, _targetDirectory);

            // Assert
            Assert.IsTrue(Directory.Exists(_targetDirectory), "Target directory should be created if it doesn't exist.");
        }

        // Test: Ensure subdirectories are also cloned
        [TestMethod]
        public void CloneFolder_ShouldCopySubdirectories()
        {
            // Arrange
            var fileCloner = new GitCloner.GitCloner();
            string subDirectory = Path.Combine(_sourceDirectory, "SubFolder");
            Directory.CreateDirectory(subDirectory);
            File.WriteAllText(Path.Combine(subDirectory, "SubFile.txt"), "Sub folder content.");

            // Act
            fileCloner.CloneFolder(_sourceDirectory, _targetDirectory);

            // Assert
            string targetSubDirectory = Path.Combine(_targetDirectory, "SubFolder");
            Assert.IsTrue(Directory.Exists(targetSubDirectory), "Subdirectory should be copied to the target directory.");
            Assert.IsTrue(File.Exists(Path.Combine(targetSubDirectory, "SubFile.txt")), "Sub file should be copied to the target directory.");
        }
    }
}