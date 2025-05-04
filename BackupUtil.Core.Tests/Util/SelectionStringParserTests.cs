using BackupUtil.Core.Util;

namespace BackupUtil.Core.Tests.Util;

[TestFixture]
public class SelectionStringParserTests
{
    [TestCaseSource(nameof(TestCases))]
    public void Parse_WhenGivenInput_ShouldReturnExpectedResult(string input, IEnumerable<int> expected)
    {
        // Act
        HashSet<int> result = SelectionStringParser.Parse(input);

        // Assert
        Assert.That(result, Is.EquivalentTo(expected));
    }

    private static IEnumerable<TestCaseData> TestCases()
    {
        // Valid inputs
        yield return new TestCaseData("1", new[] { 1 }).SetName("Parse_SingleNumber");
        yield return new TestCaseData("15", new[] { 15 }).SetName("Parse_SingleLargerNumber");
        yield return new TestCaseData("1 - 3", new[] { 1, 2, 3 }).SetName("Parse_SimpleRange");
        yield return new TestCaseData("1 - 1", new[] { 1 }).SetName("Parse_RangeWithSameNumber");
        yield return new TestCaseData("1 - 4, 6", new[] { 1, 2, 3, 4, 6 }).SetName("Parse_RangeWithSingleNumber");
        yield return new TestCaseData("6, 1 - 4", new[] { 1, 2, 3, 4, 6 }).SetName("Parse_SingleNumberWithRange");
        yield return new TestCaseData("15, 6, 1 - 4", new[] { 1, 2, 3, 4, 6, 15 }).SetName(
            "Parse_MultipleNumbersWithRange");
        yield return new TestCaseData("5 - 1", new[] { 5 }).SetName("Parse_InvertedRange_ReturnsOnlyFirst");

        // Error cases
        yield return new TestCaseData("-15, 6, 1 - 4", new[] { 1, 2, 3, 4, 6 }).SetName("Parse_NegativeNumber_Ignored");
        yield return new TestCaseData("1 -, 6", new[] { 6 }).SetName("Parse_InvalidRange_Ignored");
        yield return new TestCaseData("0.1", Array.Empty<int>()).SetName("Parse_DecimalNumber_ReturnsEmpty");
        yield return new TestCaseData("Hello World", Array.Empty<int>()).SetName("Parse_NonNumericString_ReturnsEmpty");
    }

    [Test]
    public void Parse_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SelectionStringParser.Parse(null));
    }

    [Test]
    public void Parse_EmptyString_ReturnsEmptySet()
    {
        HashSet<int> result = SelectionStringParser.Parse(string.Empty);

        Assert.That(result, Is.Empty);
    }
}
