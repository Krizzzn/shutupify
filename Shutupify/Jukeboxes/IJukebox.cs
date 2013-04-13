using Shutupify.Settings;
using System;
namespace Shutupify
{
    public interface IJukebox : IName
    {
        void PerformAction(JukeboxCommand cmd);

        /// <summary>
        /// Shall the client receive messages?
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Is the client available?
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Is the client playing?
        /// </summary>
        bool IsPlaying { get; }
    }
}
