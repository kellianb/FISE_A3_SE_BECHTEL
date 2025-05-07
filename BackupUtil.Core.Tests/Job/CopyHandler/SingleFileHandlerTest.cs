using System.Runtime.InteropServices;
using BackupUtil.Core.Job.CopyHandler;

namespace BackupUtil.Core.Tests.Job.CopyHandler;

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
            foreach (string file in Directory.GetFiles(_sourceFolder))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }

            Directory.Delete(_sourceFolder, true);
        }

        if (Directory.Exists(_destinationFolder))
        {
            foreach (string file in Directory.GetFiles(_destinationFolder))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }

            Directory.Delete(_destinationFolder, true);
        }
    }

    private string _sourceFolder;
    private string _destinationFolder;

    [Test]
    public void Run_ValidFile_CopiesSuccessfully()
    {
        const string fileContent = "Test content";

        // Arrange
        string sourceFile = Path.Combine(_sourceFolder, "test.txt");
        string destFile = Path.Combine(_destinationFolder, "test.txt");

        File.WriteAllText(sourceFile, fileContent);

        SingleFileHandler handler = new(new FileInfo(sourceFile), destFile);

        // Act
        handler.Run();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(File.Exists(destFile), Is.True, "Destination file should exist");
            Assert.That(File.ReadAllText(destFile), Is.EqualTo(fileContent), "File content should match");
        });
    }


    [Test]
    public void Run_NonExistentSource_ThrowsFileNotFoundException()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourceFolder, "nonexistent.txt");
        string destFile = Path.Combine(_destinationFolder, "output.txt");

        SingleFileHandler handler = new(new FileInfo(sourceFile), destFile);

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => handler.Run());
    }

    [Test]
    public void Run_DestinationFileExists_OverwritesFile()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourceFolder, "test.txt");
        string destFile = Path.Combine(_destinationFolder, "test.txt");
        const string initialContent = "Initial content";
        const string newContent = "New content";

        File.WriteAllText(sourceFile, newContent);
        File.WriteAllText(destFile, initialContent);

        SingleFileHandler handler = new(new FileInfo(sourceFile), destFile);

        // Act
        handler.Run();

        // Assert
        Assert.That(File.ReadAllText(destFile), Is.EqualTo(newContent), "File should be overwritten");
    }

    [Test]
    public void Run_DestinationFolderDoesNotExist_CreatesFolder()
    {
        const string fileContent = "Test content";

        // Arrange
        string sourceFile = Path.Combine(_sourceFolder, "test.txt");
        string newFolder = Path.Combine(_destinationFolder, "newfolder");
        string destFile = Path.Combine(newFolder, "test.txt");

        File.WriteAllText(sourceFile, fileContent);

        SingleFileHandler handler = new(new FileInfo(sourceFile), destFile);

        // Act
        handler.Run();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(Directory.Exists(newFolder), Is.True, "Destination folder should be created");
            Assert.That(File.Exists(destFile), Is.True, "Destination file should exist");
            Assert.That(File.ReadAllText(destFile), Is.EqualTo(fileContent), "File content should match");
        });
    }

    [Test]
    public void Run_PreservesAttributes()
    {
        const string fileContent = "Test content";

        // Arrange
        string sourceFile = Path.Combine(_sourceFolder, "test.txt");
        string destFile = Path.Combine(_destinationFolder, "test.txt");
        File.WriteAllText(sourceFile, fileContent);

        // Set attributes on source file
        File.SetAttributes(sourceFile, FileAttributes.Hidden | FileAttributes.ReadOnly);

        SingleFileHandler handler = new(new FileInfo(sourceFile), destFile);

        // Act
        handler.Run();

        // Assert
        FileAttributes destAttributes = File.GetAttributes(destFile);

        // Check "ReadOnly" attribute on all platforms
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
