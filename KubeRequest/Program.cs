using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using k8s;
using Microsoft.IdentityModel.Tokens;

namespace KubeRequest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                KubeService kbs = new KubeService();
                var th = new Thread(kbs.Receive);
                th.Start();
                while (true)
                {
                    kbs.Send();
                    Thread.Sleep(10000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Not in a kubernetes");
            }

            int i = 0;
            while (i < 5)
            {
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add("http://localhost:5678/container/");
                listener.Start();
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                using (StreamReader reader = new StreamReader(request.InputStream))
                {
                    string reqMsg = reader.ReadToEnd();
                    Console.WriteLine(reqMsg);
                }

                HttpListenerResponse response = context.Response;

                response.AddHeader("Access-Control-Allow-Headers", "*");
                response.AddHeader("Access-Control-Allow-Methods", "*");
                response.AddHeader("Access-Control-Allow-Origin", "*");

                string jsonString = JsonSerializer.Serialize(new ContainerData
                {
                    Id = 0,
                    Name = "Container 0",
                    IpAddr = "192.128.0.1"
                });

                byte[] buffer = Encoding.UTF8.GetBytes(jsonString);
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
                listener.Stop();
                i++;
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

    public class ContainerData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IpAddr { get; set; }
    }
}