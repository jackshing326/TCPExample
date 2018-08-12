using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using IndyTCP;
using System.Configuration;
namespace ClientForm
{
    public partial class Form1 : Form
    {
        //資料來源 http://csharp.net-informations.com/communications/csharp-multi-threaded-client-socket.htm
             
        IndyClient _IndyClient = null;
        string Account = string.Empty;
        public Form1()
        {
            InitializeComponent();
            Account =  ConfigurationSettings.AppSettings["Account"].Trim();
            //timer1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
         
            string Result=string.Empty;
            string Send = "cLW" + Account + "#0";
            _IndyClient.Send_Indy(Send);
            Update_Text("Message Send to Server:" + " " + Send + "\r\n", tb_Server);

            _IndyClient.Receive_Indy(ref Result);

            Update_Text("Message recieved by Server:" + " " + Result + "\r\n", tb_Server);
                     

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //msg("Client Socket Program - Server Connected");
            Update_Text("Client Socket Program - Server Connected"+ "\r\n", tb_Server);
       
            _IndyClient = new IndyClient();
            _IndyClient.IP = "127.0.0.1";
            _IndyClient.Port = 8888;
            //clientSocket.Connect("127.0.0.1", 8888);

        
        }


        private delegate void updateUI(string str, Control ctl);
        private void Update_Text(string str, Control ctl)
        {
            if (ctl.InvokeRequired)
            {
                updateUI uu = new updateUI(Update_Text);
                ctl.Invoke(uu, str, ctl);
            }
            else
            {
                ctl.Text += str;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int Now= Int32.Parse(DateTime.Now.ToString("ss"));
            if (Now % 5 == 0)
            {
                button1_Click(null, EventArgs.Empty);
            }
        }

 
    }
}
