using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Interfaces;

namespace FileShareClient
{
    public partial class Register : Form
    {
        private IUser user;
        private Login login;

        public Register(Login lgn,IUser usr)

        {
            InitializeComponent();
            user = usr;
            login = lgn;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (passwordTXT.Text == conpasswordTXT.Text)
            {
                if (user.checkUsername(usernameTXT.Text))
                {
                    MessageBox.Show("Sorry Username Already Taken, Please Choose Another Username And Try Again");
                }

                else

                {
                    try
                    {
                        user.registerUser(usernameTXT.Text, passwordTXT.Text);
                        MessageBox.Show("Account Registered Successfully!You Can Now Login");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error Creating Account!!");
                    }

                    this.Close();
                    login.Show();
                }
            }

            else
            {
                MessageBox.Show("Password Fields Don't Match!");
            }

            }

        private void button2_Click(object sender, EventArgs e)
        {
             this.Close();
             login.Show();
        }
    }
}
