using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Manga_checker.Properties;
using Newtonsoft.Json.Linq;

namespace Manga_checker.Handlers
{
    class ConnectToServer
    {
        TcpClient clientSocket = new TcpClient();
        NetworkStream serverStream;
        DebugText debug = new DebugText();
        Config cnf = new Config();
        
        JObject msg = new JObject();
        
        public void Connect()
        {
            var config = cnf.GetMangaConfig().ToString();
            debug.Write("Client Started");
            msg["config"] = config;
            msg["msg"] = "Connected!";
            msg["time"] = DateTime.Now;
            msg["pcname"] = Environment.MachineName;
            while (Settings.Default.ThreadStatus)
            {
                try
                {
                    clientSocket.Connect("192.168.1.50", 8888);
                    debug.Write($"[{DateTime.Now}] Client Socket Program - Server Connected ...");
                    Thread.Sleep(1000);
                    send(msg.ToString());
                    break;
                }
                catch (Exception)
                {
                    //debug.Write($"[{DateTime.Now}] Trying to connect to Server.2");
                    Thread.Sleep(10000);
                }
                    
            }
            
            while (Settings.Default.ThreadStatus)
            {
                try
                {
                    if (!clientSocket.Connected)
                    {
                        clientSocket.Dispose();
                        //debug.Write($"[{DateTime.Now}] Trying to connect to Server.1");
                        clientSocket = new TcpClient();
                        clientSocket.Connect("192.168.1.50", 8888);
                        msg["msg"] = "Connected!";
                        debug.Write($"[{DateTime.Now}] Client Socket Program - Server Connected ...");
                        Thread.Sleep(1000);
                        send(msg.ToString());
                    }
                    else
                    {
                        msg["msg"] = "PING";
                        send(msg.ToString());
                        Thread.Sleep(5000);
                    }
                }
                catch (Exception)
                {
                    //debug.Write($"[{DateTime.Now}] {es.Message}");
                    Thread.Sleep(5000);
                }

            }
            if (clientSocket.Connected)
            {
                debug.Write($"[{DateTime.Now}] closing connection.");
                msg["msg"] = "closing connection...";
                send(msg.ToString());
                clientSocket.Close();
                debug.Write($"[{DateTime.Now}] connection closed!");
            }
            else
            {
                debug.Write($"[{DateTime.Now}] Server is Offline no connection to close");
            }
        }

        private void send(string text)
        {
            NetworkStream networkStream = clientSocket.GetStream();
            byte[] outStream = Encoding.ASCII.GetBytes(text+"$");
            networkStream.Write(outStream, 0, outStream.Length);
            networkStream.Flush();
            //byte[] inStream = new byte[10025];
            //networkStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
            //string returndata = System.Text.Encoding.ASCII.GetString(inStream);
            //debug.Write("Data from Server : " + returndata);
        }

    }
}
