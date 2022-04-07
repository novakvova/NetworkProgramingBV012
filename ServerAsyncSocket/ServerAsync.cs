using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerAsyncSocket
{
    class ServerAsync
    {
        private const int port = 1100;
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        static void Main(string[] args)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
            Socket server = new Socket(ipAddress.AddressFamily, 
                SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Bind(endPoint);
                server.Listen(100);
                while(true)
                {
                    allDone.Reset();
                    Console.WriteLine("Thread id server {0}", Thread.CurrentThread.ManagedThreadId);
                    Console.WriteLine("Server start {0}", endPoint);
                    //Якщо прийде запит від клієнта
                    server.BeginAccept(AcceptCallback, server);
                    allDone.WaitOne();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Server error {0}", ex.Message);
            }
            Console.WriteLine("Hello World!");
        }
        //Обробка асихрого запита від клієнта
        public static void AcceptCallback(IAsyncResult ar) 
        {
            allDone.Set();
            Socket server = (Socket)ar.AsyncState;
            Socket client = server.EndAccept(ar); //отримали запит від клієнта
            Console.WriteLine("Thread id client server accept {0}", Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Client info {0}", client.RemoteEndPoint);
            //Thread.Sleep(4000);
        }
    }
}
