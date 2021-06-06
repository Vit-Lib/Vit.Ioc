using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls(new string[] { "http://*:5123" }) //(ori.1)
                .UseStartup<Startup>();
    }
}
