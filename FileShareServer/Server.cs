using System;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Interfaces;

namespace FileShareServer
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
            SetupTcpChannelWithCustomizedSink();

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(User), "UserHandeling",
                WellKnownObjectMode.Singleton);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Portal), "PortalHandeling",
            WellKnownObjectMode.Singleton);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(FTserver), "TransferHandeling",
            WellKnownObjectMode.Singleton);
        }

        static void SetupTcpChannelWithCustomizedSink()
        {
            BinaryServerFormatterSinkProvider server_provider = new BinaryServerFormatterSinkProvider();
            server_provider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            BinaryClientFormatterSinkProvider client_provider = new BinaryClientFormatterSinkProvider();

            IDictionary properties = new Hashtable();
            properties["port"] = 9998;

            TcpChannel channel = new TcpChannel(properties, client_provider, server_provider);
            ChannelServices.RegisterChannel(channel, false);
        }

        private void Server_Load(object sender, EventArgs e)
        {
        }
    }
}
