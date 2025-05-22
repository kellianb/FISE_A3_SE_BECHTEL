using BackupUtil.Core.Job;
using FluentAssertions;

namespace BackupUtil.Core.Tests.Job;

public class JobManagerTest
{
    private string _sourceFolder;
    private string _targetFolder;

    [SetUp]
    public void Setup()
    {
        // Create temporary directories for testing
        _sourceFolder = Path.Combine(Path.GetTempPath(), "NUnitSourceTest_" + Guid.NewGuid());
        _targetFolder = Path.Combine(Path.GetTempPath(), "NUnitDestTest_" + Guid.NewGuid());

        Directory.CreateDirectory(_sourceFolder);
        Directory.CreateDirectory(_targetFolder);
    }

    [TearDown]
    public void Cleanup()
    {
        // Force garbage collection to release file handles
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Clean up test directories
        if (Directory.Exists(_sourceFolder))
        {
            foreach (string file in Directory.GetFiles(_sourceFolder))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }

            Directory.Delete(_sourceFolder, true);
        }

        if (Directory.Exists(_targetFolder))
        {
            // Remove ReadOnly attributes
            foreach (string file in Directory.GetFiles(_targetFolder))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }

            Directory.Delete(_targetFolder, true);
        }
    }

    [Test]
    public void AddJobsFromFile_FileDoesNotExist_ShouldThrowAndReturnEmptyList()
    {
        // Arrange
        string jobFilePath = Path.Combine(_sourceFolder, "jobs.json");

        JobManager manager = new();

        // Act and Assert
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentException>(() => manager.AddJobsFromFile(jobFilePath));
            Assert.That(manager.Jobs, Is.Empty, "Job list should be empty");
        });
    }


    [Test]
    public void AddJobsFromFile_FileExistsButInvalid_ShouldThrowAndReturnEmptyList()
    {
        // Arrange
        const string fileContent = "abc";

        string jobFilePath = Path.Combine(_sourceFolder, "jobs.json");

        File.WriteAllText(jobFilePath, fileContent);

        JobManager manager = new();

        // Act and Assert
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentException>(() => manager.AddJobsFromFile(jobFilePath));
            Assert.That(manager.Jobs, Is.Empty, "Job list should be empty");
        });
    }

    [Test]
    public void AddJobsFromFile_FileExistsAndValid_ShouldLoadJob()
    {
        // Arrange
        const string fileContent = """
                                           [
                                             {
                                               "Name": "Hello World",
                                               "SourcePath": "/tmp/test/1",
                                               "TargetPath": "/tmp/test/2",
                                               "Recursive": false,
                                               "Differential": true,
                                               "EncryptionKey": "abc"
                                             }
                                           ]
                                   """;

        Core.Job.Job expectedJob = new(
            "/tmp/test/1",
            "/tmp/test/2",
            false,
            true,
            "Hello World",
            "abc");

        string jobFilePath = Path.Combine(_sourceFolder, "jobs.json");

        File.WriteAllText(jobFilePath, fileContent);

        JobManager manager = new();

        // Act
        manager.AddJobsFromFile(jobFilePath);

        // Assert
        manager.Jobs.Should().BeEquivalentTo([expectedJob]);
    }
}
