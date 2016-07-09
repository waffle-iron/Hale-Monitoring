using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Hale.Alert
{
    // Taken from http://stackoverflow.com/questions/372742/can-i-test-smtpclient-before-calling-client-send
    public static class SmtpHelper
    {

        /// <summary>
        /// test the smtp connection by sending a HELO command
        /// </summary>
        /// <param name="smtpServerAddress"></param>
        /// <param name="port"></param>
        public static bool TestConnection(string smtpServerAddress, int port, out string response)
        {
            IPAddress hostAddress = Dns.GetHostAddresses(smtpServerAddress)[0];
            IPEndPoint endPoint = new IPEndPoint(hostAddress, port);
            using (Socket tcpSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                string responseConnect;

                //try to connect and test the rsponse for code 220 = success
                tcpSocket.Connect(endPoint);
                if (!CheckResponse(tcpSocket, 220, out responseConnect))
                {
                    response = responseConnect;
                    return false;
                }

                string responseHelo;

                // send HELO and test the response for code 250 = proper response
                SendData(tcpSocket, string.Format("HELO {0}\r\n", Dns.GetHostName()));
                if (!CheckResponse(tcpSocket, 250, out responseHelo))
                {
                    response = string.Concat(responseConnect, "\n", responseHelo);
                    return false;
                }

                response = string.Concat(responseConnect, "\n", responseHelo);

                // if we got here it's that we can connect to the smtp server
                return true;
            }
        }

        public static bool TestConnection(string smtpServerAddress, int port)
        {
            string dummy;
            return TestConnection(smtpServerAddress, port, out dummy);
        }

        private static void SendData(Socket socket, string data)
        {
            byte[] dataArray = Encoding.ASCII.GetBytes(data);
            socket.Send(dataArray, 0, dataArray.Length, SocketFlags.None);
        }

        private static bool CheckResponse(Socket socket, int expectedCode, out string response)
        {
            while (socket.Available == 0)
            {
                System.Threading.Thread.Sleep(100);
            }
            byte[] responseArray = new byte[1024];
            socket.Receive(responseArray, 0, socket.Available, SocketFlags.None);
            string responseData = Encoding.ASCII.GetString(responseArray);
            int responseCode = Convert.ToInt32(responseData.Substring(0, 3));
            response = responseData;
            if (responseCode == expectedCode)
            {
                return true;
            }
            return false;
        }
    }
}
