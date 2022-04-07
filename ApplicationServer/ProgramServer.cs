using Bogus;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ApplicationServer
{
    class ProgramServer
    {
        static void Main(string[] args)
        {

            var fakeUser = new Faker<User>("uk")
                    .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName())
                    .RuleFor(u => u.LastName, (f, u) => f.Name.LastName());
            
            //ctrl+A - виділити все
            //ctrl+K+F - форматує текст
            //Console.InputEncoding = Encoding.Unicode;
            //Console.OutputEncoding = Encoding.Unicode;
            string ipAdress = "127.0.0.1";
            Console.WriteLine("Enter Server ip: ");
            ipAdress = Console.ReadLine();
            if (string.IsNullOrEmpty(ipAdress))
                ipAdress = "91.238.103.51";
            int port = 2076;

            IPAddress serverIP = IPAddress.Parse(ipAdress);
            //Кінцева точка
            IPEndPoint endPoint = new IPEndPoint(serverIP, port);
            //
            Socket socket = new Socket(serverIP.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            //Сервер буде отримувати запити на дану кінцеву точнку
            socket.Bind(endPoint); 
            socket.Listen(10); //черга яка може бути до сервера
            Console.WriteLine("Сервер очікує запитів від клієнтів ...");
            while (true)
            {
                //Клієнт, який відправив запит на серве
                Socket client = socket.Accept(); //Отримати запит від клієнта
                byte[] bytes = new byte[1024];
                //Очкікує дані від клієнта 
                int size = client.Receive(bytes); // ASCII - 1 байт - Hello - 5 Байт
                String text = Encoding.UTF8.GetString(bytes);
                Console.WriteLine("Client: \n message {0} \n ip: {1}", text, client.RemoteEndPoint.ToString());
                //Відповідь клієнту від сервера
                var user = fakeUser.Generate();
                byte[] clintSendText = Encoding.UTF8.GetBytes($"{DateTime.Now} {user}");
                client.Send(clintSendText);
                //завершаємо роботу із клієнтом
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            //for (int i = 0; i < size; i++)
            //{
            //    Console.WriteLine("bytes {0}",bytes[i]);
            //}
        }
    }
}
