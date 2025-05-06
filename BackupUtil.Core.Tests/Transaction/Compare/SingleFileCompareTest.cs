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
            foreach (string file in Directory.GetFiles(_targetFolder))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }

            Directory.Delete(_targetFolder, true);
        }
    }

    private static IEnumerable<TestCaseData> DiffAndFullTestCases()
    {
        yield return new TestCaseData(true).SetName("Test_Diff");
        yield return new TestCaseData(false).SetName("Test_Full");
    }

    [TestCaseSource(nameof(DiffAndFullTestCases))]
    public void Compare_WhenSourceFileExists_ShouldCreateFile(bool differential)
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

    [TestCaseSource(nameof(DiffAndFullTestCases))]
    public void Compare_WhenSourceFileExists_ShouldCreateFileAndDirectory(bool differential)
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
}
