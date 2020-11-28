using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Marcin_Domek.Src
{
    class Client
    {
        private IPHostEntry IPHost { get; }
        private IPAddress IPAddress { get; }
        private IPEndPoint LocalEndPoint { get; }
        private Socket SenderSocket { get; }

        public Client()
        {
            IPHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress = IPHost.AddressList[0];
            LocalEndPoint = new IPEndPoint(IPAddress, 12121);

            SenderSocket = new Socket(IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            Open();
            SendMessage();
            Close();
        }

        private void Open()
        {
            try
            {
                SenderSocket.Connect(LocalEndPoint);
            }
            catch (ArgumentNullException ane)
            {

                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {

                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }

        private void SendMessage()
        {
            try
            {
                Trace.WriteLine("Socket connected to: " + SenderSocket.RemoteEndPoint.ToString());

                byte[] messageSent = Encoding.UTF8.GetBytes("Test Client<EOF>");
                int byteSent = SenderSocket.Send(messageSent);

                byte[] messageReceived = new byte[1024];

                int byteRecv = SenderSocket.Receive(messageReceived);
                Trace.WriteLine("Message from Server: " + Encoding.UTF8.GetString(messageReceived, 0, byteRecv));
            }
            catch (ArgumentNullException ane)
            {

                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {

                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }


        private void Close()
        {
            try
            {
                SenderSocket.Shutdown(SocketShutdown.Both);
                SenderSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
