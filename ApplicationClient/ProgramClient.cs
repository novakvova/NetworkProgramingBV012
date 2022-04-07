using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ApplicationClient
{
    class ProgramClient
    {
        static void Main(string[] args)
        {
            //Console.InputEncoding = Encoding.UTF8;
            //Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Клієнт посилає запит на сервер :)");
            //string ip="127.0.0.1";
            string ip= "91.238.103.51";
            IPAddress serverIP = IPAddress.Parse(ip);
            IPEndPoint serverEndPoint = new IPEndPoint(serverIP, 2076);
            Socket clientSender = new Socket(serverIP.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            clientSender.Connect(serverEndPoint);
            Console.WriteLine("Socket connecte to {0}",
                serverEndPoint);
            string message = Console.ReadLine();

            byte[] bytes = Encoding.UTF8.GetBytes(message);
            clientSender.Send(bytes); //відпраивти на вервер запит

            byte[] serverResponse=new byte[1024];
            //Отримую відповідь від сервера
            clientSender.Receive(serverResponse);
            string serverString = Encoding.UTF8.GetString(serverResponse);
            Console.WriteLine("Server response: {0}", serverString);

            clientSender.Shutdown(SocketShutdown.Both);
            clientSender.Close();
        }
    }
}
