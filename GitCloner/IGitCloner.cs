namespace GitCloner
{

    public interface IGitCloner
    {
        public CloneStatus CloneFolder(string sourcePath, string targetPath);

    }
}
