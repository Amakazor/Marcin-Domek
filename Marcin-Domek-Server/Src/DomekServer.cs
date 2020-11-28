using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Marcin_Domek_Server.Src
{
    class DomekServer
    {
        private IPHostEntry IPHost { get; }
        private IPAddress IPAddress { get; }
        private IPEndPoint LocalEndPoint { get; }
        private Socket ListnenerSocket { get; }

        public DomekServer()
        {
            IPHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress = IPHost.AddressList[0];
            LocalEndPoint = new IPEndPoint(IPAddress, 12121);

            ListnenerSocket = new Socket(IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            Open();
        }

        private void Open()
        {
            try
            {
                ListnenerSocket.Bind(LocalEndPoint);
                ListnenerSocket.Listen(10);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Close()
        {
            try
            {
                ListnenerSocket.Shutdown(SocketShutdown.Both);
                ListnenerSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Loop()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Awaiting connection...");

                    Socket clientSocket = ListnenerSocket.Accept();

                    byte[] bytes = new byte[1024];
                    string data = null;

                    while (true)
                    {
                        int numByte = clientSocket.Receive(bytes);

                        data += Encoding.UTF8.GetString(bytes, 0, numByte);

                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                    Console.WriteLine("Text received: " + data);
                    byte[] message = Encoding.UTF8.GetBytes("Test Server");

                    clientSocket.Send(message);
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            catch (Exception e)
            {
                Close();
                Console.WriteLine(e.ToString());
            }
        }
    }
}
