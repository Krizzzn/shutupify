using Shutupify.Settings;
using System;
namespace Shutupify
{
    public interface IJukebox : IName
    {
        bool Active { get; set; }
        void PerformAction(JukeboxCommand cmd);

    }
}
