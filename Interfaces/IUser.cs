using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces
{
    public interface IUser
    {

        Boolean checkUser(string username, string password);
        Boolean checkUsername(string username);
        void registerUser(string username, string password);
        string getHFolders(int id);
        int getID(string username, string password);
    }
}
