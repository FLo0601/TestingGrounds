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

            var pods = client.CoreV1.ListNamespacedPod("default");
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

        private List<string> listPodIps()
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
            var httpClient = new HttpClient();
            List<string> list = listPodIps();
            foreach (var ips in list)
            {
                httpClient.BaseAddress = new Uri(ips);
                HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, httpClient.BaseAddress);
                msg.Content = new StringContent("Pong!");
                httpClient.Send(msg);
            }
                
        }

        public void receiveMessage()
        {
            httpClient.BaseAddress = new Uri("test");
            HttpListener listenr = new HttpListener();
            listenr.Start();
            var req = listenr.GetContext().Request;
            string text;
            StreamReader reader = new StreamReader();
            req.InputStream.BeginRead();
        }
    }
}