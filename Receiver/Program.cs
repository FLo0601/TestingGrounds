namespace Receiver
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient heySend = new HttpClient();

            while (true)
            {
                Console.Write("URI: ");
                string uri = Console.ReadLine();
                try
                {
                    HttpRequestMessage heyMessage = new HttpRequestMessage(HttpMethod.Post, uri);
                    Console.Write("Message: ");
                    string heyLine = Console.ReadLine();
                    heyMessage.Content = new StringContent(heyLine);
                    await heySend.SendAsync(heyMessage);
                    Console.WriteLine("Message send");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("URI could not be reached");
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}