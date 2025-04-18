﻿namespace GitCommandsTests
{
    [TestFixture]
    public sealed class StringExtensionTests
    {
        [TestCase(null, 10, "")]
        [TestCase("Hello", 10, "Hello")]
        [TestCase("Hello", 5, "Hello")]
        [TestCase("Hello", 4, "H...")]
        [TestCase("Hello", 3, "...")]
        [TestCase("Hello", 2, "He")]
        public void ShortenTo_works_as_expected(string s, int length, string expected)
        {
            ClassicAssert.AreEqual(expected, s.ShortenTo(length));
        }

        [TestCase("Hello World", "Hello", true)]
        [TestCase("Hello World", " ", true)]
        [TestCase("Hello World", "World", true)]
        [TestCase("Hello World", "", true)]
        [TestCase("Hello World", "Hi", false)]
        [TestCase("Hello World", "ldi", false)]
        public void Contains_works_as_expected(string str, string other, bool expected)
        {
            ClassicAssert.AreEqual(expected, str.Contains(other, StringComparison.InvariantCultureIgnoreCase));
        }

        [TestCase("ABCDEFG", "ABC", "DEFG")]
        [TestCase("ABCDEFG", "AB", "CDEFG")]
        [TestCase("ABCDEFG", "A", "BCDEFG")]
        [TestCase("ABCDEFG", "", "ABCDEFG")]
        [TestCase("ABCDEFG", "XYZ", "ABCDEFG")]
        [TestCase("AB", "ABCD", "AB")]
        public void RemovePrefix_works_as_expected(string str, string prefix, string expected)
        {
            ClassicAssert.AreEqual(expected, str.RemovePrefix(prefix));
        }

        [TestCase("ABCDEFG", "EFG", "ABCD")]
        [TestCase("ABCDEFG", "FG", "ABCDE")]
        [TestCase("ABCDEFG", "G", "ABCDEF")]
        [TestCase("ABCDEFG", "", "ABCDEFG")]
        [TestCase("ABCDEFG", "XYZ", "ABCDEFG")]
        [TestCase("CD", "ABCD", "CD")]
        [TestCase("", "ABCD", "")]
        [TestCase("ABCD", "", "ABCD")]
        public void RemoveSuffix_works_as_expected(string str, string prefix, string expected)
        {
            ClassicAssert.AreEqual(expected, str.RemoveSuffix(prefix));
        }

        [TestCase("ABCDEFG", 'A', "")]
        [TestCase("ABCDEFG", 'B', "A")]
        [TestCase("ABCDEFG", 'C', "AB")]
        [TestCase("ABCDEFG", 'D', "ABC")]
        [TestCase("ABCDEFG", 'E', "ABCD")]
        [TestCase("ABCDEFG", 'Z', "ABCDEFG")]
        [TestCase("", 'Z', "")]
        [TestCase("A", 'A', "")]
        public void SubstringUntil_works_as_expected(string str, char c, string expected)
        {
            ClassicAssert.AreEqual(expected, str.SubstringUntil(c));
        }

        [TestCase("ABCABC", 'A', "ABC")]
        [TestCase("ABCABC", 'B', "ABCA")]
        [TestCase("ABCABC", 'C', "ABCAB")]
        [TestCase("ABCABC", 'Z', "ABCABC")]
        [TestCase("", 'Z', "")]
        [TestCase("A", 'A', "")]
        [TestCase("AAAA", 'A', "AAA")]
        public void SubstringUntilLast_works_as_expected(string str, char c, string expected)
        {
            ClassicAssert.AreEqual(expected, str.SubstringUntilLast(c));
        }

        [TestCase("ABCDEFG", 'A', "BCDEFG")]
        [TestCase("ABCDEFG", 'B', "CDEFG")]
        [TestCase("ABCDEFG", 'C', "DEFG")]
        [TestCase("ABCDEFG", 'Z', "ABCDEFG")]
        [TestCase("", 'Z', "")]
        [TestCase("A", 'A', "")]
        [TestCase("ABBA", 'A', "BBA")]
        public void SubstringAfter_char_works_as_expected(string str, char c, string expected)
        {
            ClassicAssert.AreEqual(expected, str.SubstringAfter(c));
        }

        [TestCase("ABCDEFG", "A", "BCDEFG")]
        [TestCase("ABCDEFG", "AB", "CDEFG")]
        [TestCase("ABCDEFG", "B", "CDEFG")]
        [TestCase("ABCDEFG", "C", "DEFG")]
        [TestCase("ABCDEFG", "Z", "ABCDEFG")]
        [TestCase("", "Z", "")]
        [TestCase("A", "A", "")]
        [TestCase("ABBA", "A", "BBA")]
        public void SubstringAfter_string_works_as_expected(string str, string s, string expected)
        {
            ClassicAssert.AreEqual(expected, str.SubstringAfter(s));
        }

        [TestCase("ABCABC", 'A', "BC")]
        [TestCase("ABCABC", 'B', "C")]
        [TestCase("ABCABC", 'C', "")]
        [TestCase("ABCDEFG", 'A', "BCDEFG")]
        [TestCase("ABCDEFG", 'B', "CDEFG")]
        [TestCase("ABCDEFG", 'C', "DEFG")]
        [TestCase("ABCDEFG", 'Z', "ABCDEFG")]
        [TestCase("", 'Z', "")]
        [TestCase("A", 'A', "")]
        [TestCase("ABBA", 'A', "")]
        public void SubstringAfterLast_char_works_as_expected(string str, char c, string expected)
        {
            ClassicAssert.AreEqual(expected, str.SubstringAfterLast(c));
        }

        [TestCase("ABCABC", "A", "BC")]
        [TestCase("ABCABC", "B", "C")]
        [TestCase("ABCABC", "C", "")]
        [TestCase("ABCDEFG", "A", "BCDEFG")]
        [TestCase("ABCDEFG", "AB", "CDEFG")]
        [TestCase("ABCDEFG", "B", "CDEFG")]
        [TestCase("ABCDEFG", "BCD", "EFG")]
        [TestCase("ABCDEFG", "C", "DEFG")]
        [TestCase("ABCDEFG", "Z", "ABCDEFG")]
        [TestCase("", "Z", "")]
        [TestCase("A", "A", "")]
        [TestCase("ABBA", "A", "")]
        public void SubstringAfterLast_string_works_as_expected(string str, string s, string expected)
        {
            ClassicAssert.AreEqual(expected, str.SubstringAfterLast(s));
        }

        [TestCase("/usr/bin", false, "/usr/bin")]
        [TestCase("/usr/bin", true, "/usr/bin")]
        [TestCase("\\usr\\bin", false, "\\\\usr\\\\bin")]
        [TestCase("\\usr\\bin", true, "\\usr\\bin")]
        [TestCase("C:\\Program Files (x86)\\Microsoft Visual Studio\\Installer\\setup.exe", true, "C:\\Program Files (x86)\\Microsoft Visual Studio\\Installer\\setup.exe")]
        [TestCase("echo \"Hello world\"", false, "echo \\\"Hello world\\\"")]
        [TestCase("echo \"Hello world\"", true, "echo \"\"Hello world\"\"")]
        [TestCase("echo \'Hello world\'", false, "echo \\\'Hello world\\\'")]
        [TestCase("echo \'Hello world\'", true, "echo \'Hello world\'")]
        [TestCase("cmd /c \"echo \\\"Hello world\\\"\"", false, "cmd /c \\\"echo \\\\\\\"Hello world\\\\\\\"\\\"")]
        [TestCase("cmd /c \"echo \\\"Hello world\\\"\"", true, "cmd /c \"\"echo \\\"\"Hello world\\\"\"\"\"")]
        [TestCase("cmd /c \"echo \"\"Hello world\"\"\"", false, "cmd /c \\\"echo \\\"\\\"Hello world\\\"\\\"\\\"")]
        [TestCase("cmd /c \"echo \"\"Hello world\"\"\"", true, "cmd /c \"\"echo \"\"\"\"Hello world\"\"\"\"\"\"")]
        public void EscapeForCommandLine_works_as_expected(string s, bool forWindows, string expected)
        {
            ClassicAssert.AreEqual(expected, s.EscapeForCommandLine(forWindows));
        }

        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase(" \"", "\" \\\"\"")]
        [TestCase("\"\"", "\"\"")]
        [TestCase("file", "\"file\"")]
        [TestCase("\"file", "\"file")]
        [TestCase("\"file\"", "\"file\"")]
        [TestCase("file with spaces", "\"file with spaces\"")]
        [TestCase("\"file with spaces", "\"file with spaces")]
        [TestCase("\"file with spaces\"", "\"file with spaces\"")]
        [TestCase("wei\"rd", "\"wei\\\"rd\"")]
        [TestCase("\"wei\"rd\"", "\"wei\"rd\"")]
        [TestCase("weird\"", "\"weird\\\"\"")]

        public void QuoteIfUnquotedAndNE(string? str, string? expected)
        {
            ClassicAssert.AreEqual(expected, str.QuoteIfNotQuotedAndNE());
        }

        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("\"", "\"")]
        [TestCase("\"\"", "")]
        [TestCase("\" \"\"", " \"")]
        [TestCase("\" \\\"", " \\")]
        [TestCase("\" \\\"\"", " \\\"")]
        [TestCase("\"file\"", "file")]
        [TestCase("\"file", "\"file")]
        [TestCase("file\"", "file\"")]
        [TestCase("\"file with spaces\"", "file with spaces")]

        public void RemoveQuotes(string? str, string? expected)
        {
            ClassicAssert.AreEqual(expected, str.RemoveQuotes());
        }
    }
}
