using BackupUtil.Core.Job;

namespace BackupUtil.Core.Tests.Job.Handler;

[TestFixture]
public class SingleFileHandlerTest
{
    [SetUp]
    public void Setup()
    {
        // Create temporary test directories
        _sourceFolder = Path.Combine(Path.GetTempPath(), "NUnitSourceTest_" + Guid.NewGuid());
        _destinationFolder = Path.Combine(Path.GetTempPath(), "NUnitDestTest_" + Guid.NewGuid());

        Directory.CreateDirectory(_sourceFolder);
        Directory.CreateDirectory(_destinationFolder);
    }

    private string _sourceFolder;
    private string _destinationFolder;

    [Test]
    public void CopyFile_ValidFile_CopiesSuccessfully()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourceFolder, "test.txt");
        string destFile = Path.Combine(_destinationFolder, "test.txt");
        const string fileContent = "Test content";

        File.WriteAllText(sourceFile, fileContent);

        Core.Job.Job job = new(sourceFile, destFile);

        // Act
        JobHandlerFactory.GetHandler(job).Run();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(File.Exists(destFile), Is.True, "Destination file should exist");
            Assert.That(File.ReadAllText(destFile), Is.EqualTo(fileContent), "File content should match");
        });
    }


    [Test]
    public void CopyFile_NonExistentSource_ThrowsFileNotFoundException()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourceFolder, "nonexistent.txt");
        string destFile = Path.Combine(_destinationFolder, "output.txt");

        Core.Job.Job job = new(sourceFile, destFile);

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => JobHandlerFactory.GetHandler(job).Run());
    }

    [Test]
    public void CopyFile_DestinationFileExists_OverwritesFile()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourceFolder, "test.txt");
        string destFile = Path.Combine(_destinationFolder, "test.txt");
        string initialContent = "Initial content";
        string newContent = "New content";

        File.WriteAllText(sourceFile, newContent);
        File.WriteAllText(destFile, initialContent);

        Core.Job.Job job = new(sourceFile, destFile);

        // Act
        JobHandlerFactory.GetHandler(job).Run();

        // Assert
        Assert.That(File.ReadAllText(destFile), Is.EqualTo(newContent), "File should be overwritten");
    }
}
