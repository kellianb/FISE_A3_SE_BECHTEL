using System.Runtime.InteropServices;
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

    [TearDown]
    public void TearDown()
    {
        // Clean up test directories
        if (Directory.Exists(_sourceFolder))
        {
            Directory.Delete(_sourceFolder, true);
        }

        if (Directory.Exists(_destinationFolder))
        {
            Directory.Delete(_destinationFolder, true);
        }
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


    [Test]
    public void Job_DestinationFolderDoesNotExist_CreatesFolder()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourceFolder, "test.txt");
        string newFolder = Path.Combine(_destinationFolder, "newfolder");
        string destFile = Path.Combine(newFolder, "test.txt");
        string fileContent = "Test content";

        File.WriteAllText(sourceFile, fileContent);

        Core.Job.Job job = new(sourceFile, destFile);

        // Act
        JobHandlerFactory.GetHandler(job).Run();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(Directory.Exists(newFolder), Is.True, "Destination folder should be created");
            Assert.That(File.Exists(destFile), Is.True, "Destination file should exist");
            Assert.That(File.ReadAllText(destFile), Is.EqualTo(fileContent), "File content should match");
        });
    }

    [Test]
    public void Job_PreservesAttributes()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourceFolder, "test.txt");
        string destFile = Path.Combine(_destinationFolder, "test.txt");
        File.WriteAllText(sourceFile, "Test content");

        // Set attributes on source file
        File.SetAttributes(sourceFile, FileAttributes.Hidden | FileAttributes.ReadOnly);

        Core.Job.Job job = new(sourceFile, destFile);

        // Act
        JobHandlerFactory.GetHandler(job).Run();

        // Assert
        FileAttributes destAttributes = File.GetAttributes(destFile);
        // Check ReadOnly attribute on all platforms
        Assert.That(destAttributes.HasFlag(FileAttributes.ReadOnly), Is.True,
            "ReadOnly attribute should be preserved");

        // Only check Hidden attribute on Windows
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Assert.That(destAttributes.HasFlag(FileAttributes.Hidden), Is.True,
                "Hidden attribute should be preserved");
        }
        else
        {
            // Skip hidden attribute check on non-Windows platforms or log it
            TestContext.Out.WriteLine("Hidden attribute test skipped on non-Windows platform");
        }
    }
}
