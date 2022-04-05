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
            Console.WriteLine("Клієнт посилає запит на сервер :)");
            IPAddress serverIP = IPAddress.Parse("91.238.103.51");
            IPEndPoint serverEndPoint = new IPEndPoint(serverIP, 2076);
            Socket clientSender = new Socket(serverIP.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            clientSender.Connect(serverEndPoint);
            Console.WriteLine("Socket connectec to {0}",
                serverEndPoint);
            string message = Console.ReadLine();

            byte[] bytes = Encoding.ASCII.GetBytes(message);
            clientSender.Send(bytes); //відпраивти на вервер запит

            clientSender.Shutdown(SocketShutdown.Both);
            clientSender.Close();
        }
    }
}
