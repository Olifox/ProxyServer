using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proxy
{
    public class Client
    {
        private TcpClient client;
        private SitesPool sp;

        public Client(TcpClient client, SitesPool sp)
        {
            this.client = client;
            this.sp = sp;
            Console.WriteLine("Start new client");
        }

        public void Start()
        {

            var Request = ""; // хранит сам запрос полученный от пользователя
            var RequestUri = ""; // URI вытащенный из запроса (куда стучаться кароч)
            var receivebuffer = new List<byte>(); //буфер для ответа, листом ибо ответ хз каких размеров
            var Buffer = new byte[4096]; //буфер для запроса, тоже сделал бы листом но стрим читает только в байт массив

            var Count = client.GetStream().Read(Buffer, 0, Buffer.Length); //читаем запрос
            Request += Encoding.ASCII.GetString(Buffer, 0, Count); // переводим из байт кода в буквы

            Match ReqMatch = Regex.Match(Request, @"\s[\S]+\s(HTTP)"); // вытаскиваем адрес
            RequestUri = ReqMatch.ToString().Trim(new char[] { ' ', 'H', 'T', 'P' }); // убираем лишнее
            if (Regex.IsMatch(RequestUri, @"lostfilm.tv"))
                Console.WriteLine();
            if (String.IsNullOrEmpty(RequestUri)) //пустой адрес
                return;
            if (!sp.Validate(RequestUri)) //запрещенный
            {
                Console.WriteLine("Упс");
                return;
            }
            if (Regex.IsMatch(RequestUri, @"[\w.]+:[\d]{3}")) // Https, так и не сделал, скажи что не смогла, тип тут заебы пиздец
            {
                Console.WriteLine("HTTPS {0}", RequestUri);
                // TODO
            }
            else
            {
                Console.WriteLine(RequestUri);
                try
                {
                    var wc = new WebClient().DownloadData(RequestUri); // получаем ответ с указанного URI
                    client.GetStream().Write(wc, 0, wc.Length); // отдаем его клиенту
                }
                catch
                {
                    Console.WriteLine();
                } // иногда не находит страницу поэтому на всякий случай 
                  // (в частности из-за этого не робит вк нормально ибо "http://vk.com/css/s_yzg.css?196" нот фаунд
            }
        }

        public void Stop()
        {
            client.Close();
        }
    }
}