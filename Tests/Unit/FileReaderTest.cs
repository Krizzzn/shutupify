using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FluentAssertions;
using Shutupify.Settings;
using Moq;

namespace Shutupify.Unit
{
    [TestFixture]
    public class FileReaderTest
    {
        [Test]
        public void wont_accept_FileReader_as_input() {
            Action a = () => { 
                FileReader m = new FileReader(@"data\FileSettingsReaderTestData"); 
                FileReader r = new FileReader(@"data\FileSettingsReaderTestData", m); 
            };

            a.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void filereader_is_transparent() {
            var settings = new Mock<ISettingsReader>();
            settings.SetupGet(m => m["hue"]).Verifiable();

            var filereader = new FileReader(@"data\FileSettingsReaderTestData", settings.Object);

            filereader.EnsureKey("hue", "9");
            var crap = filereader["hue"];
            var stuff = filereader.Keys;
            filereader.Settings = "new settings";
            crap = filereader.Settings;

            settings.Verify(m => m.EnsureKey("hue", "9"), Times.AtLeastOnce());

            settings.VerifyGet(m => m.Keys, Times.AtLeastOnce());
            settings.VerifyGet(m => m.Settings, Times.AtLeastOnce());
            settings.VerifySet(m => { m.Settings = "new settings"; }, Times.AtLeastOnce());
            settings.Verify();
        }

        [Test]
        public void filereader_reads_file() {
            var fileContent = System.IO.File.ReadAllText(@"data\FileSettingsReaderTestData");
            var settings = new Mock<ISettingsReader>();

            var filereader = new FileReader(@"data\FileSettingsReaderTestData", settings.Object);
            
            settings.VerifySet(m => { m.Settings = fileContent; }, Times.Once());
        }

        [Test]
        public void filereader_handles_an_non_existing_file_and_creates_it()
        {
            var filename = @"data\NotExisting";
            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename); 

            var settings = new Mock<ISettingsReader>();

            var filereader = new FileReader(filename, settings.Object);

            filereader.Save();
            System.IO.File.Exists(filename).Should().BeTrue(); 
        }
    }
}
