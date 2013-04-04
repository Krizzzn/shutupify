using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FluentAssertions;
using Shutupify.Settings;

namespace Shutupify.Unit
{
    
    [TestFixture]
    public class FileSettingsReaderTest
    {
        public FileSettingsReader Subject;
        public FileSettingsReader SubjectChoppedUp;
        public string TestData;

        [TestFixtureSetUp]
        public void GetSettingsReader() {
            TestData = System.IO.File.ReadAllText(@"data\FileSettingsReaderTestData");
            Subject = new FileSettingsReader(TestData);

            SubjectChoppedUp = new FileSettingsReader(System.IO.File.ReadAllText(@"data\FileSettingsReaderEdgyTestData"));
        }

        [Test]
        public void serialize_without_change() {
            var serialized = Subject.SerializeToString();
            serialized.Should().BeEquivalentTo(TestData);
        }
        
        [Test]
        public void can_read_line() {
            Subject["PlayerActivated"].Should().BeEquivalentTo("Yes");
        }

        [Test]
        public void key_matching_is_case_insensitive()
        {
            Subject["PlaYerAcTivAted"].Should().BeEquivalentTo("Yes");
        }

        [Test]
        public void cant_see_inline_comments()
        {
            Subject["PlayerPause"].Should().BeNullOrEmpty();
        }

        [Test]
        public void handles_any_key()
        {
            SubjectChoppedUp["any key that does not exist"].Should().BeNullOrEmpty();
        }

        [Test]
        public void doesnt_return_commented_line()
        {
            Subject["ProbeSensitivity"].Should().Be("0.6");
        }

        [Test]
        public void handles_duplicate_keys()
        {
            SubjectChoppedUp["PlayerActivated"].Should().BeEquivalentTo("Yes");
        }

        [Test]
        public void no_whitespace()
        {
            SubjectChoppedUp["Loud"].Should().BeEquivalentTo("No");
        }

        [Test]
        public void whitespace_in_value()
        {
            SubjectChoppedUp["Brat"].Should().BeEquivalentTo("122 12	150		1231");
        }

        [Test]
        public void whitespace_in_key()
        {
            SubjectChoppedUp["Fe ed"].Should().BeEquivalentTo("100");
        }
    }
}
