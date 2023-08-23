using Hotel.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus.DotNetRuntime;

namespace Hotel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetRuntimeStatsBuilder
                .Customize()
                //每5个事件个采集一个
                .WithContentionStats(sampleRate: SampleEvery.FiveEvents)
                //每10事件采集一个
                .WithJitStats(sampleRate: SampleEvery.TenEvents)
                ////每100事件采集一个
                //.WithThreadPoolSchedulingStats(sampleRate: SampleEvery.HundredEvents)
                .WithThreadPoolStats()
                .WithGcStats()
                .StartCollecting();
            //CreateHostBuilder(args).Build().Run();
            IHost webHost = CreateHostBuilder(args).UseSerilogHotel().Build();

            using var scope = webHost.Services.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<UserService>();
            userService.DbSeed();

            webHost.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
