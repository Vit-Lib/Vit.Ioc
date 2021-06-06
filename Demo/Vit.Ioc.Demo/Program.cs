using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Vit.Extensions;//(demo.1)

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
                .Populate()//(demo.2)
                .UseStartup<Startup>();
    }
}
