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
                        clientStream.Close();
                        //clientSocket.Client.Disconnect(true);


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
                //clientSocket.Client.Disconnect(true);
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
                        clientStream.Close();
                        //clientSocket.Client.Disconnect(true);

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
                //clientSocket.Client.Disconnect(true);
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

                        clientSocket.Client.Disconnect(true);


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
                clientSocket.Client.Disconnect(true);

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
                    clientSocket.Client.Disconnect(true);
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

    public class IndyServer2
    {
        public event EventHandler ServerReceivedEvent;
        public event EventHandler ServerSendEvent;

        /// <summary>
        /// Default Constants.
        /// </summary>
        public static IPAddress DEFAULT_SERVER = IPAddress.Parse("127.0.0.1");
        public static int DEFAULT_PORT = 31001;
        public static IPEndPoint DEFAULT_IP_END_POINT = new IPEndPoint(DEFAULT_SERVER, DEFAULT_PORT);

        /// <summary>
        /// Local Variables Declaration.
        /// </summary>
        private TcpListener m_server = null;
        private bool m_stopServer = false;
        private bool m_stopPurging = false;

        private Thread m_serverThread = null;
        //private ParameterizedThreadStart m_serverThread = null;

        private Thread m_purgingThread = null;
        private ArrayList m_socketListenersList = null;

        public bool ServerIsRun = false;

        public string ServerReceiveData = string.Empty;
        public bool IsNeedSendBack = false;
        public byte[] ServerSendMSG = null;

        private Thread ServerReveice = null;
        private Thread ServerSend = null;

        Task task1 = null;
        Task task2 = null;

        /*Server初始化*/
        public IndyServer2()
        {
            Init(DEFAULT_IP_END_POINT);
        }
        public IndyServer2(IPAddress serverIP)
        {
            Init(new IPEndPoint(serverIP, DEFAULT_PORT));
        }

        public IndyServer2(int port)
        {
            Init(new IPEndPoint(DEFAULT_SERVER, port));
        }

        public IndyServer2(IPAddress serverIP, int port)
        {
            Init(new IPEndPoint(serverIP, port));
        }

        public IndyServer2(string serverIP, int port)
        {
            Init(new IPEndPoint(IPAddress.Parse(serverIP), port));
        }

        public IndyServer2(IPEndPoint ipNport)
        {
            Init(ipNport);
        }

        private void Init(IPEndPoint ipNport)
        {
            try
            {
                m_server = new TcpListener(ipNport);
            }
            catch (Exception e)
            {
                m_server = null;
            }
        }
        /*Server初始化*/

        /*Server關閉*/
        ~IndyServer2()
        {
            StopServer();
        }
        /*Server關閉*/

        /*Server啟動*/
        public void StartServer()
        {
            if (m_server != null)
            {
                // Create a ArrayList for storing SocketListeners before
                // starting the server.
                m_socketListenersList = new ArrayList();

                // Start the Server and start the thread to listen client 
                // requests.
                m_server.Start();

                m_serverThread = new Thread(new ThreadStart(ServerThreadStart));
                m_serverThread.Start();

                // Create a low priority thread that checks and deletes client
                // SocktConnection objcts that are marked for deletion.
                m_purgingThread = new Thread(new ThreadStart(PurgingThreadStart));
                m_purgingThread.Priority = ThreadPriority.Lowest;
                m_purgingThread.Start();

                
                ServerReveice = new Thread(new ThreadStart(ReceiveFromTCPListener));
                ServerReveice.Start();
                ServerSend = new Thread(new ThreadStart(SendToTCPListener));
                //ServerSend.Start();
                
                ServerIsRun = true;
                /*
                Task task1 = Task.Factory.StartNew(() => ReceiveFromTCPListener());
                Task task2 = Task.Factory.StartNew(() => SendToTCPListener());
                */

                
                
            }
        }
        /*Server啟動*/

        /*Server關閉*/
        public void StopServer()
        {
            try
            {

                if (m_server != null)
                {
                    // It is important to Stop the server first before doing
                    // any cleanup. If not so, clients might being added as
                    // server is running, but supporting data structures
                    // (such as m_socketListenersList) are cleared. This might
                    // cause exceptions.

                    // Stop the TCP/IP Server.
                    m_stopServer = true;
                    m_server.Stop();

                    // Wait for one second for the the thread to stop.
                    m_serverThread.Join(1000);

                    // If still alive; Get rid of the thread.
                    if (m_serverThread.IsAlive)
                    {
                        m_serverThread.Abort();
                    }
                    m_serverThread = null;

                    m_stopPurging = true;
                    m_purgingThread.Join(1000);
                    if (m_purgingThread.IsAlive)
                    {
                        m_purgingThread.Abort();
                    }
                    m_purgingThread = null;

                    // Free Server Object.
                    m_server = null;

                    // Stop All clients.
                    StopAllSocketListers();
                    ServerIsRun = false;
                }
            }
            catch (Exception ex)
            {

            }
        }
        /*Server關閉*/

        /// <summary>
        /// Method that stops all clients and clears the list.
        /// </summary>
        private void StopAllSocketListers()
        {
            foreach (TCPSocketListener2 socketListener in m_socketListenersList)
            {
                socketListener.StopSocketListener();
            }
            // Remove all elements from the list.
            m_socketListenersList.Clear();
            m_socketListenersList = null;
        }

        /// <summary>
        /// TCP/IP Server Thread that is listening for clients.
        /// </summary>
        Socket clientSocket = null;
        TCPSocketListener2 socketListener = null;

        private void ServerThreadStart()
        {
            // Client Socket variable;


            while (!m_stopServer)
            {
                try
                {

                    //Socket clientSocket = null;
                    //TCPSocketListener socketListener = null;
                    clientSocket = m_server.AcceptSocket();
                    socketListener = new TCPSocketListener2(clientSocket);
                    socketListener.TCPListerReceived += new EventHandler(server_Received);
                    socketListener.TCPListerSend += new EventHandler(server_Send);
                    //socketListener.TCPListerSend += new EventHandler(server_Send);

                    // Add the socket listener to an array list in a thread 
                    // safe fashon.
                    lock (m_socketListenersList)
                    {
                        m_socketListenersList.Add(socketListener);
                    }
                    // Start a communicating with the client in a different
                    // thread.
                    socketListener.StartSocketListener();
                }
                catch (SocketException se)
                {
                    m_stopServer = true;
                }
            }
        }

        /// <summary>
        /// Thread method for purging Client Listeneres that are marked for
        /// deletion (i.e. clients with socket connection closed). This thead
        /// is a low priority thread and sleeps for 10 seconds and then check
        /// for any client SocketConnection obects which are obselete and 
        /// marked for deletion.
        /// </summary>
        private void PurgingThreadStart()
        {
            while (!m_stopPurging)
            {
                ArrayList deleteList = new ArrayList();

                // Check for any clients SocketListeners that are to be
                // deleted and put them in a separate list in a thread sage
                // fashon.
                lock (m_socketListenersList)
                {
                    foreach (TCPSocketListener2 socketListener
                                 in m_socketListenersList)
                    {
                        if (socketListener.IsMarkedForDeletion())
                        {
                            deleteList.Add(socketListener);
                            socketListener.StopSocketListener();
                        }
                    }

                    // Delete all the client SocketConnection ojects which are
                    // in marked for deletion and are in the delete list.
                    for (int i = 0; i < deleteList.Count; ++i)
                    {
                        m_socketListenersList.Remove(deleteList[i]);
                    }
                }

                deleteList = null;
                Thread.Sleep(1000);
            }
        }

        public void server_Received(object sender, EventArgs e)
        {
            // TCPSocketListener Communicator_Server = (TCPSocketListener)sender;

            if (ServerReceivedEvent != null)
            {
                ServerReceivedEvent(sender, e);
            }
        }

        public void server_Send(object sender, EventArgs e)
        {
            if (ServerSendEvent != null)
            {
                //ServerSend(sender, e);
                //TCPSocketListener socketListener = null;
                //socketListener = new TCPSocketListener((Socket)sender);

                //TCPListerSend(sender, e);
                //socketListener.TCPListerSend += new EventHandler(server_Send);
                socketListener.TCPListerSendData(sender, e);
                //socketListener.TCPListerSend -= new EventHandler(server_Send);      


            }

        }


        public void ReceiveFromTCPListener()
        {
            while (ServerIsRun)
            {
                if (TCPSocketListener2.ListenerReceiveData != null && TCPSocketListener2.ListenerReceiveData != "")
                {
                    ServerReceiveData = TCPSocketListener2.ListenerReceiveData;
                    Thread.Sleep(1000);
                    if (!ServerSend.IsAlive)
                    {
                       ServerSend= new Thread(new ThreadStart(SendToTCPListener));
                        ServerSend.Start(); }
                }
            }
        }

        public void SendToTCPListener()
        {

            while (ServerIsRun)
            {                
                if (IsNeedSendBack && ServerSendMSG != null)
                {

                    TCPSocketListener2.ListenerSendData = ServerSendMSG;
                    Thread.Sleep(1000);
                    ServerSend.Abort();
                    
                }
            }
        }
    }

    public class TCPSocketListener2
    {
        //public static String DEFAULT_FILE_STORE_LOC = "C:\\TCP\\";
        public event EventHandler TCPListerReceived;

        public event EventHandler TCPListerSend;

        public static string ListenerReceiveData;
        public static byte[] ListenerSendData;
        public static bool IsNeedSendBack_Listener;
        public Socket Communicator_Listener;

        public byte[] SendDataByte;
        public Socket Communicate = null;

        LogWriter Log = new LogWriter();

        public enum STATE { FILE_NAME_READ, DATA_READ, FILE_CLOSED };

        /// <summary>
        /// Variables that are accessed by other classes indirectly.
        /// </summary>
        private Socket m_clientSocket = null;
        private bool m_stopClient = false;
        private Thread m_clientListenerThread = null;
        private bool m_markedForDeletion = false;

        /// <summary>
        /// Working Variables.
        /// </summary>
        private StringBuilder m_oneLineBuf = new StringBuilder();
        private STATE m_processState = STATE.FILE_NAME_READ;
        private long m_totalClientDataSize = 0;
        //private StreamWriter m_cfgFile = null;
        private DateTime m_lastReceiveDateTime;
        private DateTime m_currentReceiveDateTime;

        /// <summary>
        /// Client Socket Listener Constructor.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// 
        // protected virtual void IsDataReceived(EventArgs e)
        public void TCPListerReceivedData(object send, EventArgs e)
        {
            if (TCPListerReceived != null)
                TCPListerReceived(send, e);

        }

        public void TCPListerSendData(object send, EventArgs e)
        {
            byte[] yd = null;
            if (TCPListerSend != null)
            {
                ProcessClientData((Socket)send, "", yd);
            }
        }



        public TCPSocketListener2(Socket clientSocket)
        {
            m_clientSocket = clientSocket;
        }
        public TCPSocketListener2()
        {

        }
        /// <summary>
        /// Client SocketListener Destructor.
        /// </summary>
        ~TCPSocketListener2()
        {
            StopSocketListener();
        }

        /// <summary>
        /// Method that starts SocketListener Thread.
        /// </summary>
        public void StartSocketListener()
        {
            if (m_clientSocket != null)
            {
                m_clientListenerThread =
                    new Thread(new ThreadStart(SocketListenerThreadStart));

                m_clientListenerThread.Start();
            }
        }

        /// <summary>
        /// Thread method that does the communication to the client. This 
        /// thread tries to receive from client and if client sends any data
        /// then parses it and again wait for the client data to come in a
        /// loop. The recieve is an indefinite time receive.
        /// </summary>
        private void SocketListenerThreadStart()
        {
            int size = 0;
            Byte[] byteBuffer = new Byte[1024];

            m_lastReceiveDateTime = DateTime.Now;
            m_currentReceiveDateTime = DateTime.Now;

            Timer t = new Timer(new TimerCallback(CheckClientCommInterval),
                null, 15000, 15000);

            while (!m_stopClient)
            {
                try
                {
                    //TCPListerReceivedData(m_clientSocket, EventArgs.Empty);
                    size = m_clientSocket.Receive(byteBuffer);
                    m_currentReceiveDateTime = DateTime.Now;
                    ParseReceiveBuffer(byteBuffer, size);
                    //ParseReceiveBuffer(m_clientSocket, byteBuffer, size);
                }
                catch (SocketException se)
                {
                    m_stopClient = true;
                    m_markedForDeletion = true;
                }
            }
            t.Change(Timeout.Infinite, Timeout.Infinite);
            t = null;
        }

        /// <summary>
        /// Method that stops Client SocketListening Thread.
        /// </summary>
        public void StopSocketListener()
        {
            if (m_clientSocket != null)
            {
                m_stopClient = true;
                m_clientSocket.Close();

                if (m_clientListenerThread != null)
                {
                    // Wait for one second for the the thread to stop.
                    m_clientListenerThread.Join(1000);

                    // If still alive; Get rid of the thread.
                    if (m_clientListenerThread!=null || m_clientListenerThread.IsAlive)
                    {
                        m_clientListenerThread.Abort();
                    }
                }
                m_clientListenerThread = null;
                m_clientSocket = null;
                m_markedForDeletion = true;
            }
        }

        /// <summary>
        /// Method that returns the state of this object i.e. whether this
        /// object is marked for deletion or not.
        /// </summary>
        /// <returns></returns>
        public bool IsMarkedForDeletion()
        {
            return m_markedForDeletion;
        }

        /// <summary>
        /// This method parses data that is sent by a client using TCP/IP.
        /// As per the "Protocol" between client and this Listener, client 
        /// sends each line of information by appending "CRLF" (Carriage Return
        /// and Line Feed). But since the data is transmitted from client to 
        /// here by TCP/IP protocol, it is not guarenteed that each line that
        /// arrives ends with a "CRLF". So the job of this method is to make a
        /// complete line of information that ends with "CRLF" from the data
        /// that comes from the client and get it processed.
        /// </summary>
        /// <param name="byteBuffer"></param>
        /// <param name="size"></param>
        private void ParseReceiveBuffer(Byte[] byteBuffer, int size)
        {
            Object thisLock = new Object();
            string data = Encoding.ASCII.GetString(byteBuffer, 0, size);
            int lineEndIndex = 0;

            // Check whether data from client has more than one line of 
            // information, where each line of information ends with "CRLF"
            // ("\r\n"). If so break data into different lines and process
            // separately.
            do
            {
                lineEndIndex = data.IndexOf("\r\n");

                if (lineEndIndex != -1)
                {

                    ListenerReceiveData = data;
                    //TCPListerReceivedData(EventArgs.Empty);

                    lock (ListenerReceiveData)
                    {
                        Thread.Sleep(100);

                        while (ListenerSendData != null)
                        {
                            ProcessClientData(ListenerSendData);
                        }

                    }
                    /*
                    m_oneLineBuf = m_oneLineBuf.Append(data, 0, lineEndIndex + 2);
                    ProcessClientData(m_oneLineBuf.ToString());
                    m_oneLineBuf.Remove(0, m_oneLineBuf.Length);
                    data = data.Substring(lineEndIndex + 2,
                        data.Length - lineEndIndex - 2);
                     */
                }
                else
                {
                    // Just append to the existing buffer.
                    m_oneLineBuf = m_oneLineBuf.Append(data);
                }
            } while (lineEndIndex != -1);
        }


        /// <summary>
        /// Method that Process the client data as per the protocol. 
        /// The protocol works like this. 
        /// 1. Client first send a file name that ends with "CRLF".
        /// 
        /// 2. This SocketListener has to return the length of the file name
        /// to the client for validation. If the length matches with the length
        /// what  client had sent earlier, then client starts sending lines of
        /// information. Otherwise socket will be closed by the client.
        /// 
        /// 3. Each line of information that client sends will end with "CRLF".
        /// 
        /// 4. This socketListener has to store each line of information in 
        /// a text file whoose file name has been sent by the client earlier.
        /// 
        /// 5. As a last line of information client sends "[EOF]" line which
        /// also ends with "CRLF" ("\r\n"). This signals this SocketListener
        /// for an end of file and intern this SocketListener sends the total
        /// length of the data (lines of information excludes file name that
        /// was sent earlier) back to client for validation.
        /// </summary>
        /// <param name="oneLine"></param>
        private void ProcessClientData(String oneLine)
        {
            switch (m_processState)
            {
                //寫log
                case STATE.FILE_NAME_READ:
                    m_processState = STATE.DATA_READ;
                    int length = oneLine.Length;
                    if (length <= 2)
                    {
                        m_processState = STATE.FILE_CLOSED;
                        length = -1;
                    }
                    else
                    {
                        try
                        {
                            //_cfgFile = new StreamWriter(DEFAULT_FILE_STORE_LOC + oneLine.Substring(0, length - 2));
                            Log.WriterLog(oneLine);
                        }
                        catch (Exception e)
                        {
                            m_processState = STATE.FILE_CLOSED;
                            length = -1;
                        }
                    }

                    try
                    {
                        m_clientSocket.Send(BitConverter.GetBytes(length));
                    }
                    catch (SocketException se)
                    {
                        m_processState = STATE.FILE_CLOSED;
                    }
                    break;
                case STATE.DATA_READ: //送資料
                    m_totalClientDataSize += oneLine.Length;
                    //m_cfgFile.Write(oneLine);
                    //m_cfgFile.Flush();
                    Log.WriterLog(oneLine);
                    if (oneLine.ToUpper().Equals("[EOF]\r\n"))
                    {
                        try
                        {
                            //m_cfgFile.Close();
                            //m_cfgFile = null;
                            m_clientSocket.Send(BitConverter.GetBytes(m_totalClientDataSize));
                        }
                        catch (SocketException se)
                        {
                        }
                        m_processState = STATE.FILE_CLOSED;
                        m_markedForDeletion = true;
                    }
                    break;
                case STATE.FILE_CLOSED: //結束
                    break;
                default:
                    break;
            }
        }

        public void ProcessClientData(Socket _socket, String oneLine, byte[] SendData)
        {



            Log.WriterLog(oneLine);
            if (oneLine != "")
            {
                try
                {
                    //m_cfgFile.Close();
                    //m_cfgFile = null;
                    m_clientSocket.Send(SendData);
                }
                catch (SocketException se)
                {
                }

                m_markedForDeletion = true;
            }



        }

        public void ProcessClientData(byte[] SendData)
        {
            lock (m_clientSocket)
            {
                if (SendData != null)
                {
                    try
                    {
                        if (m_clientSocket.Connected)
                        {
                            m_clientSocket.Send(SendData);
                        }
                    }
                    catch (SocketException se)
                    {
                    }

                    m_markedForDeletion = true;
                }
            }
        }


        /// <summary>
        /// Method that checks whether there are any client calls for the
        /// last 15 seconds or not. If not this client SocketListener will
        /// be closed.
        /// </summary>
        /// <param name="o"></param>
        private void CheckClientCommInterval(object o)
        {
            if (m_lastReceiveDateTime.Equals(m_currentReceiveDateTime))
            {
                this.StopSocketListener();
            }
            else
            {
                m_lastReceiveDateTime = m_currentReceiveDateTime;
            }
        }
    }

    /*000000000000000Static*/
    public static class IndyServer
    {
        /// <summary>
        /// Default Constants.
        /// </summary>
        public static IPAddress DEFAULT_SERVER = IPAddress.Parse("127.0.0.1");
        public static int DEFAULT_PORT = 31001;
        public static IPEndPoint DEFAULT_IP_END_POINT = new IPEndPoint(DEFAULT_SERVER, DEFAULT_PORT);

        /// <summary>
        /// Local Variables Declaration.
        /// </summary>
        private static TcpListener m_server = null;
        private static bool m_stopServer = false;
        private static bool m_stopPurging = false;

        private static Thread m_serverThread = null;
        //private ParameterizedThreadStart m_serverThread = null;

        private static Thread m_purgingThread = null;
        private static ArrayList m_socketListenersList = null;

        public static bool ServerRun = false;



        /*Server初始化*/


        public static void Init(string IP, int Port)
        {
            try
            {
                m_server = new TcpListener(IPAddress.Parse(IP), Port);
            }
            catch (Exception e)
            {
                m_server = null;
            }
        }
        /*Server初始化*/


        /*Server啟動*/
        public static void StartServer()
        {
            if (m_server != null)
            {
                // Create a ArrayList for storing SocketListeners before
                // starting the server.
                m_socketListenersList = new ArrayList();

                // Start the Server and start the thread to listen client 
                // requests.
                m_server.Start();

                m_serverThread = new Thread(new ThreadStart(ServerThreadStart));
                m_serverThread.Start();

                // Create a low priority thread that checks and deletes client
                // SocktConnection objcts that are marked for deletion.
                m_purgingThread = new Thread(new ThreadStart(PurgingThreadStart));
                m_purgingThread.Priority = ThreadPriority.Lowest;
                m_purgingThread.Start();
                ServerRun = true;
            }
        }
        /*Server啟動*/

        /*Server關閉*/
        public static void StopServer()
        {
            try
            {
                if (m_server != null)
                {
                    // It is important to Stop the server first before doing
                    // any cleanup. If not so, clients might being added as
                    // server is running, but supporting data structures
                    // (such as m_socketListenersList) are cleared. This might
                    // cause exceptions.

                    // Stop the TCP/IP Server.
                    m_stopServer = true;
                    m_server.Stop();

                    // Wait for one second for the the thread to stop.
                    m_serverThread.Join(1000);

                    // If still alive; Get rid of the thread.
                    if (m_serverThread.IsAlive)
                    {
                        m_serverThread.Abort();
                    }
                    m_serverThread = null;

                    m_stopPurging = true;
                    m_purgingThread.Join(1000);
                    if (m_purgingThread.IsAlive)
                    {
                        m_purgingThread.Abort();
                    }
                    m_purgingThread = null;

                    // Free Server Object.
                    m_server = null;

                    // Stop All clients.
                    StopAllSocketListers();
                }
            }
            catch (Exception ex)
            {

            }
        }
        /*Server關閉*/

        /// <summary>
        /// Method that stops all clients and clears the list.
        /// </summary>
        private static void StopAllSocketListers()
        {
            foreach (TCPSocketListener2 socketListener in m_socketListenersList)
            {
                socketListener.StopSocketListener();
            }
            // Remove all elements from the list.
            m_socketListenersList.Clear();
            m_socketListenersList = null;
        }

        /// <summary>
        /// TCP/IP Server Thread that is listening for clients.
        /// </summary>


        private static void ServerThreadStart()
        {
            // Client Socket variable;


            while (!m_stopServer)
            {
                try
                {

                    Socket clientSocket = null;
                    /*
                     TCPSocketListener socketListener = null;
                     clientSocket = m_server.AcceptSocket();
                     socketListener = new TCPSocketListener(clientSocket);              
                     */
                    IndySocketListener.m_clientSocket = m_server.AcceptSocket();
                    // Add the socket listener to an array list in a thread 
                    // safe fashon.
                    lock (m_socketListenersList)
                    {
                        // m_socketListenersList.Add(socketListener);
                        m_socketListenersList.Add(IndySocketListener.m_clientSocket);
                    }
                    // Start a communicating with the client in a different
                    // thread.
                    //socketListener.StartSocketListener();
                    IndySocketListener.StartSocketListener();
                }
                catch (SocketException se)
                {
                    m_stopServer = true;
                }
            }
        }

        /// <summary>
        /// Thread method for purging Client Listeneres that are marked for
        /// deletion (i.e. clients with socket connection closed). This thead
        /// is a low priority thread and sleeps for 10 seconds and then check
        /// for any client SocketConnection obects which are obselete and 
        /// marked for deletion.
        /// </summary>
        private static void PurgingThreadStart()
        {
            while (!m_stopPurging)
            {
                ArrayList deleteList = new ArrayList();

                // Check for any clients SocketListeners that are to be
                // deleted and put them in a separate list in a thread sage
                // fashon.
                lock (m_socketListenersList)
                {
                    /*
                    foreach (TCPSocketListener2 socketListener
                               in m_socketListenersList)
                    {
                        if (socketListener.IsMarkedForDeletion())
                        {
                            deleteList.Add(socketListener);
                            socketListener.StopSocketListener();
                        }
                    }
                    */
                    for (int Start = 0; Start < m_socketListenersList.Count; Start++)
                    {
                        /*
                        if (socketListener.IsMarkedForDeletion())
                        {
                            deleteList.Add(socketListener);
                            socketListener.StopSocketListener();
                        }
                        */

                    }

                    // Delete all the client SocketConnection ojects which are
                    // in marked for deletion and are in the delete list.
                    for (int i = 0; i < deleteList.Count; ++i)
                    {
                        m_socketListenersList.Remove(deleteList[i]);
                    }
                }

                deleteList = null;
                Thread.Sleep(1000);
            }
        }


    }

    public static class IndySocketListener
    {
        public static string ReceiveData;
        public static string SendData;
        public static byte[] SendDataByte;


        static LogWriter Log = new LogWriter();

        public enum STATE { FILE_NAME_READ, DATA_READ, FILE_CLOSED };

        /// <summary>
        /// Variables that are accessed by other classes indirectly.
        /// </summary>
        public static Socket m_clientSocket = null;
        private static bool m_stopClient = false;
        private static Thread m_clientListenerThread = null;
        private static bool m_markedForDeletion = false;

        /// <summary>
        /// Working Variables.
        /// </summary>
        private static StringBuilder m_oneLineBuf = new StringBuilder();
        private static STATE m_processState = STATE.FILE_NAME_READ;
        private static long m_totalClientDataSize = 0;
        //private StreamWriter m_cfgFile = null;
        private static DateTime m_lastReceiveDateTime;
        private static DateTime m_currentReceiveDateTime;

        /// <summary>
        /// Client Socket Listener Constructor.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// 
        // protected virtual void IsDataReceived(EventArgs e)



        /// <summary>
        /// Method that starts SocketListener Thread.
        /// </summary>
        public static void StartSocketListener()
        {
            if (m_clientSocket != null)
            {
                m_clientListenerThread =
                    new Thread(new ThreadStart(SocketListenerThreadStart));

                m_clientListenerThread.Start();
            }
        }

        /// <summary>
        /// Thread method that does the communication to the client. This 
        /// thread tries to receive from client and if client sends any data
        /// then parses it and again wait for the client data to come in a
        /// loop. The recieve is an indefinite time receive.
        /// </summary>
        private static void SocketListenerThreadStart()
        {
            int size = 0;
            Byte[] byteBuffer = new Byte[1024];

            m_lastReceiveDateTime = DateTime.Now;
            m_currentReceiveDateTime = DateTime.Now;

            Timer t = new Timer(new TimerCallback(CheckClientCommInterval),
                null, 15000, 15000);

            while (!m_stopClient)
            {
                try
                {

                    size = m_clientSocket.Receive(byteBuffer);
                    m_currentReceiveDateTime = DateTime.Now;
                    ParseReceiveBuffer(byteBuffer, size);
                    //ParseReceiveBuffer(m_clientSocket, byteBuffer, size);
                }
                catch (SocketException se)
                {
                    m_stopClient = true;
                    m_markedForDeletion = true;
                }
            }
            t.Change(Timeout.Infinite, Timeout.Infinite);
            t = null;
        }

        /// <summary>
        /// Method that stops Client SocketListening Thread.
        /// </summary>
        public static void StopSocketListener()
        {
            if (m_clientSocket != null)
            {
                m_stopClient = true;
                m_clientSocket.Close();

                if (m_clientListenerThread != null)
                {
                    // Wait for one second for the the thread to stop.
                    m_clientListenerThread.Join(1000);

                    // If still alive; Get rid of the thread.
                    if (m_clientListenerThread.IsAlive)
                    {
                        m_clientListenerThread.Abort();
                    }
                }
                m_clientListenerThread = null;
                m_clientSocket = null;
                m_markedForDeletion = true;
            }
        }

        /// <summary>
        /// Method that returns the state of this object i.e. whether this
        /// object is marked for deletion or not.
        /// </summary>
        /// <returns></returns>
        public static bool IsMarkedForDeletion()
        {
            return m_markedForDeletion;
        }

        /// <summary>
        /// This method parses data that is sent by a client using TCP/IP.
        /// As per the "Protocol" between client and this Listener, client 
        /// sends each line of information by appending "CRLF" (Carriage Return
        /// and Line Feed). But since the data is transmitted from client to 
        /// here by TCP/IP protocol, it is not guarenteed that each line that
        /// arrives ends with a "CRLF". So the job of this method is to make a
        /// complete line of information that ends with "CRLF" from the data
        /// that comes from the client and get it processed.
        /// </summary>
        /// <param name="byteBuffer"></param>
        /// <param name="size"></param>
        public static void ParseReceiveBuffer(Byte[] byteBuffer, int size)
        {
            string data = Encoding.ASCII.GetString(byteBuffer, 0, size);
            int lineEndIndex = 0;

            // Check whether data from client has more than one line of 
            // information, where each line of information ends with "CRLF"
            // ("\r\n"). If so break data into different lines and process
            // separately.
            do
            {
                lineEndIndex = data.IndexOf("\r\n");

                if (lineEndIndex != -1)
                {
                    ReceiveData = data;
                    //TCPListerReceivedData(EventArgs.Empty);

                    /*
                    m_oneLineBuf = m_oneLineBuf.Append(data, 0, lineEndIndex + 2);
                    ProcessClientData(m_oneLineBuf.ToString());
                    m_oneLineBuf.Remove(0, m_oneLineBuf.Length);
                    data = data.Substring(lineEndIndex + 2,
                        data.Length - lineEndIndex - 2);
                     */
                }
                else
                {
                    // Just append to the existing buffer.
                    m_oneLineBuf = m_oneLineBuf.Append(data);
                }
            } while (lineEndIndex != -1);

        }


        private static void ParseReceiveBuffer2(Byte[] byteBuffer, int size)
        {
            string data = Encoding.ASCII.GetString(byteBuffer, 0, size);
            int lineEndIndex = 0;

            // Check whether data from client has more than one line of 
            // information, where each line of information ends with "CRLF"
            // ("\r\n"). If so break data into different lines and process
            // separately.
            do
            {
                lineEndIndex = data.IndexOf("\r\n");

                if (lineEndIndex != -1)
                {
                    ReceiveData = data;
                    //TCPListerReceivedData(EventArgs.Empty);

                    /*
                    m_oneLineBuf = m_oneLineBuf.Append(data, 0, lineEndIndex + 2);
                    ProcessClientData(m_oneLineBuf.ToString());
                    m_oneLineBuf.Remove(0, m_oneLineBuf.Length);
                    data = data.Substring(lineEndIndex + 2,
                        data.Length - lineEndIndex - 2);
                     */
                }
                else
                {
                    // Just append to the existing buffer.
                    m_oneLineBuf = m_oneLineBuf.Append(data);
                }
            } while (lineEndIndex != -1);
        }

        /// <summary>
        /// Method that Process the client data as per the protocol. 
        /// The protocol works like this. 
        /// 1. Client first send a file name that ends with "CRLF".
        /// 
        /// 2. This SocketListener has to return the length of the file name
        /// to the client for validation. If the length matches with the length
        /// what  client had sent earlier, then client starts sending lines of
        /// information. Otherwise socket will be closed by the client.
        /// 
        /// 3. Each line of information that client sends will end with "CRLF".
        /// 
        /// 4. This socketListener has to store each line of information in 
        /// a text file whoose file name has been sent by the client earlier.
        /// 
        /// 5. As a last line of information client sends "[EOF]" line which
        /// also ends with "CRLF" ("\r\n"). This signals this SocketListener
        /// for an end of file and intern this SocketListener sends the total
        /// length of the data (lines of information excludes file name that
        /// was sent earlier) back to client for validation.
        /// </summary>
        /// <param name="oneLine"></param>
        private static void ProcessClientData(String oneLine)
        {
            switch (m_processState)
            {
                //寫log
                case STATE.FILE_NAME_READ:
                    m_processState = STATE.DATA_READ;
                    int length = oneLine.Length;
                    if (length <= 2)
                    {
                        m_processState = STATE.FILE_CLOSED;
                        length = -1;
                    }
                    else
                    {
                        try
                        {
                            //_cfgFile = new StreamWriter(DEFAULT_FILE_STORE_LOC + oneLine.Substring(0, length - 2));
                            Log.WriterLog(oneLine);
                        }
                        catch (Exception e)
                        {
                            m_processState = STATE.FILE_CLOSED;
                            length = -1;
                        }
                    }

                    try
                    {
                        m_clientSocket.Send(BitConverter.GetBytes(length));
                    }
                    catch (SocketException se)
                    {
                        m_processState = STATE.FILE_CLOSED;
                    }
                    break;
                case STATE.DATA_READ: //送資料
                    m_totalClientDataSize += oneLine.Length;
                    //m_cfgFile.Write(oneLine);
                    //m_cfgFile.Flush();
                    Log.WriterLog(oneLine);
                    if (oneLine.ToUpper().Equals("[EOF]\r\n"))
                    {
                        try
                        {
                            //m_cfgFile.Close();
                            //m_cfgFile = null;
                            m_clientSocket.Send(BitConverter.GetBytes(m_totalClientDataSize));
                        }
                        catch (SocketException se)
                        {
                        }
                        m_processState = STATE.FILE_CLOSED;
                        m_markedForDeletion = true;
                    }
                    break;
                case STATE.FILE_CLOSED: //結束
                    break;
                default:
                    break;
            }
        }

        public static void ProcessClientData(Socket _socket, String oneLine, byte[] SendData)
        {



            Log.WriterLog(oneLine);
            if (oneLine != "")
            {
                try
                {
                    //m_cfgFile.Close();
                    //m_cfgFile = null;
                    m_clientSocket.Send(SendData);
                }
                catch (SocketException se)
                {
                }

                m_markedForDeletion = true;
            }



        }

        /// <summary>
        /// Method that checks whether there are any client calls for the
        /// last 15 seconds or not. If not this client SocketListener will
        /// be closed.
        /// </summary>
        /// <param name="o"></param>
        private static void CheckClientCommInterval(object o)
        {
            if (m_lastReceiveDateTime.Equals(m_currentReceiveDateTime))
            {
                StopSocketListener();
            }
            else
            {
                m_lastReceiveDateTime = m_currentReceiveDateTime;
            }
        }
    }
}
