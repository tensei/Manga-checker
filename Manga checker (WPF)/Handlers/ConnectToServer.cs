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
        Base64 base64 = new Base64();
        JObject msg = new JObject();
        
        public void Connect()
        {
            var config = cnf.GetMangaConfig().ToString();
            var basestr = base64.Base64Encode(config);
            debug.Write("Client Started");
            if (!basestr.Equals("null"))
            {
                msg["config"] = basestr;
            }
            else
            {
                msg["config"] = config;
            }
            msg["msg"] = "Connected!";
            msg["time"] = DateTime.Now;
            msg["pcname"] = Environment.MachineName;
            
            while (Settings.Default.ThreadStatus)
            {
                try
                {
                    clientSocket.Connect("ts.overrustlelogs.net", 8888);
                    debug.Write($"[{DateTime.Now}] Client Socket Program - Server Connected ...");
                    serverStream = clientSocket.GetStream();
                    byte[] inStream = new byte[10025];
                    Int32 bytes = serverStream.Read(inStream, 0, inStream.Length);
                    string returndata = Encoding.ASCII.GetString(inStream, 0, bytes);
                    debug.Write("Data from Server : " + returndata);
                    Thread.Sleep(1000);
                    send(msg.ToString());
                    break;
                }
                catch (Exception e)
                {
                    debug.Write($"[{DateTime.Now}] {e.Message}");
                    Thread.Sleep(10000);
                }
                if (!Settings.Default.ThreadStatus)
                {
                    break;
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
                        clientSocket.Connect("ts.overrustlelogs.net", 8888);
                        serverStream = clientSocket.GetStream();
                        byte[] inStream = new byte[10025];
                        Int32 bytes = serverStream.Read(inStream, 0, inStream.Length);
                        string returndata = Encoding.ASCII.GetString(inStream, 0, bytes);
                        debug.Write("Data from Server : " + returndata);
                        msg["msg"] = "Connected!";
                        debug.Write($"[{DateTime.Now}] Client Socket Program - Server Connected ...");
                        Thread.Sleep(1000);
                        send(msg.ToString());
                    }
                    else
                    {
                        msg["msg"] = "PING";
                        send(msg.ToString());
                        Thread.Sleep(10000);
                    }
                }
                catch (Exception es)
                {
                    debug.Write($"[{DateTime.Now}] {es.Message}");
                    Thread.Sleep(10000);
                }
                if (!Settings.Default.ThreadStatus)
                {
                    break;
                }

            }
            if (clientSocket.Connected)
            {
                debug.Write($"[{DateTime.Now}] closing connection.");
                msg["msg"] = "closing connection...";
                Thread.Sleep(1000);
                clientSocket.Close();
                debug.Write($"[{DateTime.Now}] connection closed!");
            }
            else
            {
                debug.Write($"[{DateTime.Now}] Server is Offline no connection to close");
            }
            debug.Write($"[{DateTime.Now}] Thread closed");
        }

        private void send(string text)
        {
            try
            {
                NetworkStream networkStream = clientSocket.GetStream();
                byte[] outStream = Encoding.UTF8.GetBytes(text + "$");
                networkStream.Write(outStream, 0, outStream.Length);
                networkStream.Flush();
                
            }
            catch (Exception m)
            {
                debug.Write(m.Message);
            }
        }
    }
}
