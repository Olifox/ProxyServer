using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Proxy
{
    class HttpServer
    {
        TcpListener Listener; // Эта хрень принимает клиентов (слушатель)
        SitesPool sitesPool; // Класс для хранения запрещеночки

        public HttpServer(int Port)
        {           
            Listener = new TcpListener(IPAddress.Any, Port); // Слушатель вешается на конкретный порт и перехватывает все запросы со всех IP (в данном случае)
            Listener.Start(); // Стартуем
            sitesPool = new SitesPool("Sites.txt"); // Иницализация

            while (true)
            {
                // Когда приходит клиент срабатывает AcceptSocket и вызывается ClientThread
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), Listener.AcceptTcpClient());
            }
        }

        void ClientThread(Object StateInfo)
        {
            // Просто создаем новый экземпляр класса Client и передаем ему приведенный к классу TcpClient объект StateInfo
            var client=new Client((TcpClient)StateInfo,sitesPool);
            // Собсна запускаем саму обработку нашего клиента
            client.Start();
            client.Stop();
        }

        // это для того что бы при остановке сервер не остался висеть на порту
        ~HttpServer()
        {
            if (Listener != null)
            {
                Listener.Stop();
            }
        }

        static void Main(string[] args)
        {
            // Максимальное кол-во потоков, в данном случае 4 на каждый проц
            int MaxThreadsCount = Environment.ProcessorCount * 4;
            // Максимальное колличество рабочих потоков
            ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
            // Минимальное количество рабочих потоков
            ThreadPool.SetMinThreads(2, 2);
            // Создаем сервер на порту
            new HttpServer(8888);
        }
    }
}
