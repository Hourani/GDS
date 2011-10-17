using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Interfaces;

namespace FileShareServer
{
    public class FTserver : MarshalByRefObject,IFTserver
    {
        IPEndPoint ipEnd;
        Socket sock;
        public FTserver()
        {
           ipEnd = new IPEndPoint(IPAddress.Any, 9988);
           sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
           sock.Bind(ipEnd);
        }
        public static string receivedPath = "C:/FileShareHD";

        public  void StartServer()
        {
            try
            {
               //Starting
                sock.Listen(100);

                //Running and waiting to receive file
                Socket clientSock = sock.Accept();

                byte[] clientData = new byte[1024 * 5000];
                
                int receivedBytesLen = clientSock.Receive(clientData);
               //Receiving data

                int fileNameLen = BitConverter.ToInt32(clientData, 0);
                string fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);

                BinaryWriter bWrite = new BinaryWriter(File.Open(receivedPath +"/"+ fileName, FileMode.Append)); ;
                bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);

                //Saving file

                bWrite.Close();
                clientSock.Close();
                //Recived & Saved file; Server Stopped
            }
            catch (Exception ex)
            {
                MessageBox.Show("File Recieving error.");
            }
        }
    }
}
