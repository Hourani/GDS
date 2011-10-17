using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Interfaces;

namespace FileShareServer
{
    public class Portal : MarshalByRefObject, IPortal
    {
        public event PortalEvent portalEvent;

        private DataSet1TableAdapters.FoldersTableAdapter foldersAdapter = new FileShareServer.DataSet1TableAdapters.FoldersTableAdapter();

        public Boolean checkFolder(string name)
        {
            string x = "0";

            x = foldersAdapter.CheckFolder(name).ToString();

            if (x == "1")
                return true;
            else
                return false;
        }


        public void createFolder(string name, long id)
        {
            try
            {
                foldersAdapter.CreateFolder(name, id);
            }
            catch
            {
                MessageBox.Show("sssss");
            }
            
            
        }

        void FireEvent(object sender, int event_number)
        {
            if (portalEvent != null) //only if at least one client has registered to event
            {
                portalEvent.Invoke(sender, event_number);
            }
        }

    }
}