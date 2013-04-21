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
    public class SettingsReaderTest
    {
        public SettingsReader Subject;
        public SettingsReader SubjectChoppedUp;
        public string TestData;

        [TestFixtureSetUp]
        public void GetSettingsReader() {
            TestData = System.IO.File.ReadAllText(@"data\FileSettingsReaderTestData");
            Subject = new SettingsReader(TestData);

            SubjectChoppedUp = new SettingsReader(System.IO.File.ReadAllText(@"data\FileSettingsReaderEdgyTestData"));
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

        [Test]
        public void list_all_keys() {
            var keys = new SettingsReader("key1 = 123\r\nkey2 = 124\r\n# key3 = 12838");

            keys.Keys.Should().ContainInOrder(new[] {"key1","key2", "key3" });
        }

        [Test]
        public void list_all_keys_without_duplicates()
        {
            var keys = new SettingsReader("key1 = 123\r\nkey1 = 124\r\n# key3 = 12838");

            keys.Keys.Should().HaveCount(2);
            keys.Keys.Should().ContainInOrder(new[] { "key1", "key3" });
        }

        [Test]
        public void commented_keys_wont_return_a_value()
        {
            var keys = new SettingsReader("key1 = 123\r\nkey2 = 124\r\n# key3 = 12838");

            keys["key3"].Should().BeNullOrEmpty();
            keys["key2"].Should().BeEquivalentTo("124");
        }

        [Test]
        public void ensure_key_adds_commented_out_key()
        {
            var keys = new SettingsReader("key1 = 123");
            keys.Keys.Should().Contain("key1");

            keys.EnsureKey("key2", "455");
            var serialized = keys.SerializeToString();

            keys.Keys.Should().Contain("key2");
            keys["key2"].Should().BeNullOrEmpty();

            serialized.Should().EndWithEquivalent("#key2 = 455");
        }

        [Test]
        public void ignore_empty_or_invalid_keys() {
            var keys = new SettingsReader("key1 = 123");

            keys.EnsureKey(null, "");
            keys.EnsureKey("", "");
            keys.EnsureKey("    ", "");
            keys.EnsureKey("foo\nbar", "");
            keys.EnsureKey("foo\rbar", "");
            
            keys.SerializeToString().Should().BeEquivalentTo("key1 = 123");
        }

        [Test]
        public void ensure_adds_key_only_once() {
            var keys = new SettingsReader("key1 = 123");
            keys.EnsureKey("foo", "1");
            keys.EnsureKey("foo", "2");
            keys.EnsureKey("foo", "3");

            keys.Keys.Should().HaveCount(2);
            keys.SerializeToString().Should().BeEquivalentTo("key1 = 123\r\n#foo = 1");
        }

        [Test]
        public void read_key_ensures_key()
        {
            var keys = new SettingsReader("key1 = 123");

            var newkey = keys["key2"];

            newkey.Should().BeNullOrEmpty();
            keys.SerializeToString().Should().BeEquivalentTo("key1 = 123\r\n#key2 = ");
        }

        [Test]
        public void handles_multi_hash()
        {
            var DefTestData = "###########################################\r\nkey1 = 234";
            var DefSubject = new SettingsReader(DefTestData);
            DefSubject["key1"].Should().BeEquivalentTo("234");
        }

        [Test]
        public void handles_default_setup_file()
        {
            var DefTestData = System.IO.File.ReadAllText(@"data\default-shutupify-settings");
            var DefSubject = new SettingsReader(DefTestData);
            DefSubject["Hotkeys:Activated"].Should().BeEquivalentTo("yes");
        }

    }
}
