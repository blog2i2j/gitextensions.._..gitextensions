using System.Text;
using GitExtensions.Extensibility.Git;

namespace GitCommandsTests.Git
{
    [TestFixture]
    public class GitBlameCommitTests
    {
        [Test]
        public void TestToString()
        {
            DateTime committerTime = DateTime.Now;
            DateTime authorTime = DateTime.Now;
            ObjectId commitHash = ObjectId.Random();

            StringBuilder str = new();

            str.AppendLine("Author: Author");
            str.AppendLine($"Author date: {authorTime}");
            str.AppendLine("Committer: committer");
            str.AppendLine($"Commit date: {committerTime}");
            str.AppendLine($"Commit hash: {commitHash.ToShortString()}");
            str.AppendLine("Summary: test summary");
            str.AppendLine();
            str.Append("FileName: fileName.txt");

            GitBlameCommit commit = new(
                commitHash,
                "Author",
                "author@authormail.com",
                authorTime,
                "authorTimeZone",
                "committer",
                "committer@authormail.com",
                committerTime,
                "committerTimeZone",
                "test summary",
                "fileName.txt");

            ClassicAssert.AreEqual(str.ToString(), commit.ToString());
        }

        [Test]
        public void ToString_When_Not_Null_Returns_Output()
        {
            DateTime committerTime = DateTime.Now;
            DateTime authorTime = DateTime.Now;
            ObjectId commitHash = ObjectId.Random();

            Func<string?, string?> summaryBuilder = (input) => $"SOME BUILDER TEXT: {input}";

            StringBuilder str = new();

            str.AppendLine("Author: Author");
            str.AppendLine($"Author date: {authorTime}");
            str.AppendLine("Committer: committer");
            str.AppendLine($"Commit date: {committerTime}");
            str.AppendLine($"Commit hash: {commitHash.ToShortString()}");
            str.AppendLine("Summary: SOME BUILDER TEXT: test summary");
            str.AppendLine();
            str.Append("FileName: fileName.txt");

            GitBlameCommit commit = new(
                commitHash,
                "Author",
                "author@authormail.com",
                authorTime,
                "authorTimeZone",
                "committer",
                "committer@authormail.com",
                committerTime,
                "committerTimeZone",
                "test summary",
                "fileName.txt");

            ClassicAssert.AreEqual(str.ToString(), commit.ToString(summaryBuilder));
        }

        [Test]
        public void ToString_When_Null_Returns_Input()
        {
            DateTime committerTime = DateTime.Now;
            DateTime authorTime = DateTime.Now;
            ObjectId commitHash = ObjectId.Random();

            Func<string?, string?> summaryBuilder = (input) => null;

            StringBuilder str = new();

            str.AppendLine("Author: Author");
            str.AppendLine($"Author date: {authorTime}");
            str.AppendLine("Committer: committer");
            str.AppendLine($"Commit date: {committerTime}");
            str.AppendLine($"Commit hash: {commitHash.ToShortString()}");
            str.AppendLine("Summary: test summary");
            str.AppendLine();
            str.Append("FileName: fileName.txt");

            GitBlameCommit commit = new(
                commitHash,
                "Author",
                "author@authormail.com",
                authorTime,
                "authorTimeZone",
                "committer",
                "committer@authormail.com",
                committerTime,
                "committerTimeZone",
                "test summary",
                "fileName.txt");

            ClassicAssert.AreEqual(str.ToString(), commit.ToString(summaryBuilder));
        }
    }
}
