using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using IndyTCP;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using LogWriter_Obj;

using System.IO;

namespace ServerForm
{

    public partial class Form1 : Form
    {
        TcpListener tcpListener = null;
        private delegate void updateUI(string str, Control ctl);
        //private Thread newThread;

        private Boolean _isRunning;
        Thread _LoopClients;
        public Form1()
        {
            InitializeComponent();
            init();
        }

        public void init()
        {

            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            tcpListener.Start();

            _isRunning = true;
            _LoopClients = new Thread(LoopClients);
            _LoopClients.Start();

            textBox1.Text = textBox1.Text + "Server 啟動!\r\n";
        }

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

        public void LoopClients()
        {
            while (_isRunning)
            {
                // wait for client connection
                TcpClient newClient = tcpListener.AcceptTcpClient();
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.Start(newClient);
            }
        }

        public void HandleClient(object obj)
        {
            // retrieve client from parameter passed to thread
            TcpClient _obj = (TcpClient)obj;
            Socket client = _obj.Client;
            //Socket socketForClient = tcpListener.AcceptSocket();
            // sets two streams
            //StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            //StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);
            // you could use the NetworkStream to read and write, 
            // but there is no forcing flush, even when requested


             //Update_Text("Client:" + socketForClient.RemoteEndPoint + " now connected  to server. \n", textBox1);
                NetworkStream networkStream = new NetworkStream(client);
                //System.IO.StreamWriter streamWriter =  new System.IO.StreamWriter(networkStream);
                System.IO.StreamReader streamReader = new System.IO.StreamReader(networkStream);
                string _Result = string.Empty;
                byte[] result = null;
                string _Port = string.Empty;
                string RemoteIP = client.RemoteEndPoint.ToString();
                while (true)
                {
                    if (client.Connected)
                    {

                        try
                        {
                            
                            string Receive = streamReader.ReadLine();
                            if (Receive != "" && Receive != null)
                            {
                                //Console.WriteLine("Message recieved by client:" + theString);  
                                Update_Text("Message recieved by client:" + " " + Receive + "\r\n", textBox1);
                                result = System.Text.Encoding.ASCII.GetBytes("    " + Receive);//  data
                                networkStream.Write(result, 0, result.Length);
                                Update_Text("Send recieved to client:" + " " + _Result + "\r\n", textBox1);
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch
                        {
                            MessageBox.Show("client close");
                            break;
                        }
                    }
                   
                }
                streamReader.Close();
                networkStream.Close();
            
           
        }




        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            tcpListener = null;
            _isRunning = false;

            this.Dispose();
            Environment.Exit(Environment.ExitCode);
        }

    }

}