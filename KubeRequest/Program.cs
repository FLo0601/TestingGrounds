using System.Net;
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
            kbs.listPodIps().ForEach(x => Console.WriteLine(x));
            var th = new Thread(kbs.receiveMessage);
            th.Start();
            while (true)
            {
                kbs.sendMessage();
                Thread.Sleep(10000);
            }
        }
    }

    public class KubeService
    {
        private KubernetesClientConfiguration config;
        private Kubernetes client;
        private HttpClient httpClient;

        public KubeService()
        {
            config = KubernetesClientConfiguration.InClusterConfig();
            client = new Kubernetes(config);
            httpClient = new HttpClient();
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

        public void sendMessage()
        {
            List<string> list = listPodIps();
            foreach (var ips in list)
            {
                if (ips.Equals(Environment.GetEnvironmentVariable("podIP"))) continue;
                httpClient.BaseAddress = new Uri($"http://{ips}:80/");
                HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, httpClient.BaseAddress);
                msg.Content = new StringContent("Ping!");
                Console.WriteLine("Request sending...");
                HttpResponseMessage response = httpClient.Send(msg);
                string responseMsg = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();
                Console.WriteLine($"Request response: {responseMsg}");
            }
                
        }

        public void receiveMessage()
        {
            HttpListener listenr = new HttpListener();
            listenr.Start();
            listenr.Prefixes.Add("http://*:80/");
            var reqContext = listenr.GetContext();
            string reqText = new StreamReader(reqContext.Request.InputStream).ReadToEnd();
            Console.WriteLine($"Request Comming in: {reqText}");
            byte[] responseBytes = Encoding.UTF8.GetBytes("Pong!");
            reqContext.Response.StatusCode = 200;
            reqContext.Response.KeepAlive = false;
            reqContext.Response.ContentLength64 = responseBytes.Length;
            reqContext.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            reqContext.Response.Close();
            Console.WriteLine("Response sending...");
        }
    }
}