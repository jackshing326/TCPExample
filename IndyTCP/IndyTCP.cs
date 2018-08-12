using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LogWriter_Obj;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Drawing;
using System.Collections;
using System.Threading.Tasks;
namespace IndyTCP
{
  
    #region  IndyClient 說明
    /*
    IndyClient要送出去資料時,最後面都會加入"\r\n";並且在接收資料時,接收完後斷線
    而C# 的 TCPClient ,每次斷線後,要在連線時,就要在new 一個新的TCPClient     
    最好使用Close , 而非Disconnect!
     clientSocket.Close();
     clientSocket.Disconnet(true);
    */
    #endregion

    public class IndyClient
    {

        private int _Port;
        public int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }

        private string _IP;
        public string IP
        {
            get { return _IP; }
            set { _IP = value; }
        }

        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream;
        LogWriter Log = new LogWriter();
        int _ThreadSleepTime = 1000;
        int buffersize = 1024;

        public void Dispose()
        {
            if (clientSocket.Client.Connected)
            {
                //clientSocket.Client.Disconnect(true);
                clientSocket.Close();
            }
        }

        public bool IsConnect()
        {
            if (clientSocket.Connected)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool Send_Indy(string data)
        {
            // Convert the string data to byte data using ASCII encoding.
            // Clinet送過去ㄉ資料不用前面加四碼長度計算 ,只需要後面加上\r\n
            // 但從Server 收到資料會有前面4個byte是計算長度 
            clientSocket = new TcpClient();
            clientSocket.Client.Connect(_IP, _Port);
            NetworkStream serverStream;
            try
            {
                if (clientSocket.Connected)
                {
                    byte[] byteData = System.Text.Encoding.ASCII.GetBytes(data + "\r\n");
                    serverStream = clientSocket.GetStream();
                    serverStream.Write(byteData, 0, byteData.Length);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.WriterLog("Send_Indy data:" + data);
                Log.WriterLog("Send_Indy ex:" + ex.ToString());
                return false;
            }
        }

        public bool Receive_Indy(ref string ReturnString)
        {
            // Convert the string data to byte data using ASCII encoding.
            try
            {
                if (clientSocket.Connected)
                {
                    int bytesRead = 0;
                    int loop = 0;
                    int howMany = 0;
                    byte[] buffer = new byte[buffersize];
                    MemoryStream ms = new MemoryStream();
                    using (NetworkStream clientStream = clientSocket.GetStream())
                    {

                        // Wait for data to begin coming in for up to 20 secs
                        while (!clientStream.DataAvailable && loop < 2000)
                        {
                            loop++;
                            Thread.Sleep(_ThreadSleepTime);
                        }

                        // Keep reading until nothing comes for over 1 sec
                        while (clientStream.DataAvailable)
                        {

                            // Keep reading until nothing comes for over 1 sec
                            do
                            {
                                howMany = clientStream.Read(buffer, bytesRead, buffer.Length);
                                ms.Write(buffer, bytesRead, buffer.Length);
                            }
                            while (howMany > 0 && clientStream.DataAvailable);
                            byte[] buffer2 = new byte[ms.Length - 4];
                            Buffer.BlockCopy(ms.ToArray(), 4, buffer2, 0, buffer2.Length);
                            ReturnString = System.Text.Encoding.Default.GetString(buffer2, 0, buffer2.Length);
                            //去除空白,需要用特別的方法
                            ReturnString = Regex.Replace(ReturnString, @"[^\u0009^\u000A^\u000D^\u0020-\u007E]", "");
                            ReturnString = ReturnString.Replace("\r", "").Replace("\n", "");

                        }
                        clientSocket.Close();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.WriterLog("Receive_Indy ex:" + ex.ToString());
                clientSocket.Close();
                return false;
            }
        }

        public bool Receive_DirectionIndy(ref string ReturnString)
        {
            // Convert the string data to byte data using ASCII encoding.
            try
            {

                if (!clientSocket.Connected)
                {
                    clientSocket = new TcpClient();
                    clientSocket.Client.Connect(_IP, _Port);
                }

                if (clientSocket.Connected)
                {
                    int bytesRead = 0;
                    int loop = 0;
                    int howMany = 0;
                    byte[] buffer = new byte[buffersize];
                    MemoryStream ms = new MemoryStream();


                    using (NetworkStream clientStream = clientSocket.GetStream())
                    {

                        // Wait for data to begin coming in for up to 20 secs

                        while (!clientStream.DataAvailable && loop < 2000)
                        {
                            loop++;
                            Thread.Sleep(_ThreadSleepTime);
                        }

                        // Keep reading until nothing comes for over 1 sec
                        while (clientStream.DataAvailable)
                        {

                            // Keep reading until nothing comes for over 1 sec
                            do
                            {
                                howMany = clientStream.Read(buffer, bytesRead, buffer.Length);
                                ms.Write(buffer, bytesRead, buffer.Length);

                            }
                            while (howMany > 0 && clientStream.DataAvailable);
                            byte[] buffer2 = new byte[ms.Length - 4];
                            Buffer.BlockCopy(ms.ToArray(), 4, buffer2, 0, buffer2.Length);
                            ReturnString = System.Text.Encoding.Default.GetString(buffer2, 0, buffer2.Length);
                        }
                        clientSocket.Close();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.WriterLog("Receive_Indy ex:" + ex.ToString());
                clientSocket.Close();
                return false;
            }
        }

        public bool Receive_IndyByteArray_Pic(ref byte[] ReturnByte)
        {
            try
            {
                if (clientSocket.Connected)
                {
                    int bytesRead = 0;
                    int loop = 0;
                    int howMany = 0;
                    byte[] buffer = new byte[32468];
                    MemoryStream ms = new MemoryStream();
                    using (NetworkStream clientStream = clientSocket.GetStream())
                    {
                        // Wait for data to begin coming in for up to 20 secs
                        while (!clientStream.DataAvailable && loop < 2000)
                        {
                            loop++;
                            Thread.Sleep(_ThreadSleepTime);
                        }

                        // Keep reading until nothing comes for over 1 sec
                        while (clientStream.DataAvailable)
                        {
                            do
                            {
                                howMany = clientStream.Read(buffer, bytesRead, buffer.Length);
                                ms.Write(buffer, bytesRead, buffer.Length);


                            }
                            while (howMany > 0 && clientStream.DataAvailable);
                            byte[] buffer2 = new byte[ms.Length - 4];
                            Buffer.BlockCopy(ms.ToArray(), 4, buffer2, 0, buffer2.Length);
                            ReturnByte = buffer2;
                        }
                        clientStream.Close();
                    }
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.WriterLog("Receive_Indy ex:" + ex.ToString());
                clientSocket.Close();
                return false;
            }
        }

        public Image ByteToImage(byte[] blob, int Height, int Width)
        {
            try
            {
                Image image = Image.FromStream(new MemoryStream(blob));
                Size size = new System.Drawing.Size(Width, Height);
                image = (Image)(new Bitmap(image, size));
                return image;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool SendACK()
        {

            string SendMsg = "ACK";

            bool IsSuccess = false;
            try
            {

                if (!clientSocket.Connected)
                {
                    clientSocket = new TcpClient();
                    clientSocket.Client.Connect(_IP, _Port);
                }


                if (clientSocket.Connected)
                {

                    byte[] byteData = System.Text.Encoding.ASCII.GetBytes(SendMsg + "\r\n");
                    serverStream = clientSocket.GetStream();
                    serverStream.Write(byteData, 0, byteData.Length);
                    //clientSocket.Client.Disconnect(true);
                    clientSocket.Close();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                clientSocket.Client.Disconnect(true);
                return IsSuccess;
            }

            return true;
        }
    }

    #region  IndyServer 說明
    /*
    IndyServer要送出去資料時,最前面4個byte是長度的欄位,最後面都會加入"\r\n",如下所示:
      public byte[] ConvertToIndy(string data)
        {           
            try
            {          
                byte[] byteData = System.Text.Encoding.ASCII.GetBytes(data+"\r\n");//  data
                int dataLength = data.Length; 
                int Len1 = (dataLength) / (256 * 256 * 256);
                if (Len1 > 0)
                {
                    dataLength = dataLength - (Len1 * (256 * 256 * 256));
                }
                int Len2 = (dataLength) / (256 * 256);
                if (Len2 > 0)
                {
                    dataLength = dataLength - (Len2 * (256 * 256));
                }
                int Len3 = (dataLength) / (256);
                if (Len3 > 0)
                {
                    dataLength = dataLength - (Len3 * (256));
                }
                int Len4 = dataLength;
                byte[] sendbyteData = new byte[byteData.Length + 4];
                sendbyteData[0] = Convert.ToByte(Len1);
                sendbyteData[1] = Convert.ToByte(Len2);
                sendbyteData[2] = Convert.ToByte(Len3);
                sendbyteData[3] = Convert.ToByte(Len4);

                for (int i = 0; i < byteData.Length; i++)
                {
                    sendbyteData[4 + i] = byteData[i];
                }
                return sendbyteData;            
            }
            catch (Exception ex)
            {
                return null;
            }
        } 
    --------------------------------------------------------------------------------------------------------------------------------  
      而建立Server程式,其步驟如下
     *宣告 TcpListener 還有Thread 與其他變數
        TcpListener tcpListener = null;
        private delegate void updateUI(string str, Control ctl);
        BCServerExecClass _BCServerExec = new BCServerExecClass();
        private Boolean _isRunning;
        Thread _LoopClients;
     *設定各變數
        tcpListener = new TcpListener(IPAddress.Parse(ServerIP), Int32.Parse(ServerPort));
        tcpListener.Start();
        _isRunning = true;
        _LoopClients = new Thread(LoopClients);
        _LoopClients.Start();
    *各物件如下
     *更新Form上面的物件使用
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
     *啟動Server ,TcpListener
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
     * 處理資料的部分
        public void HandleClient(object obj)
        {
            TcpClient _obj = (TcpClient)obj;
            Socket client = _obj.Client;

            if (client.Connected)
            {
                //Update_Text("Client:" + socketForClient.RemoteEndPoint + " now connected  to server. \n", textBox1);
                NetworkStream networkStream = new NetworkStream(client);
                //System.IO.StreamWriter streamWriter =  new System.IO.StreamWriter(networkStream);
                System.IO.StreamReader streamReader = new System.IO.StreamReader(networkStream);
                string _Result = string.Empty;
                byte[] result = null;
                string _Port = string.Empty;
                string[] RemoteIP = client.RemoteEndPoint.ToString().Split(':');

                while (true)
                {
                    try
                    {

                        string Receive = streamReader.ReadLine();
                        Receive = Receive.Replace("\r\n", "");
                        if (Receive.Substring(0, 2).ToUpper() == "CL")
                        {

                            if (BCServerExec.CheckLogin(Receive, ref _Result, ref result, ref _Port))
                            {
                                string _Account = Receive.Substring(3, 1);
                                WriteConnectLog _WriteConnectLog = new WriteConnectLog();
                                string text = System.IO.File.ReadAllText(_WriteConnectLog.ConnectLogPath());
                                string PortResult1 = "Port" + _Port + "=1";
                                if (text.IndexOf(PortResult1) < 0)
                                {
                                    result = BCServerExec.ConvertToIndy("X");
                                    networkStream.Write(result, 0, result.Length);
                                }
                                else
                                {
                                    networkStream.Write(result, 0, result.Length);
                                    lock (AllService.Status)
                                    {
                                        AllService.Status = AllService.Status + "PeerIP=" + RemoteIP[0] + ":S" + _Account + _Port + ";";
                                    }
                                    _Log.WriterLog("Server Receive Message :" + Receive);
                                    _Log.WriterLog("Server Send Message :" + _Result);   
                                }
                            }
                        }
                        else if (Receive.Substring(0, 1).ToUpper() == "O")
                        {  //關機                 

                            lock (AllService.Status)
                            {
                                AllService.Status = AllService.Status + "PeerIP=" + RemoteIP[0] + ":" + "...."; ;
                            }
                        }
                        else if (Receive.Substring(0, 1).ToUpper() == "S")
                        {
                            //還活者          
                            lock (AllService.Status)
                            {
                                AllService.Status = AllService.Status + "PeerIP=" + RemoteIP[0] + ":" + Receive + ";";
                            }
                        }
                        break;

                    }
                    catch (Exception ex)
                    {
                        _Log.WriterLog("HandleClient Exception Ex:"+ex.ToString());     
                        streamReader.Close();
                        networkStream.Close();
                        break;
                    }
                }
                streamReader.Close();
                networkStream.Close();
            }
        }
    --------------------------------------------------------------------------------------------------------------------------------  
      其他說明:
          由於閱卷程式的Server端要處理的資料比較少但是比較多Client的部分,所以每次送回資料後,Server馬上跟Client斷線
          但Service只要處理單一個Client即可,但是處理資料比較多,所以不用馬上斷線,但須要設定EnterServiceFlag來判別目前是否可以處理資料
        public void HandleClient_DB(object obj)
        {
            // retrieve client from parameter passed to thread
            TcpClient _obj = (TcpClient)obj;
            Socket client = _obj.Client;
            NetworkStream networkStream = new NetworkStream(client);
            //System.IO.StreamWriter streamWriter =  new System.IO.StreamWriter(networkStream);
            System.IO.StreamReader streamReader = new System.IO.StreamReader(networkStream);
            string _Result = string.Empty;
            byte[] _Result_ByteArray = null;

            while (EnterServiceFlag)
            {
                try
                {
                    if (client.Connected)
                    {
                        string Receive = streamReader.ReadLine();

                        if (Receive != null && Receive != "")
                        {

                            if (Receive.Substring(0, 2).ToUpper() == "GF")
                            {
                                _BCServiceExecClass.Get_FirstItemInfo(ref EnterServiceFlag, Prgstate, QNumber, Receive, ref _Result, ref _Result_ByteArray, Item, GradeTime
                                    , RadioButtonFontSize, StrategyWidth, ScriptItemRange, ServerDirection, GradeTimeFlag, ref BarcodeQnumber, ref LoginName);

                                networkStream.Write(_Result_ByteArray, 0, _Result_ByteArray.Length);
                                EnterServiceFlag = true;
                                TTimer_Timer.Enabled = true;

                            }
                            else if (Receive.Substring(0, 2).ToUpper() == "GN")
                            {
                                // Get_ItemInfo(Command, AThread); 
                                //這裡有問題 20170911
                                _BCServiceExecClass.Get_ItemInfo(ref   EnterServiceFlag, ref Prgstate, QNumber, Receive, ref  _Result, ref  _Result_ByteArray
         , ref ThisAnsS1, ref ThisAnsS2, ref ThisAnsS3, ref ThisAnsR1, ref ThisAnsR2, ref ThisAnsR3, ref  RandomTestModeFlag, RandomStartNum
         , ref  _Barcode, TimeOut, Port, ref  timertimer_bool, ref LoginName, ref BarcodeQnumber);

                                networkStream.Write(_Result_ByteArray, 0, _Result_ByteArray.Length);

                                if (Prgstate == (int)PRG_LEVEL.GRADE || Prgstate == (int)PRG_LEVEL.REGRADE)
                                {
                                    _BCServiceExecClass.Write_DoTime(LoginName, false);
                                }
                                //  Jack add Port ,timertimer_bool
                                //  Jack 要再跑這functiojn Set_ApplicationTitle(Prgstate);
                                Set_ApplicationTitle(Prgstate);
                                //    int ThisAnsS1 = 0; int ThisAnsS2 = 0; int ThisAnsS3 = 0;
                                EnterServiceFlag = true;
                                TTimer_Timer.Enabled = true;

                            }
                            else if (Receive.Substring(0, 2).ToUpper() == "GI")
                            {
                                // Get_Item( Command, AThread );
                                //gIC0B105#531970532M-6
                                EnterServiceFlag = false;
                                _BCServiceExecClass.Get_Item(ref   EnterServiceFlag, Prgstate, Receive, ref  _Result_ByteArray, ItemRootPath, BlankPath, ref  LoginName);

                                networkStream.Write(_Result_ByteArray, 0, _Result_ByteArray.Length);
                                EnterServiceFlag = true;
                                TTimer_Timer.Enabled = true;
                                // break;   
                            }
                            else if (Receive.Substring(0, 2).ToUpper() == "WS")
                            {
                                EnterServiceFlag = false;
                                //wS10110C0B103#583400714M@ZiT1ZasQZ39eLQHQYzm7nPVUbn4rFQf1oscquoEiGCKuICk6Wxuty10/riiUN2Py2VqSHI0cGfhKffVJFHE9PSSem9Qh+XvLmlesE6gl28a3XB1gWE7c6DPJurj6nhcXBqbT/Ox9EbBU5qlPmKTNLrgTK92PDlEXI7HS8DaeCA0=
                                _BCServiceExecClass.Save_ItemScore(ref   EnterServiceFlag, ref Prgstate, Receive, ref  _Result, ref  _Result_ByteArray
        , ref ThisAnsS1, ref ThisAnsS2, ref ThisAnsS3, ref ThisAnsR1, ref ThisAnsR2, ref ThisAnsR3, ref  RandomTestModeFlag, RandomStartNum
        , ref  _Barcode, Port, ref LoginName, ItemRootPath, S_Rule1_Range, S_Rule2_Range, Rules_num
        , S_Rule1_Limit
        , S_Rule2_Limit
        , R_Rule1_Range
        , R_Rule2_Range
        );


                                networkStream.Write(_Result_ByteArray, 0, _Result_ByteArray.Length);
                                Set_ApplicationTitle(Prgstate);
                                EnterServiceFlag = true;
                                TTimer_Timer.Enabled = true;
                            }

                            else if (Receive.Substring(0, 2).ToUpper() == "FS")
                            {
                                //fS1013000A0B103#583410732M@
                                // Get_ItemInfo(Command, AThread); 
                                EnterServiceFlag = false;
                                _BCServiceExecClass.Save_Sample3Score(ref   EnterServiceFlag, ref Prgstate, Receive, ref  _Result, ref  _Result_ByteArray
         , ThisAnsS1, ThisAnsS2, ThisAnsS3, ThisAnsR1, ThisAnsR2, ThisAnsR3, ref  RandomTestModeFlag, RandomStartNum, ref  _Barcode, Port
         , ref LoginName, ItemRootPath, S_Rule1_Range, S_Rule2_Range, Rules_num, S_Rule1_Limit, S_Rule2_Limit, R_Rule1_Range, R_Rule2_Range);

                                networkStream.Write(_Result_ByteArray, 0, _Result_ByteArray.Length);
                                Set_ApplicationTitle(Prgstate);
                                EnterServiceFlag = true;
                                TTimer_Timer.Enabled = true;
                            }
                            else if (Receive.Substring(0, 2).ToUpper() == "WP")
                            {
                                //wP99100A0B103#531970405M //目前已無使用
                                EnterServiceFlag = false;
                                _BCServiceExecClass.Save_SampleScore(ref   EnterServiceFlag, ref Prgstate, Receive, ref  _Result, ref  _Result_ByteArray
                                , ref  RandomTestModeFlag, ref  _Barcode, ref LoginName);

                                networkStream.Write(_Result_ByteArray, 0, _Result_ByteArray.Length);
                                EnterServiceFlag = true;
                                TTimer_Timer.Enabled = true;
                                //  Set_ApplicationTitle(Prgstate);
                            }
                            else if (Receive.Substring(0, 2).ToUpper() == "WC")
                            {
                                // _BCServiceExecClass.Change_PRGstateToSAMPLE2(Command, AThread, (int)PRG_LEVEL.SAMPLE_2);
                                EnterServiceFlag = false;
                                _BCServiceExecClass.Change_PRGstateToSAMPLE2(ref EnterServiceFlag, (int)PRG_LEVEL.SAMPLE_2, Receive, ref  _Result, ref  _Result_ByteArray
            , Port, ref  LoginName);
                                Prgstate = (int)PRG_LEVEL.SAMPLE_2;

                                networkStream.Write(_Result_ByteArray, 0, _Result_ByteArray.Length);
                                EnterServiceFlag = true;
                                Set_ApplicationTitle(Prgstate);
                                TTimer_Timer.Enabled = true;
                            }
                            else if (Receive.Substring(0, 2).ToUpper() == "WD")
                            {
                                EnterServiceFlag = false;
                                //Change_PRGstateToSAMPLE2( Command, AThread, (int)PRG_LEVEL.STANDARD_2 );
                                _BCServiceExecClass.Change_PRGstateToSAMPLE2(ref EnterServiceFlag, (int)PRG_LEVEL.SAMPLE_2, Receive, ref  _Result, ref  _Result_ByteArray
          , Port, ref  LoginName);
                                Prgstate = (int)PRG_LEVEL.STANDARD_2;

                                networkStream.Write(_Result_ByteArray, 0, _Result_ByteArray.Length);
                                
                                Set_ApplicationTitle(Prgstate);
                                EnterServiceFlag = true;
                                TTimer_Timer.Enabled = true;
                            }
                            else if (Receive.Substring(0, 3).ToUpper() == "ACK")
                            {

                                _Log.WriterLog("  ACK  ");
                                TTimer_Timer.Enabled = false;

                            }
                            _Log.WriterLog(" Send Data : " + _Result);
                        }
                        //     
                    }
                }
                catch (Exception ex)
                {
                    TTimer_Timer.Enabled = false;
                    MessageBox.Show("client close ex:" + ex.ToString());
                    //break;
                }
            }
        }
    
    */
    #endregion


}
