using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using k8s;

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
            while (i < 15)
            {
                // Setup Part
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add("http://localhost:5678/container/");
                listener.Prefixes.Add("http://localhost:5678/containers/");
                listener.Start();
                HttpListenerContext context = listener.GetContext();

                // Request Part
                HttpListenerRequest request = context.Request;
                Console.WriteLine(request.Url);
                using (StreamReader reader = new StreamReader(request.InputStream))
                {
                    string reqMsg = reader.ReadToEnd();
                    Console.WriteLine(reqMsg);
                }


                // Response Part
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

    public class Server
    {
        private HttpListener _listener;

        public Server(HttpListener listener)
        {
            _listener = listener;
        }

        public void StartListening()
        {
            _listener.Prefixes.Add("http://localhost:5678/");
            _listener.Start();
            HttpListenerContext context = _listener.GetContext();
            HttpListenerRequest request = context.Request;
            string? path = request.Url?.LocalPath;
            string? jsonData = this.ExtractData(request);
            this.HandlePath(path);
        }

        private string? ExtractData(HttpListenerRequest request)
        {
            if (request.HttpMethod == "GET" || request.HttpMethod == "DELETE") return null;

            using StreamReader reader = new StreamReader(request.InputStream);
            return reader.ReadToEnd();
        }

        private string HandlePath(string? path)
        {
            switch (path)
            {
               case "containers":
                   return "handle kubeservice and html here";
               case "container/{key}":
                   return "handle kubeservice and more html here";
               default:
                   return "error";
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

        public List<string> ListPodIps()
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
            foreach (var podIp in ListPodIps())
            {

                IPEndPoint groupIp = new IPEndPoint(IPAddress.Parse(podIp), this.port);

                string message = $"Send from {ipstr}";
                byte[] sendBytes = Encoding.ASCII.GetBytes(message);

                udpClient.Send(sendBytes, sendBytes.Length, groupIp);

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

    public class HttpEnpointService
    {
        private HttpListener _listener;

        public HttpEnpointService()
        {
            _listener = new HttpListener();
        }

        public async void StartListeningOn(string[] urlEndpoints)
        {
            foreach (var urlEnpoint in urlEndpoints)
            {
                _listener.Prefixes.Add($"http://localhost:5678/{urlEnpoint}/");
            }
            _listener.Start();
            HttpListenerContext context = await _listener.GetContextAsync();
        }

        public void StopListening()
        {
            if (_listener.IsListening)
            {
                _listener.Stop();
            }
            else
            {
                Console.WriteLine("Client already stopped listening");
            }
        }

        private async void processGetRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;

            using (StreamReader reader = new StreamReader(request.InputStream))
            {
                string requestMessage = reader.ReadToEnd();
                Console.WriteLine(requestMessage);
            }

            switch (request.Url.AbsoluteUri)
            {
                case "http://localhost:5678/container/":
                    this.GetContainerName(request, context);
                    break;
                case "http://localhost:5678/containers/":
                    this.ListContainerData();
                    break;
                default:
                    Console.WriteLine("Unknown URL");
                    break;
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
        }

        private async void GetContainerName(HttpListenerRequest request, HttpListenerContext context)
        {
        }

        private void ListContainerData()
        {

        }
    }

    public class ContainerData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IpAddr { get; set; }
    }
}