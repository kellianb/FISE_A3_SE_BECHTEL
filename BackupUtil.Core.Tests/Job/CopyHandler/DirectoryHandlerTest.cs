using System.Runtime.InteropServices;
using BackupUtil.Core.Job.CopyHandler;

namespace BackupUtil.Core.Tests.Job.CopyHandler;

[TestFixture]
public class DirectoryHandlerTests
{
    [SetUp]
    public void Setup()
    {
        // Create temporary directories for testing
        _sourceFolder = Path.Combine(Path.GetTempPath(), "NUnitSourceTest_" + Guid.NewGuid());
        _destinationFolder = Path.Combine(Path.GetTempPath(), "NUnitDestTest_" + Guid.NewGuid());

        Directory.CreateDirectory(_sourceFolder);
    }

    [TearDown]
    public void Cleanup()
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
    public void Run_NonRecursiveCopy_CopiesOnlyTopLevelFiles()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourceFolder, "test.txt");
        string subDirectory = Path.Combine(_sourceFolder, "subdir");
        string subDirectoryFile = Path.Combine(subDirectory, "subtest.txt");

        Directory.CreateDirectory(subDirectory);
        File.WriteAllText(sourceFile, "test content");
        File.WriteAllText(subDirectoryFile, "subdir content");

        DirectoryHandler handler = new(
            new DirectoryInfo(_sourceFolder),
            _destinationFolder,
            false);

        // Act
        handler.Run();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(File.Exists(Path.Combine(_destinationFolder, "test.txt")), Is.True);
            Assert.That(File.Exists(Path.Combine(_destinationFolder, "subdir", "subtest.txt")), Is.False);
            Assert.That(File.ReadAllText(Path.Combine(_destinationFolder, "test.txt")), Is.EqualTo("test content"));
        });
    }

    [Test]
    public void Run_RecursiveCopy_CopiesAllFilesAndDirectories()
    {
        // Arrange
        Dictionary<string, string> testFiles = new()
        {
            { "test1.txt", "Test content 1" },
            { "test2.txt", "Test content 2" },
            { Path.Combine("subdir", "subtest1.txt"), "Subdirectory test content 1" },
            { Path.Combine("subdir", "subtest2.txt"), "Subdirectory test content 2" }
        };

        // Create test directory structure and files
        foreach ((string path, string content) in testFiles)
        {
            string fullPath = Path.Combine(_sourceFolder, path);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            File.WriteAllText(fullPath, content);
        }

        DirectoryHandler handler = new(
            new DirectoryInfo(_sourceFolder),
            _destinationFolder,
            true);

        // Act
        handler.Run();

        // Assert
        Assert.Multiple(() =>
        {
            foreach ((string path, string expectedContent) in testFiles)
            {
                string destinationPath = Path.Combine(_destinationFolder, path);
                Assert.That(File.Exists(destinationPath), Is.True, $"File {path} should exist in destination");
                Assert.That(File.ReadAllText(destinationPath), Is.EqualTo(expectedContent),
                    $"Content mismatch for file {path}");
            }

            // Verify directory structure
            Assert.That(Directory.Exists(Path.Combine(_destinationFolder, "subdir")), Is.True,
                "Subdirectory should exist in destination");
        });
    }

    [Test]
    public void Run_TargetDirectoryDoesNotExist_CreatesTargetDirectory()
    {
        // Arrange
        DirectoryHandler handler = new(
            new DirectoryInfo(_sourceFolder),
            _destinationFolder,
            false);

        Assert.That(Directory.Exists(_destinationFolder), Is.False);

        // Act
        handler.Run();

        // Assert
        Assert.That(Directory.Exists(_destinationFolder), Is.True);
    }

    [Test]
    public void Run_PreservesFileAttributes()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourceFolder, "test.txt");
        File.WriteAllText(sourceFile, "test content");
        File.SetAttributes(sourceFile, FileAttributes.Hidden | FileAttributes.ReadOnly);

        DirectoryHandler handler = new(
            new DirectoryInfo(_sourceFolder),
            _destinationFolder,
            false);

        // Act
        handler.Run();

        // Assert
        FileAttributes destAttributes = File.GetAttributes(Path.Combine(_destinationFolder, "test.txt"));

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
            TestContext.Out.WriteLine("Readonly attribute test skipped on non-Windows platform");
        }
    }
}
