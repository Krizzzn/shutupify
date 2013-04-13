using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using Shutupify;

namespace Shutupify.Unit
{
    [TestFixture]
    public class EventDispatcherTest
    {
        private Mock<IJukebox> itunes;
        private Mock<IJukebox> spotify;
        private Mock<IJukebox> superplayer;

        [SetUp]
        public void CreateMocks() {
            itunes = new Mock<IJukebox>();
            spotify = new Mock<IJukebox>();
            superplayer = new Mock<IJukebox>();

            itunes.SetupGet(m => m.IsActive).Returns(true);
            spotify.SetupGet(m => m.IsActive).Returns(true);
            superplayer.SetupGet(m => m.IsActive).Returns(true);
        }

        [TestCase(true, JukeboxCommand.Pause)]
        [TestCase(false, JukeboxCommand.Play)]
        [TestCase(true, JukeboxCommand.NextTrack)]
        [TestCase(true, JukeboxCommand.PreviousTrack)]
        [TestCase(true, JukeboxCommand.Toggle)]
        public void dispatch_events_to_single_player(bool isPlaying, JukeboxCommand cmd) {
            itunes.SetupGet(m => m.IsPlaying).Returns(isPlaying); 
            EventDispatcher dsp = new EventDispatcher(new[]{itunes.Object});

            dsp.Dispatch(cmd);

            itunes.Verify(m => m.PerformAction(cmd));
        }

        [TestCase(JukeboxCommand.Pause)]
        [TestCase(JukeboxCommand.Play)]
        [TestCase(JukeboxCommand.NextTrack)]
        [TestCase(JukeboxCommand.PreviousTrack)]
        [TestCase(JukeboxCommand.Toggle)]
        public void dispatch_events_to_single_of_multiple_players(JukeboxCommand cmd)
        {
            itunes.SetupGet(m => m.IsPlaying).Returns(false);
            superplayer.SetupGet(m => m.IsPlaying).Returns(false);
            spotify.SetupGet(m => m.IsPlaying).Returns(true);
            EventDispatcher dsp = new EventDispatcher(new[] { itunes.Object, spotify.Object, superplayer.Object });

            dsp.Dispatch(cmd);

            superplayer.Verify(m => m.PerformAction(cmd), Times.Never());
            itunes.Verify(m => m.PerformAction(cmd), Times.Never());
            spotify.Verify(m => m.PerformAction(cmd), Times.Once());
        }

        [Test]
        public void event_is_sent_to_single_active_player()
        {
            var cmd = JukeboxCommand.Play;
            itunes.SetupGet(m => m.IsPlaying).Returns(false);
            superplayer.SetupGet(m => m.IsPlaying).Returns(false);
            spotify.SetupGet(m => m.IsPlaying).Returns(false);
            itunes.SetupGet(m => m.IsActive).Returns(false);
            superplayer.SetupGet(m => m.IsActive).Returns(false);

            EventDispatcher dsp = new EventDispatcher(new[] { itunes.Object, spotify.Object, superplayer.Object });

            dsp.Dispatch(cmd);

            superplayer.Verify(m => m.PerformAction(cmd), Times.Never());
            itunes.Verify(m => m.PerformAction(cmd), Times.Never());
            spotify.Verify(m => m.PerformAction(cmd), Times.Once());
        }

        [Test]
        public void event_may_not_be_dispatched_to_and_inactive_playing_player()
        {
            var cmd = JukeboxCommand.Play;
            itunes.SetupGet(m => m.IsPlaying).Returns(false);
            superplayer.SetupGet(m => m.IsPlaying).Returns(false);
            spotify.SetupGet(m => m.IsPlaying).Returns(true);
            itunes.SetupGet(m => m.IsActive).Returns(false);
            superplayer.SetupGet(m => m.IsActive).Returns(false);
            spotify.SetupGet(m => m.IsActive).Returns(false);

            EventDispatcher dsp = new EventDispatcher(new[] { itunes.Object, spotify.Object, superplayer.Object });

            dsp.Dispatch(cmd);

            superplayer.Verify(m => m.PerformAction(cmd), Times.Never());
            itunes.Verify(m => m.PerformAction(cmd), Times.Never());
            spotify.Verify(m => m.PerformAction(cmd), Times.Never());
        }

        [Test]
        public void send_event_to_last_player_if_unsure()
        {
            itunes.SetupGet(m => m.IsPlaying).Returns(false);
            superplayer.SetupGet(m => m.IsPlaying).Returns(false);
            spotify.SetupGet(m => m.IsPlaying).Returns(true);

            EventDispatcher dsp = new EventDispatcher(new[] { itunes.Object, spotify.Object, superplayer.Object });

            dsp.Dispatch(JukeboxCommand.Pause);
            spotify.SetupGet(m => m.IsPlaying).Returns(false);

            dsp.Dispatch(JukeboxCommand.Play);

            superplayer.Verify(m => m.PerformAction(It.IsAny<JukeboxCommand>()), Times.Never());
            itunes.Verify(m => m.PerformAction(It.IsAny<JukeboxCommand>()), Times.Never());
            spotify.Verify(m => m.PerformAction(It.IsAny<JukeboxCommand>()), Times.Exactly(2));
        }

        [Test]
        public void handles_play_after_paused_logic_start_playing() {
            itunes.SetupGet(m => m.IsPlaying).Returns(true);
            EventDispatcher dsp1 = new EventDispatcher(new[] { itunes.Object });

            dsp1.Dispatch(JukeboxCommand.Pause);
            itunes.SetupGet(m => m.IsPlaying).Returns(false);
            dsp1.Dispatch(JukeboxCommand.PlayAfterPaused);
            itunes.Verify(m => m.PerformAction(JukeboxCommand.Play), Times.Once());
        }

        [Test]
        public void handles_play_after_paused_logic_keep_quite() {
            spotify.SetupGet(m => m.IsPlaying).Returns(false);
            EventDispatcher dsp1 = new EventDispatcher(new[] { spotify.Object });

            dsp1.Dispatch(JukeboxCommand.Pause);
            spotify.SetupGet(m => m.IsPlaying).Returns(false);
            dsp1.Dispatch(JukeboxCommand.PlayAfterPaused);
            spotify.Verify(m => m.PerformAction(JukeboxCommand.Play), Times.Never());
        }
    }
}
