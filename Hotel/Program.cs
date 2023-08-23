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
                //ÿ5���¼����ɼ�һ��
                .WithContentionStats(sampleRate: SampleEvery.FiveEvents)
                //ÿ10�¼��ɼ�һ��
                .WithJitStats(sampleRate: SampleEvery.TenEvents)
                ////ÿ100�¼��ɼ�һ��
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
