using Shutupify.Settings;
using System;
namespace Shutupify
{
    public interface IJukebox : IName
    {
        void PerformAction(JukeboxCommand cmd);

        bool IsActive { get; set; }
        bool IsPlaying { get; }
    }
}
