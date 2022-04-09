using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerAsyncSocket
{
    public class StateObject
    {
        // Size of receive buffer.  
        public const int BufferSize = 1024;

        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];

        // Received data string.
        public StringBuilder sb = new StringBuilder();

        // Client socket.
        public Socket workSocket = null;
    }
    class ServerAsync
    {
        private const int port = 1100;
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        static void Main(string[] args)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1"); // ipHostInfo.AddressList[0];
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

            // Create the state object.  
            //Асихроне читання даних від клієнта
            StateObject state = new StateObject();
            state.workSocket = client;  //Зберігаємо кліжнта із якого читаємо дані
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
            //Thread.Sleep(4000);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;
            //Отримуємо дані про клієнта
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            //Читаємо дані із сокета
            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read
                // more data.  
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the
                    // client. Display it on the console.  
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);
                    // Echo the data back to the client.  
                    Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
