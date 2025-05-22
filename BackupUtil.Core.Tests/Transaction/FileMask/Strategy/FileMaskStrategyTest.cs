using System.Text.Json;
using BackupUtil.Core.Transaction.FileMask.Strategy;

namespace BackupUtil.Core.Tests.Transaction.FileMask.Strategy;

public class FileMaskStrategyTest
{
    [Test]
    public void DeserializeFileMask_ValidJson_ReturnsMaxSizeFileMask()
    {
        // Arrange
        const string fileMask = """{"$type": "MaxSize", "MaxSize": 100}""";

        // Act
        FileMaskingStrategy? strategy = JsonSerializer.Deserialize<FileMaskingStrategy>(fileMask);

        Assert.Multiple(() =>
            {
                Assert.That(strategy, Is.Not.Null);
                Assert.That(strategy, Is.TypeOf<MaxFileSizeStrategy>());
                Assert.That(((MaxFileSizeStrategy)strategy!).MaxSize, Is.EqualTo(100));
            }
        );
    }

    [Test]
    public void DeserializeFileMask_ValidJson_ReturnsMinSizeFileMask()
    {
        // Arrange
        const string fileMask = """{"$type": "MinSize", "MinSize": 100}""";

        // Act
        FileMaskingStrategy? strategy = JsonSerializer.Deserialize<FileMaskingStrategy>(fileMask);

        Assert.Multiple(() =>
            {
                Assert.That(strategy, Is.Not.Null);
                Assert.That(strategy, Is.TypeOf<MinFileSizeStrategy>());
                Assert.That(((MinFileSizeStrategy)strategy!).MinSize, Is.EqualTo(100));
            }
        );
    }

    [Test]
    public void DeserializeFileMask_ValidJson_ReturnsAllowedExtensionsFileMask()
    {
        // Arrange
        const string fileMask = """{"$type": "AllowedExtensions", "AllowedExtensions": [".json", ".txt"]}""";

        string[] expected = [".json", ".txt"];

        // Act
        FileMaskingStrategy? strategy = JsonSerializer.Deserialize<FileMaskingStrategy>(fileMask);

        Assert.Multiple(() =>
            {
                Assert.That(strategy, Is.Not.Null);
                Assert.That(strategy, Is.TypeOf<AllowedFileExtensionsStrategy>());
                Assert.That(((AllowedFileExtensionsStrategy)strategy!).AllowedExtensions, Is.EquivalentTo(expected));
            }
        );
    }

    [Test]
    public void DeserializeFileMask_ValidJson_ReturnsBannedExtensionsFileMask()
    {
        // Arrange
        const string fileMask = """{"$type": "BannedExtensions", "BannedExtensions": [".json", ".txt"]}""";

        string[] expected = [".json", ".txt"];

        // Act
        FileMaskingStrategy? strategy = JsonSerializer.Deserialize<FileMaskingStrategy>(fileMask);

        Assert.Multiple(() =>
            {
                Assert.That(strategy, Is.Not.Null);
                Assert.That(strategy, Is.TypeOf<BannedFileExtensionsStrategy>());
                Assert.That(((BannedFileExtensionsStrategy)strategy!).BannedExtensions, Is.EquivalentTo(expected));
            }
        );
    }
}
