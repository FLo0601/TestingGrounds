using k8s;

namespace KubeRequest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = KubernetesClientConfiguration.InClusterConfig();
            var client = new Kubernetes(config);

            var namespaces = client.CoreV1.ListNamespace();
            foreach (var ns in namespaces.Items) {
                Console.WriteLine(ns.Metadata.Name);
                var list = client.CoreV1.ListNamespacedPod(ns.Metadata.Name);
                foreach (var item in list.Items)
                {
                    Console.WriteLine(item.Metadata.Name);
                }
            }
        }
    }
}