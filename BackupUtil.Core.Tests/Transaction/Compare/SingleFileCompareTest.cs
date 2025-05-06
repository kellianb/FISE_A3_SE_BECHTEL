using BackupUtil.Core.Transaction;
using BackupUtil.Core.Transaction.ChangeType;
using BackupUtil.Core.Transaction.Compare;

namespace BackupUtil.Core.Tests.Transaction.Compare;

public class SingleFileCompareTest
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

    [TestCase(true, TestName = "Test_Diff", Description = "Test differential compare")]
    [TestCase(false, TestName = "Test_Full", Description = "Test non differential compare")]
    public void Compare_TargetDirectoryExists_ShouldCreateFile(bool differential)
    {
        const string fileContent = "Test content";

        // Arrange
        string sourceFilePath = Path.Combine(_sourceFolder, "test.txt");
        string targetFilePath = Path.Combine(_targetFolder, "test.txt");

        File.WriteAllText(sourceFilePath, fileContent);

        // Expected file change
        FileChange expectedFileChange = new(targetFilePath, FileChangeType.Create, sourceFilePath,
            new FileInfo(sourceFilePath).Length);

        // Create compare object
        SingleFileCompare compare = new(new FileInfo(sourceFilePath), targetFilePath, differential);

        // Act
        BackupTransaction transaction = compare.Compare();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(transaction.GetTotalCreatedFiles(), Is.EqualTo(1),
                "Transaction should contain exactly one created file");

            Assert.That(transaction.FileChanges, Has.Count.EqualTo(1),
                "Transaction should contain exactly one file change");

            Assert.That(transaction.FileChanges[0], Is.EqualTo(expectedFileChange),
                "File change does not match expected value");
        });
    }

    [TestCase(true, TestName = "Test_Diff", Description = "Test differential compare")]
    [TestCase(false, TestName = "Test_Full", Description = "Test non differential compare")]
    public void Compare_TargetDirectoryDoesNotExists_ShouldCreateFileAndDirectory(bool differential)
    {
        const string fileContent = "Test content";

        // Arrange
        string sourceFilePath = Path.Combine(_sourceFolder, "test.txt");
        string targetSubFolder = Path.Combine(_targetFolder, "testFolder");
        string targetFilePath = Path.Combine(targetSubFolder, "test.txt");

        File.WriteAllText(sourceFilePath, fileContent);

        // Expected file change
        FileChange expectedFileChange = new(targetFilePath, FileChangeType.Create, sourceFilePath,
            new FileInfo(sourceFilePath).Length);

        // Expected folder change
        DirectoryChange expectedDirectoryChange = new(targetSubFolder, DirectoryChangeType.Create);

        // Create compare object
        SingleFileCompare compare = new(new FileInfo(sourceFilePath), targetFilePath, differential);

        // Act
        BackupTransaction transaction = compare.Compare();

        // Assert
        Assert.Multiple(() =>
        {
            // Check files
            Assert.That(transaction.FileChanges, Has.Count.EqualTo(1),
                "Transaction should contain exactly one file change");

            Assert.That(transaction.FileChanges[0], Is.EqualTo(expectedFileChange),
                "File change should match the expected value");

            Assert.That(transaction.DirectoryChanges, Has.Count.EqualTo(1),
                "Transaction should contain exactly one directory change");

            Assert.That(transaction.DirectoryChanges[0], Is.EqualTo(expectedDirectoryChange),
                "Directory should match the expected value");
        });
    }

    [Test]
    public void CompareDiff_TargetFileExists_ShouldUpdateFile()
    {
        // Arrange
        const string newContent = "New content";
        const string oldContent = "Old content";

        string sourceFilePath = Path.Combine(_sourceFolder, "test.txt");
        string targetFilePath = Path.Combine(_targetFolder, "test.txt");

        File.WriteAllText(sourceFilePath, newContent);
        File.WriteAllText(targetFilePath, oldContent);

        // Expected file change
        FileChange expectedFileChange = new(targetFilePath, FileChangeType.Modify, sourceFilePath,
            new FileInfo(sourceFilePath).Length);

        // Create compare object
        SingleFileCompare compare = new(new FileInfo(sourceFilePath), targetFilePath, true);

        // Act
        BackupTransaction transaction = compare.Compare();

        Assert.Multiple(() =>
        {
            Assert.That(transaction.FileChanges, Has.Count.EqualTo(1),
                "Transaction should contain exactly one file change");

            Assert.That(transaction.FileChanges[0], Is.EqualTo(expectedFileChange),
                "File change should match the expected value");

            Assert.That(transaction.DirectoryChanges, Has.Count.EqualTo(0),
                "Transaction should contain exactly no directory changes");
        });
    }

    [Test]
    public void CompareDiff_IdenticalTargetFileExists_ShouldUpdateFile()
    {
        // Arrange
        const string fileContent = "New content";

        string sourceFilePath = Path.Combine(_sourceFolder, "test.txt");
        string targetFilePath = Path.Combine(_targetFolder, "test.txt");

        File.WriteAllText(sourceFilePath, fileContent);
        File.WriteAllText(targetFilePath, fileContent);

        // Create compare object
        SingleFileCompare compare = new(new FileInfo(sourceFilePath), targetFilePath, true);

        // Act
        BackupTransaction transaction = compare.Compare();

        Assert.Multiple(() =>
        {
            Assert.That(transaction.FileChanges, Has.Count.EqualTo(0),
                "Transaction should contain exactly no file changes");


            Assert.That(transaction.DirectoryChanges, Has.Count.EqualTo(0),
                "Transaction should contain exactly no directory changes");
        });
    }

    [Test]
    public void CompareFull_TargetFileExists_ShouldCreateFile()
    {
        // Arrange
        const string initialContent = "Initial content";
        const string newContent = "New content";

        string sourceFilePath = Path.Combine(_sourceFolder, "test.txt");
        string targetFilePath = Path.Combine(_targetFolder, "test.txt");

        File.WriteAllText(sourceFilePath, initialContent);
        File.WriteAllText(targetFilePath, newContent);

        // Expected file change
        FileChange expectedFileChange = new(targetFilePath, FileChangeType.Create, sourceFilePath,
            new FileInfo(sourceFilePath).Length);

        // Expected folder change
        DirectoryChange expectedDirectoryChange = new(_targetFolder, DirectoryChangeType.Create);

        // Create compare object
        SingleFileCompare compare = new(new FileInfo(sourceFilePath), targetFilePath, false);

        // Act
        BackupTransaction transaction = compare.Compare();

        Assert.Multiple(() =>
        {
            Assert.That(transaction.FileChanges, Has.Count.EqualTo(1),
                "Transaction should contain exactly one file change");

            Assert.That(transaction.FileChanges[0], Is.EqualTo(expectedFileChange),
                "File change should match the expected value");

            Assert.That(transaction.DirectoryChanges, Has.Count.EqualTo(1),
                "Transaction should contain exactly one directory change");

            Assert.That(transaction.DirectoryChanges[0], Is.EqualTo(expectedDirectoryChange),
                "Directory should match the expected value");
        });
    }
}
