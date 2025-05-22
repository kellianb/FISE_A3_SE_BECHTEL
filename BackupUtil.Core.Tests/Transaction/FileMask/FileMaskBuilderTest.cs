using System.Text.Json;
using BackupUtil.Core.Transaction.FileMask;
using FluentAssertions;

namespace BackupUtil.Core.Tests.Transaction.FileMask;

[TestFixture]
[TestOf(typeof(FileMaskBuilder))]
public class FileMaskBuilderTest
{

    [Test]
    public void SerializeAndDeserializeFileMask_EmptyFileMask_ReturnsSameMask()
    {
        // Arrange
        Core.Transaction.FileMask.FileMask expectation = new();

        // Act
        string serialized = FileMaskBuilder.From(expectation).BuildSerialized();

        Core.Transaction.FileMask.FileMask deserialized = FileMaskBuilder.FromString(serialized).Build();

        // Assert
        deserialized.Should().BeEquivalentTo(expectation);
    }


    [Test]
    public void SerializeAndDeserializeFileMask_SampleFileMask_ReturnsSameMask()
    {
        // Arrange
        Core.Transaction.FileMask.FileMask expectation = new();

        // Act
        string serialized = FileMaskBuilder
            .From(expectation)
            // Only copy JSON files
            .AllowedExtensions([".json"], FileMaskEffect.Copy)
            // Only encrypt files larger than 100
            .MinFileSize(100, FileMaskEffect.Encrypt)
            .BuildSerialized();

        Core.Transaction.FileMask.FileMask deserialized = FileMaskBuilder.FromString(serialized).Build();

        // Assert
        deserialized.Should().BeEquivalentTo(expectation);
    }
}
