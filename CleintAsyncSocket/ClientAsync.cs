using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CleintAsyncSocket
{
    class ClientAsync
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Клієнт посилає запит на сервер :)");
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint serverEndPoint = new IPEndPoint(ipAddress, 1100);
            Socket clientSender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            clientSender.Connect(serverEndPoint);
            Console.WriteLine("Socket connecte to {0}",
                serverEndPoint);
            string message = Console.ReadLine();

            byte[] bytes = Encoding.UTF8.GetBytes(message);
            clientSender.Send(bytes); //відпраивти на вервер запит

            byte[] serverResponse = new byte[1024];
            //Отримую відповідь від сервера
            clientSender.Receive(serverResponse);
            string serverString = Encoding.UTF8.GetString(serverResponse);
            Console.WriteLine("Server response: {0}", serverString);

            clientSender.Shutdown(SocketShutdown.Both);
            clientSender.Close();
        }
    }
}
