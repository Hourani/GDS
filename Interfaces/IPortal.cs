using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces
{
    public delegate void PortalEvent(object sender, int event_number);

    public interface IPortal
    {
        event PortalEvent portalEvent;
        Boolean checkFolder(string name);
        void createFolder(string name, long id);
    }
}