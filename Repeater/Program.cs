using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Repeater
{
        
        class Program
        {
            public static void Main(string[] args)
            {
                int port = 15000;
                Console.WriteLine($"{Environment.GetEnvironmentVariable("my-ip")}");
                using UdpClient udp = new UdpClient(port);
                var th = new Thread(Receive);
                th.Start(udp);
                while (true)
                {
                    Send(udp, port);
                    Console.WriteLine("Beep");
                }
            }

            private static void Send(UdpClient udp, int port)
            {
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Broadcast, port);

                string message = "I want to receive this!";
                byte[] sendBytes = Encoding.ASCII.GetBytes(message);

                udp.Send(sendBytes, sendBytes.Length, groupEP);

                Console.WriteLine("Message send!");
                Thread.Sleep(10000);
            }

            private static void Receive(Object udp)
            {
                while (true)
                {
                    IPEndPoint server = new IPEndPoint(IPAddress.Any, 0);

                    byte[] packet = ((UdpClient) udp).Receive(ref server);
                    Console.WriteLine(Encoding.ASCII.GetString(packet));

                    Console.WriteLine("MessageReceived");
                }
            }
        }
}