using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Windows.Forms;
using Interfaces;

namespace FileShareClient
{
    public partial class Login : Form
    {
        private IUser user;
        private IPortal portal;
        private IFTserver ftserver;
        private int myID;
        public string myUsername;
        public string myHFolders;

        public Login()
        {
            InitializeComponent();
            RegisterRemoting();
            this.Focus();
        }

        private void RegisterRemoting()
        {
            {
                try
                {
                    BinaryServerFormatterSinkProvider server_provider = new BinaryServerFormatterSinkProvider();
                    server_provider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

                    BinaryClientFormatterSinkProvider client_provider = new BinaryClientFormatterSinkProvider();
                    IDictionary properties = new Hashtable();

                    properties["port"] = "0";

                    TcpChannel channel = new TcpChannel(properties, client_provider, server_provider);
                    ChannelServices.RegisterChannel(channel, false);

                    user = (IUser)Activator.GetObject(typeof(IUser), "tcp://localhost:9998/UserHandeling");
                    portal = (IPortal)Activator.GetObject(typeof(IPortal), "tcp://localhost:9998/PortalHandeling");
                    ftserver = (IFTserver)Activator.GetObject(typeof(IFTserver), "tcp://localhost:9998/TransferHandeling");
                }
                catch (RemotingException e)
                {
                    MessageBox.Show("Connection Error");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Register fm = new Register(this,user);
            this.Hide();
            fm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (user.checkUser(usernameTXT.Text, passwordTXT.Text))
            {
                myUsername = usernameTXT.Text;
                myID = user.getID(myUsername, passwordTXT.Text);
                myHFolders = user.getHFolders(myID);
                PortalForm pfm = new PortalForm(portal,this,ftserver);
                this.Hide();
                pfm.Show();
            }
            else
            {
                MessageBox.Show("Username/Password not correct");
            }
        }

    }
}
