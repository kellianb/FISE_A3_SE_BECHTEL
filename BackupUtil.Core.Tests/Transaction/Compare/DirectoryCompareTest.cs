using BackupUtil.Core.Transaction;
using BackupUtil.Core.Transaction.ChangeType;
using BackupUtil.Core.Transaction.Compare;
using BackupUtil.Core.Transaction.Editor;
using BackupUtil.Core.Transaction.FileMask;
using BackupUtil.Core.Util;

namespace BackupUtil.Core.Tests.Transaction.Compare;

public class DirectoryCompareTest
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

    #region Target directory does not exist

    [TestCase(true, TestName = "Test_Diff", Description = "Test differential compare")]
    [TestCase(false, TestName = "Test_Full", Description = "Test non differential compare")]
    public void Compare_TargetDirectoryDoesNotExist_ShouldCreateFileAndDirectory(bool differential)
    {
        const string fileContent = "Test content";

        // Arrange
        string sourceFilePath = Path.Combine(_sourceFolder, "test.txt");
        string targetSubFolder = Path.Combine(_targetFolder, "testFolder");
        string targetFilePath = Path.Combine(targetSubFolder, "test.txt");

        File.WriteAllText(sourceFilePath, fileContent);

        // Expected file change
        FileChange expectedFileChange =
            FileChange.Creation(sourceFilePath, targetFilePath, new FileInfo(sourceFilePath).Length, null, effects: FileMaskEffect.Copy);

        // Expected folder change
        DirectoryChange expectedDirectoryChange = DirectoryChange.Creation(targetSubFolder);

        // Create compare object
        DirectoryCompare compare = new(new DirectoryInfo(_sourceFolder), targetSubFolder, false, differential,
            new FileCompare());

        // Act
        BackupTransaction transaction = compare.Compare(BackupTransactionEditor.New()).Get();

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

    #endregion

    #region Target directory exists

    [Test]
    public void CompareDiff_TargetDirectoryExists_ShouldCreateFile()
    {
        const string fileContent = "Test content";

        // Arrange
        string sourceFilePath = Path.Combine(_sourceFolder, "test.txt");
        string targetFilePath = Path.Combine(_targetFolder, "test.txt");

        File.WriteAllText(sourceFilePath, fileContent);

        // Expected file change
        FileChange expectedFileChange =
            FileChange.Creation(sourceFilePath, targetFilePath, new FileInfo(sourceFilePath).Length, null, effects: FileMaskEffect.Copy);

        // Create compare object
        DirectoryCompare compare = new(new DirectoryInfo(_sourceFolder), _targetFolder, false, true,
            new FileCompare());

        // Act
        BackupTransaction transaction = compare.Compare(BackupTransactionEditor.New()).Get();

        // Assert
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
    public void CompareFull_TargetDirectoryExists_ShouldCreateFileAndDirectory()
    {
        const string fileContent = "Test content";

        // Arrange
        string sourceFilePath = Path.Combine(_sourceFolder, "test.txt");
        string targetFilePath = Path.Combine(_targetFolder, "test.txt");

        File.WriteAllText(sourceFilePath, fileContent);

        // Expected file change
        FileChange expectedFileChange =
            FileChange.Creation(sourceFilePath, targetFilePath, new FileInfo(sourceFilePath).Length, null, effects: FileMaskEffect.Copy);

        // Expected folder change
        DirectoryChange expectedDirectoryChange = DirectoryChange.Creation(_targetFolder);

        // Create compare object
        DirectoryCompare compare = new(new DirectoryInfo(_sourceFolder), _targetFolder, false, false,
            new FileCompare());

        // Act
        BackupTransaction transaction = compare.Compare(BackupTransactionEditor.New()).Get();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(transaction.FileChanges, Has.Count.EqualTo(1),
                "Transaction should contain exactly one file change");

            Assert.That(transaction.FileChanges[0], Is.EqualTo(expectedFileChange),
                "File change should match the expected value");

            Assert.That(transaction.DirectoryChanges, Has.Count.EqualTo(1),
                "Transaction should contain exactly one directory change");

            Assert.That(transaction.DirectoryChanges[0], Is.EqualTo(expectedDirectoryChange),
                "Directory change should match the expected value");
        });
    }

    #endregion

    #region Target file exists, but is different from source file

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
        FileChange expectedFileChange =
            FileChange.Modification(sourceFilePath, targetFilePath, new FileInfo(sourceFilePath).Length, null, effects: FileMaskEffect.Copy);

        // Create compare object
        DirectoryCompare compare = new(new DirectoryInfo(_sourceFolder), _targetFolder, false, true,
            new FileCompare());

        // Act
        BackupTransaction transaction = compare.Compare(BackupTransactionEditor.New()).Get();

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
    public void CompareFull_TargetFileExists_ShouldCreateFileAndDirectory()
    {
        // Arrange
        const string initialContent = "Initial content";
        const string newContent = "New content";

        string sourceFilePath = Path.Combine(_sourceFolder, "test.txt");
        string targetFilePath = Path.Combine(_targetFolder, "test.txt");

        File.WriteAllText(sourceFilePath, initialContent);
        File.WriteAllText(targetFilePath, newContent);

        // Expected file change
        FileChange expectedFileChange =
            FileChange.Creation(sourceFilePath, targetFilePath, new FileInfo(sourceFilePath).Length, null, effects: FileMaskEffect.Copy);

        // Expected folder change
        DirectoryChange expectedDirectoryChange = DirectoryChange.Creation(_targetFolder);

        // Create compare object
        DirectoryCompare compare = new(new DirectoryInfo(_sourceFolder), _targetFolder, false, false,
            new FileCompare());

        // Act
        BackupTransaction transaction = compare.Compare(BackupTransactionEditor.New()).Get();

        Assert.Multiple(() =>
        {
            Assert.That(transaction.FileChanges, Has.Count.EqualTo(1),
                "Transaction should contain exactly one file change");

            Assert.That(transaction.FileChanges[0], Is.EqualTo(expectedFileChange),
                "File change should match the expected value");

            Assert.That(transaction.DirectoryChanges, Has.Count.EqualTo(1),
                "Transaction should contain exactly one directory change");

            Assert.That(transaction.DirectoryChanges[0], Is.EqualTo(expectedDirectoryChange),
                "Directory change should match the expected value");
        });
    }

    #endregion

    #region Target file exists and is identical to source file

    [Test]
    public void CompareDiff_IdenticalTargetFileExists_ShouldDoNothing()
    {
        // Arrange
        const string fileContent = "New content";

        string sourceFilePath = Path.Combine(_sourceFolder, "test.txt");
        string targetFilePath = Path.Combine(_targetFolder, "test.txt");

        File.WriteAllText(sourceFilePath, fileContent);
        File.WriteAllText(targetFilePath, fileContent);

        // Create compare object
        DirectoryCompare compare = new(new DirectoryInfo(_sourceFolder), _targetFolder, false, true,
            new FileCompare());

        // Act
        BackupTransaction transaction = compare.Compare(BackupTransactionEditor.New()).Get();

        Assert.Multiple(() =>
        {
            Assert.That(transaction.FileChanges, Has.Count.EqualTo(0),
                "Transaction should contain exactly no file changes");

            Assert.That(transaction.DirectoryChanges, Has.Count.EqualTo(0),
                "Transaction should contain exactly no directory changes");
        });
    }

    [Test]
    public void CompareFull_IdenticalTargetFileExists_ShouldCreateFile()
    {
        // Arrange
        const string fileContent = "New content";

        string sourceFilePath = Path.Combine(_sourceFolder, "test.txt");
        string targetFilePath = Path.Combine(_targetFolder, "test.txt");

        File.WriteAllText(sourceFilePath, fileContent);
        File.WriteAllText(targetFilePath, fileContent);

        // Expected file change
        FileChange expectedFileChange =
            FileChange.Creation(sourceFilePath, targetFilePath, new FileInfo(sourceFilePath).Length, null, effects: FileMaskEffect.Copy);

        // Expected folder change
        DirectoryChange expectedDirectoryChange = DirectoryChange.Creation(_targetFolder);

        // Create compare object
        DirectoryCompare compare = new(new DirectoryInfo(_sourceFolder), _targetFolder, false, false,
            new FileCompare());

        // Act
        BackupTransaction transaction = compare.Compare(BackupTransactionEditor.New()).Get();

        Assert.Multiple(() =>
        {
            Assert.That(transaction.FileChanges, Has.Count.EqualTo(1),
                "Transaction should contain exactly one file change");

            Assert.That(transaction.FileChanges[0], Is.EqualTo(expectedFileChange),
                "File change should match the expected value");

            Assert.That(transaction.DirectoryChanges, Has.Count.EqualTo(1),
                "Transaction should contain exactly one directory change");

            Assert.That(transaction.DirectoryChanges[0], Is.EqualTo(expectedDirectoryChange),
                "Directory change should match the expected value");
        });
    }

    #endregion
}
