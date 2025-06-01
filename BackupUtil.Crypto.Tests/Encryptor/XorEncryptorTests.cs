using BackupUtil.Crypto.Encryptor;

namespace BackupUtil.Crypto.Tests.Encryptor;

public class XorEncryptorTests
{
    [SetUp]
    public void Setup()
    {
    }

    private static IEnumerable<TestCaseData> TestCases()
    {
        yield return new TestCaseData("abc", "abc", "\0\0\0").SetName("Xor_KeyIdenticalToString_1");
        yield return new TestCaseData("abcd", "abcd", "\0\0\0\0").SetName("Xor_KeyIdenticalToString_2");

        yield return new TestCaseData("ABC", "123", "ppp").SetName("Xor_UppercaseWithNumbers");

        yield return new TestCaseData("", "key", "").SetName("Xor_EmptyString");

        yield return new TestCaseData("a", "abc", "\0").SetName("Xor_KeyLongerThanInput");

        yield return new TestCaseData("abcdef", "xy", "\u0019\u001b\u001b\u001d\u001d\u001f").SetName(
            "Xor_KeyShorterThanInput");

        yield return new TestCaseData("!@#$", "key", "J%ZO").SetName("Xor_SpecialCharactersInput");

        yield return new TestCaseData("test", "!@#$", "U%PP").SetName("Xor_SpecialCharactersKey");

        yield return new TestCaseData("αβγ", "123", "΀΀΀").SetName("Xor_UnicodeChars");
    }

    [TestCaseSource(nameof(TestCases))]
    public void Encrypt_WhenGivenInput_ShouldReturnExpectedResult(string input, string key, string expected)
    {
        XorEncryptor encryptor = new(key);

        // Act
        // Encrypt string
        string result1 = encryptor.Encrypt(input);

        // Decrypt string
        string result2 = encryptor.Encrypt(result1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.EqualTo(expected));
            Assert.That(result2, Is.EqualTo(input));
        });
    }
}
