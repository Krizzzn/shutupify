using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using FluentAssertions;
using Shutupify;
using Shutupify.Probes;
using Shutupify.Settings;
using System.Windows.Forms;

namespace Shutupify.Unit
{
    [TestFixture]
    public class HotkeysProbeTest
    {
        [Test]
        public void ensure_default_buttons() {
            HotkeysProbe keys = new HotkeysProbe();

            var settings = new Mock<ISettingsReader>();

            keys.ReadSettings(settings.Object);

            settings.Verify(m => m.EnsureKey("Hotkeys:Play", It.IsAny<string>()));
            settings.Verify(m => m.EnsureKey("Hotkeys:Pause", It.IsAny<string>()));
            settings.Verify(m => m.EnsureKey("Hotkeys:PreviousTrack", It.IsAny<string>()));
            settings.Verify(m => m.EnsureKey("Hotkeys:NextTrack", It.IsAny<string>()));
        }

        [Test]
        [TestCase("CTRL+ALT+SHIFT+Up")]
        [TestCase("STRG+ALT+SHIFT+Up")]
        [TestCase("STRG + ALT + SHIFT + Up")]
        [TestCase("ALT + SHIFT + CTRL + Up")]
        public void read_hotkeys(string hotkey)
        {
            HotkeysProbe keys = new HotkeysProbe();

            var settings = new Mock<ISettingsReader>();
            settings.SetupGet(m => m["Hotkeys:Play"]).Returns(hotkey);

            keys.ReadSettings(settings.Object);

            var key = keys.Hotkeys.First();
            key.Shift.Should().BeTrue();
            key.Alt.Should().BeTrue();
            key.Ctrl.Should().BeTrue();
            key.KeyCode.Should().Be(Keys.Up);

            key.Enabled = false;
            key.Dispose();
        }

        [Test]
        [TestCase("CTRL+ALT+SHIFT+Down", System.Windows.Forms.Keys.Down)]
        [TestCase("STRG+ALT+SHIFT+A", System.Windows.Forms.Keys.A)]
        [TestCase("STRG + ALT + SHIFT + F1", System.Windows.Forms.Keys.F1)]
        [TestCase("ALT + SHIFT + CTRL + Left", System.Windows.Forms.Keys.Left)]
        public void read_hotkeys_key(string hotkey, System.Windows.Forms.Keys expected)
        {
            HotkeysProbe keys = new HotkeysProbe();

            var settings = new Mock<ISettingsReader>();
            settings.SetupGet(m => m["Hotkeys:Play"]).Returns(hotkey);

            keys.ReadSettings(settings.Object);

            var key = keys.Hotkeys.First();
            key.KeyCode.Should().Be(expected);

            key.Enabled = false;
            key.Dispose();
        }

        [Test]
        [TestCase("CTRL + ALT + SHIFT")]
        [TestCase("CTRL + ALT")]
        [TestCase("ALT + CTRL")]
        [TestCase("")]
        [TestCase("Up")]
        [TestCase(null)]
        public void read_hotkeys_and_dont_accept_invalds(string hotkey)
        {
            HotkeysProbe keys = new HotkeysProbe();

            var settings = new Mock<ISettingsReader>();
            settings.SetupGet(m => m["Hotkeys:Play"]).Returns(hotkey);

            keys.ReadSettings(settings.Object);

            keys.Hotkeys.Should().BeEmpty();
        }
    }
}
