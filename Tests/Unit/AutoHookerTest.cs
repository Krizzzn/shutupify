using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using Moq.Linq;
using Shutupify;
using System.Reflection;
using FluentAssertions;
using Shutupify.Probes;
using Shutupify.Settings;

namespace Shutupify.Unit
{
    [TestFixture]
    public class AutoHookerTest {

        [Test]
        public void load_probes_from_given_dll() {
            AutoHooker auto = new AutoHooker();

            var assem = Assembly.LoadFrom(@"data\shutupify-mock.dll");
            auto.LoadFromAssembly(assem);

            auto.Probes.Length.Should().BeGreaterOrEqualTo(1);
            auto.Jukeboxes.Length.Should().BeGreaterOrEqualTo(1);
            auto.Probes.Should().Contain(probe => probe.GetType().FullName.EndsWith("ProbeMock"));
        }

        [Test]
        public void should_automatically_load_own_assembly() {
            AutoHooker auto = new AutoHooker();
            
            auto.Probes.Length.Should().BeGreaterOrEqualTo(1);
            auto.Jukeboxes.Length.Should().BeGreaterOrEqualTo(1);

            auto.Probes.Should().Contain(probes => probes is LockWindowsProbe);
        }

        [Test]
        public void all_probes_and_jukeboxes_can_be_cleared() {
            AutoHooker auto = new AutoHooker();

            auto.Probes.Length.Should().BeGreaterOrEqualTo(1);

            auto.Clear();

            auto.Probes.Length.Should().BeGreaterOrEqualTo(0);
            auto.Jukeboxes.Length.Should().BeGreaterOrEqualTo(0);
        }

        [Test]
        public void event_routing_setup() {
            var probe = new Mock<IEventProbe>();
            var jukebox = new Mock<IJukebox>();
            AutoHooker auto = new AutoHooker();
            auto.Clear();

            probe.Setup(m => m.StartObserving())
                .Returns(true);

            auto.Add(probe.Object);
            auto.Add(jukebox.Object);

            auto.Hookup();
            probe.Verify(m => m.StartObserving(), Times.AtLeastOnce());
        }

        [Test]
        public void event_routing_is_correct() {
            var probe = new Mock<IEventProbe>();
            var jukebox = new Mock<IJukebox>();
            var eventFired = false;
            AutoHooker auto = new AutoHooker();
            auto.Clear();
            auto.ReactOnEvent += (a) => {if (a == JukeboxCommand.Toggle) eventFired = true;};

            probe.Setup(m => m.StartObserving())
                .Returns(true);

            auto.Add(probe.Object);
            auto.Add(jukebox.Object);

            auto.Hookup();

            probe.Raise(m => m.ReactOnEvent += null, JukeboxCommand.Toggle);
            jukebox.Verify(j => j.PerformAction(JukeboxCommand.Toggle));
            eventFired.Should().BeTrue();
        }

        [Test]
        public void reads_settings()
        {
            var settings = new Mock<ISettingsReader>();

            var probe = new Mock<IEventProbe>();
            var probeSettable = probe.As<ISettable>();
            
            var jukebox = new Mock<IJukebox>();
            var jukeboxSettable = jukebox.As<ISettable>();

            AutoHooker auto = new AutoHooker(settings.Object);
            auto.Clear();

            auto.Add(probe.Object);
            auto.Add(jukebox.Object);

            auto.Hookup();
            probeSettable.Verify(m => m.ReadSettings(settings.Object), Times.Once());
            jukeboxSettable.Verify(m => m.ReadSettings(settings.Object), Times.Once());
        }

        [Test]
        public void ensures_type_activated_key()
        {
            var settings = new Mock<ISettingsReader>();

            var probe = new Mock<IEventProbe>();
            var probeSettable = probe.As<ISettable>();
            var probeName = probe.As<IName>();
            probeName.Setup(m => m.Name).Returns("probe");

            var jukebox = new Mock<IJukebox>();
            var jukeboxSettable = jukebox.As<ISettable>();

            AutoHooker auto = new AutoHooker(settings.Object);
            auto.Clear();

            auto.Add(probe.Object);
            auto.Add(jukebox.Object);

            auto.Hookup();
            settings.Verify(m => m.EnsureKey("probe:activated", "no"), Times.Once());
        }

        [Test]
        [TestCase("no", false)]
        [TestCase("asdjasdlkj", false)]
        [TestCase("yes", true)]
        [TestCase("true", true)]
        [TestCase("active", true)]
        public void obeys_probe_type_activated_key(string returns, bool shouldBeActivated)
        {
            var settings = new Mock<ISettingsReader>();
            settings.SetupGet(m => m["Proby:activated"]).Returns(returns);

            var probe = new Mock<IEventProbe>();
            probe.SetupGet(m => m.Name).Returns("Proby");

            var jukebox = new Mock<IJukebox>();

            AutoHooker auto = new AutoHooker(settings.Object);
            auto.Clear();

            auto.Add(probe.Object);
            auto.Add(jukebox.Object);

            auto.Hookup();
            
            probe.Verify(m=>m.StartObserving(), Times.Exactly(shouldBeActivated ? 1 : 0));
        }

        [Test]
        [TestCase("no", false)]
        [TestCase("asdjasdlkj", false)]
        [TestCase("yes", true)]
        [TestCase("true", true)]
        [TestCase("active", true)]
        public void obeys_jukebox_type_activated_key(string returns, bool shouldBeActivated)
        {
            var settings = new Mock<ISettingsReader>();
            settings.SetupGet(m => m["Jukey:activated"]).Returns(returns);

            var probe = new Mock<IEventProbe>();
            var jukebox = new Mock<IJukebox>();
            var jukeboxIName = jukebox.As<IName>();
            jukeboxIName.SetupGet(m => m.Name).Returns("Jukey");

            AutoHooker auto = new AutoHooker(settings.Object);
            auto.Clear();

            auto.Add(probe.Object);
            auto.Add(jukebox.Object);

            auto.Hookup();

            jukebox.VerifySet(m => m.Active = shouldBeActivated, Times.Once());
        }
    }
}
