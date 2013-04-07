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

namespace Shutupify.Unit
{
    [TestFixture]
    public class AutoHookerTest {

        [Test]
        public void load_probes_from_given_dll() {
            AutoHooker auto = new AutoHooker();

            var assem = Assembly.LoadFrom(@"testdata\shutupify-mock.dll");
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
    }
}
