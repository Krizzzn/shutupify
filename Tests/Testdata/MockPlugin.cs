using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shutupify;

namespace Shutupify.Testdata.MockPlugin
{
    public class ProbeMock : Shutupify.IEventProbe
    {
        public event Action<JukeboxCommand> ReactOnEvent;

        public bool Alive()
        {
            return true;
        }

        public bool StartObserving()
        {
            return true;
        }


        public string Name
        {
            get { return "ProbeMock"; }
        }
    }

    public class PlayerMock : Shutupify.IJukebox
    {
        public void PerformAction(JukeboxCommand cmd)
        {
            
        }

        public bool IsActive
        {
            get;
            set;
        }


        public string Name
        {
            get { return "PlayerMock"; }
        }


        public bool IsPlaying
        {
            get { return true; }
        }
    }

}
