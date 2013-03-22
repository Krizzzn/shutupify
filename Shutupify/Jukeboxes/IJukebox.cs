using System;
namespace Shutupify
{
    public interface IJukebox
    {
        void PerformAction(JukeboxCommand cmd);
    }
}
