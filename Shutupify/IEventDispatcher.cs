using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shutupify
{
    public interface IEventDispatcher
    {
         void Dispatch(JukeboxCommand cmd);

         IEnumerable<IJukebox> Jukeboxes {get;set;}
    }
}
