using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace rpi_stat_ui
{
    public static class Program
    {
        public static void Main(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .Build()
                .Run();
    }
}
