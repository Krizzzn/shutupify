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
        public FileSettingsReader GetSettingsReader() {
            var settings = "BlaSettings = 123\n" +
            "FooSettings       =     \t\t  123\t 21\t \n" +
            "\r\t\tBeeSettings =\t\t\t12321321\n" +
            "#commentedkey1 = value\n" +
            "   #   commentedkey2 = value\n" +
            " aComment = foo baz # bert\n" +
            " keywithoutvalue = \n" +
            " multiplus = = =";
                
            return new FileSettingsReader(settings);
        }

        [Test]
        public void reads_settings_from_string() {
            new FileSettingsReader("").Settings.Should().NotBeNull();
        }

        [Test]
        public void reads_simple_line() {
            var settings = GetSettingsReader();
            settings.Settings.Should().ContainKey("blasettings");
            settings.Settings["blasettings"].Should().BeEquivalentTo("123");
        }

        [Test]
        public void can_handle_tabs_and_whitespaces()
        {
            var settings = GetSettingsReader();
            settings.Settings.Should().ContainKey("foosettings");
            settings.Settings["foosettings"].Should().BeEquivalentTo("123\t 21");
        }

        [Test]
        public void ignores_line_comments()
        {
            var settings = GetSettingsReader();
            settings.Settings.Should().NotContainKey("commentedkey1");
            settings.Settings.Should().NotContainKey("#commentedkey1");
            settings.Settings.Should().NotContainKey("commentedkey2");
        }

        [Test]
        public void trims_comments_at_end_of_line()
        {
            var settings = GetSettingsReader();
            settings.Settings.Should().ContainKey("acomment");
            settings.Settings["acomment"].Should().BeEquivalentTo("foo baz");
        }

        [Test]
        public void key_without_value()
        {
            var settings = GetSettingsReader();
            settings.Settings.Should().ContainKey("keywithoutvalue");
            settings.Settings["keywithoutvalue"].Should().BeNullOrEmpty();
        }

        [Test]
        public void multiple_plusses()
        {
            var settings = GetSettingsReader();
            settings.Settings.Should().ContainKey("multiplus");
            settings.Settings["multiplus"].Should().BeEquivalentTo("= =");
        }

        [Test]
        public void serialize_to_string()
        {
            var keyValue = "key=value\nvey=kalue";
            var Settings = new FileSettingsReader(keyValue);
            var result = Settings.SerializeToString();

            keyValue.Should().BeEquivalentTo(result);
        }
    }
}
