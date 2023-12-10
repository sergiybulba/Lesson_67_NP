using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Task_1_Server
{
    class Program
    {
        const string ip = "127.0.0.1";
        const int port = 8080;
        static void Main(string[] args)
        {
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

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            server.Bind(endPoint);
            server.Listen(10);
            Console.WriteLine("Server is running...");

            while (true)
            {
                try
                {
                    Socket client_listener = server.Accept();
                    StringBuilder message = null;
                    while (true)
                    {
                        if (client_listener.Poll(1000, SelectMode.SelectRead))
                        {
                            Console.WriteLine("\nClient dropped the connection.");
                            break;
                        }

                        byte[] buffer = new byte[256];
                        message = new StringBuilder();
                        var size_part_msg = 0;

                        do
                        {
                            size_part_msg = client_listener.Receive(buffer);
                            message.Append(Encoding.UTF8.GetString(buffer, 0, size_part_msg));
                        } while (client_listener.Available > 0);

                        Thread.Sleep(1500);
                        Console.Write("\n" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "  Client #" + ((IPEndPoint)client_listener.RemoteEndPoint).Port + ": ");
                        Thread.Sleep(1500);
                        Console.WriteLine(message);

                        if (message.ToString().ToLower().Contains("bye"))
                        {
                            client_listener.Shutdown(SocketShutdown.Both);
                            client_listener.Close();
                            break;
                        }
                        //client_listener.Send(Encoding.UTF8.GetBytes("Message received."));

                        string answer = phrases[rand.Next(phrases.Count)];
                        Thread.Sleep(1500);
                        Console.Write("\nServer's answer: ");
                        Thread.Sleep(1500);
                        Console.WriteLine(answer);
                        client_listener.Send(Encoding.UTF8.GetBytes(answer));

                        if (answer.ToString().ToLower().Contains("bye"))
                        {
                            client_listener.Shutdown(SocketShutdown.Both);
                            client_listener.Close();
                            break;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            //Console.ReadLine();
        }
    }
}
