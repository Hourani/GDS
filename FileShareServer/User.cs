using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Interfaces;

namespace FileShareServer
{
    public class User : MarshalByRefObject, IUser
    {
        private DataSet1TableAdapters.UsersTableAdapter usersAdapter = new FileShareServer.DataSet1TableAdapters.UsersTableAdapter();

        public Boolean checkUser(string username, string password)
        {
            string x = "0";

            x =  usersAdapter.CheckUser(username, password).ToString();
           
            if (x == "1")
                return true;
            else
                return false;
        }

        public Boolean checkUsername(string username)
        {
            string x = "0";

            x = usersAdapter.CheckUsername(username).ToString();

            if (x == "1")
                return true;
            else
                return false;
        }

        public void registerUser(string username, string password)
        {
                usersAdapter.RegisterUser(username, password);

        }

        public string getHFolders(int id)
        {
            string HFolders = usersAdapter.GetHomeFolders(id);
            return HFolders;
        }

        public int getID(string username,string password)
        {
            int ID = (int)usersAdapter.GetUserID(username, password);
            return ID;
        }

    }
}
