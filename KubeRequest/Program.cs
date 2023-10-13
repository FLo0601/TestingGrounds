using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using k8s;
using Microsoft.IdentityModel.Tokens;

namespace KubeRequest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = KubernetesClientConfiguration.InClusterConfig();
            var client = new Kubernetes(config);
            KubeService kbs = new KubeService();
            var th = new Thread(kbs.Receive);
            th.Start();
            while (true)
            {
                kbs.Send();
                Thread.Sleep(10000);
            }
        }
    }

    public class KubeService
    {
        private KubernetesClientConfiguration config;
        private Kubernetes client;
        private UdpClient udpClient;
        private int port = 15000;
        private string ipstr;

        public KubeService()
        {
            config = KubernetesClientConfiguration.InClusterConfig();
            client = new Kubernetes(config);
            udpClient = new UdpClient(port);
            ipstr = Environment.GetEnvironmentVariable("podIP");
        }

        public List<string> listPodIps()
        {
            List<string> ips = new List<string>();
            foreach (var pod in this.client.CoreV1.ListNamespacedPod("default").Items)
            {
                ips.Add(pod.Status.PodIP);
            }
            return ips;
        }

        public void Send()
        {
            foreach (var podIp in listPodIps())
            {

                IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse(podIp), this.port);

                string message = $"Send from {ipstr}";
                byte[] sendBytes = Encoding.ASCII.GetBytes(message);

                udpClient.Send(sendBytes, sendBytes.Length, groupEP);

                Console.WriteLine("Message send!");
            }
            Thread.Sleep(10000);
        }

        public void Receive()
        {
            while (true)
            {
                IPEndPoint server = new IPEndPoint(IPAddress.Any, 0);

                byte[] packet = ((UdpClient)udpClient).Receive(ref server);
                Console.WriteLine(Encoding.ASCII.GetString(packet));

                Console.WriteLine("MessageReceived");
            }
        }
    }
}