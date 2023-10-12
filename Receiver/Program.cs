using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Receiver
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            int port = 15000;
            using UdpClient udp = new UdpClient(port);
            Receive(udp);
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