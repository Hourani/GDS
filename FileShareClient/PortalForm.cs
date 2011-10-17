using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using Interfaces;


namespace FileShareClient
{
    public partial class PortalForm : Form
    {
        private IPortal portal;
        public IFTserver ftserver;
        private Login login;
        private int iFiles = 0;
        private int iDirectories = 0;

        public PortalForm(IPortal prtl, Login lgn, IFTserver ftsrvr)
        {
            InitializeComponent();
            portal = prtl;
            ftserver = ftsrvr;
            login = lgn;
            //portal.portalEvent += new PortalEvent(HandlePortalEvent);
            SetWindowTheme(treeView1.Handle, "explorer", null);
            SetWindowTheme(listView1.Handle, "explorer", null);
            SetWindowTheme(listView2.Handle, "explorer", null);
            this.treeView1.HotTracking = true;
            this.treeView1.FullRowSelect = true;
        }
        /// Native interop method, be sure to include the  System.Runtime.InteropServices
        /// name space
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="appName"></param>
        /// <param name="partList"></param>
        /// <returns></returns>
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int SetWindowTheme(IntPtr hWnd, string appName, string partList);

        private void AddDirectories(TreeNode tnSubNode)
        {
            treeView1.BeginUpdate();
            iDirectories = 0;

            try
            {
                DirectoryInfo diRoot;

                
                if (tnSubNode.SelectedImageIndex < 11)
                {
                    diRoot = new DirectoryInfo(tnSubNode.FullPath + "\\");
                }
               
                else
                {
                    diRoot = new DirectoryInfo(tnSubNode.FullPath);
                }
                DirectoryInfo[] dirs = diRoot.GetDirectories();
               
                tnSubNode.Nodes.Clear();
         
                foreach (DirectoryInfo dir in dirs)
                {
                    iDirectories++;
                    TreeNode subNode = new TreeNode(dir.Name);
                    subNode.ImageIndex = 11;
                    subNode.SelectedImageIndex = 12;
                    tnSubNode.Nodes.Add(subNode);
                }

            }
           
            catch { ;	}

            treeView1.EndUpdate();
        }

        private void AddFiles(string strPath)
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();

            iFiles = 0;

            try
            {
                DirectoryInfo di = new DirectoryInfo(strPath + "\\");
                FileInfo[] theFiles = di.GetFiles();
                foreach (FileInfo theFile in theFiles)
                {
                    if (theFile.Extension.ToString() == ".txt")
                    {
                        iFiles++;
                        ListViewItem lvItem = new ListViewItem(theFile.Name,0);
                        listView1.Items.Add(lvItem);
                    }
                }
            }
            catch (Exception Exc) { statusBar1.Text = Exc.ToString(); }

            listView1.EndUpdate();
        }

        private void menuItem4_Click(object sender, EventArgs e)
        {
            login.Close();
        }

        private void populateHFolders()
        {
            string s = login.myHFolders;
            if (s != "")
            {
                string[] sArray = s.Split(';');

                foreach (string c in sArray)
                {
                    listView2.Items.Add(c, 0);
                }
            }
            else
                listView2.Items.Add("You are not sharing anyfolders");
        }

        private void PortalForm_Load(object sender, EventArgs e)
        {
            label1.Text = login.myUsername;
            
        
            string[] aDrives = Environment.GetLogicalDrives();
            treeView1.BeginUpdate();
            foreach (string strDrive in aDrives)
            {
                TreeNode dnMyDrives = new TreeNode(strDrive.Remove(2, 1));

                switch (strDrive)
                {
                    case "A:\\":
                        dnMyDrives.SelectedImageIndex = 0;
                        dnMyDrives.ImageIndex = 0;
                        break;
                    case "C:\\":

                        treeView1.SelectedNode = dnMyDrives;
                        dnMyDrives.SelectedImageIndex = 1;
                        dnMyDrives.ImageIndex = 1;

                        break;
                    case "D:\\":
                        dnMyDrives.SelectedImageIndex = 1;
                        dnMyDrives.ImageIndex = 1;
                        break;
                    default:
                        dnMyDrives.SelectedImageIndex = 2;
                        dnMyDrives.ImageIndex = 2;
                        break;
                }

                treeView1.Nodes.Add(dnMyDrives);
            }
            treeView1.EndUpdate();
            populateHFolders();
        }

        private void treeView1_AfterSelect_1(object sender, TreeViewEventArgs e)
        {
            AddDirectories(e.Node);

            AddFiles(e.Node.FullPath.ToString());
            statusBar1.Text = iDirectories.ToString() + " Folder(s)  " + iFiles.ToString() + " Text File(s)";
        }

        private void listView1_ItemActivate_1(object sender, EventArgs e)
        {
            try
            {
                string sPath = treeView1.SelectedNode.FullPath;
                string sFileName = listView1.FocusedItem.Text;
                backgroundWorker1.RunWorkerAsync();
                SendFile(sPath + "\\" + sFileName);
                //Process.Start(sPath + "\\" + sFileName);
            }
            catch (Exception Exc) 
            { 
                MessageBox.Show(Exc.ToString());
            }
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            this.Close();
            login.Show();
        }

        public void HandlePortalEvent(object o, int evnr)
        {
            if (evnr == 1)
            {
                MessageBox.Show(" the event was fired");
            }
            else
            {

            }

        }

        public static void SendFile(string fileName)
        {
            
            try
            {         
                IPAddress[] ipAddress = Dns.GetHostAddresses("localhost");
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Loopback, 9988);
                Socket clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                string filePath = "";

                 fileName = fileName.Replace("\\", "/");
                 while (fileName.IndexOf("/") > -1)
                {
                    filePath += fileName.Substring(0, fileName.IndexOf("/") + 1);
                    fileName = fileName.Substring(fileName.IndexOf("/") + 1);
                }

                byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
                if (fileNameByte.Length > 850 * 1024)
                {
                   // "File size is more than 850kb, please try with small file.";
                    return;
                }

               // Buffering
                byte[] fileData = File.ReadAllBytes(filePath + fileName);
                byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
                byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);

                fileNameLen.CopyTo(clientData, 0);
                fileNameByte.CopyTo(clientData, 4);
                fileData.CopyTo(clientData, 4 + fileNameByte.Length);

                //Connection to server
                clientSock.Connect(ipEnd);

                //File sending
                clientSock.Send(clientData);

                //Disconnecting
                clientSock.Close();
                //File transferred

            }
            catch (Exception ex)
            {
                if (ex.Message == "No connection could be made because the target machine actively refused it")
                    MessageBox.Show("File Sending fail. Because server not running.");
                else
                    MessageBox.Show("File Sending fail." + ex.Message);
            }

        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            splitContainer1.Height = 0;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            while (panel1.Height < 413)
            {
                panel1.Height += 1;
            }
            timer1.Enabled = false;
        }

        private void listView2_ItemActivate(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            //adding the files that belong to that folder
            listView2.Items.Add("test", 1);
        }


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ftserver.StartServer();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

    }
}
