using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Task_1_Client
{
    class Program
    {
        const string ip = "127.0.0.1";
        const int port = 8080;

        static void Main(string[] args)
        {
            Socket socket = null;
            string path = "phrases.txt";
            List<string> phrases = new List<string>();
            Random rand = new Random();

            StreamReader stream_r = new StreamReader(path);

            int i = 0;
            while (!stream_r.EndOfStream)
            {
                phrases.Add(stream_r.ReadLine());
                i++;
            }
            stream_r.Close();

            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);
                StringBuilder answer = null;

                Console.WriteLine("Client is running...");

                while (true)
                {
                    if (socket.Poll(1000, SelectMode.SelectRead))
                    {
                        break;
                    }

                    string message = phrases[rand.Next(phrases.Count)];
                    Thread.Sleep(1500);
                    Console.Write("\nClient's message to server: ");
                    Thread.Sleep(1500);
                    Console.WriteLine(message);
                    socket.Send(Encoding.UTF8.GetBytes(message));

                    if (message.ToString().ToLower().Contains("bye"))
                        break;

                    byte[] buffer = new byte[256];
                    answer = new StringBuilder();
                    var size_part_msg = 0;

                    do
                    {
                        size_part_msg = socket.Receive(buffer);
                        answer.Append(Encoding.UTF8.GetString(buffer, 0, size_part_msg));
                    } while (socket.Available > 0);

                    Thread.Sleep(1500);
                    Console.Write("\n" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "  ");
                    Console.Write("Server answer: ");
                    Thread.Sleep(1500);
                    Console.WriteLine(answer.ToString()); Console.WriteLine();

                    if (answer.ToString().ToLower().Contains("bye"))
                        break;
                }

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                throw (new Exception());
            }

            catch (Exception ex)
            {
                Console.WriteLine("\nServer dropped the connection."); //Console.WriteLine(ex.Message);
            }

            Console.ReadKey();

        }
    }
}
