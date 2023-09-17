using Consul;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net;
using System;
using Consul.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hotel;

public static class WebServiceExtensions
{
    public static IServiceCollection AddHotelConsulServiceRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConsul(op => op.Address = new Uri(configuration["Consul:Uri"]));
        return services.AddSingleton(sp =>
        {
            var hostname = Dns.GetHostName();
            //var address = Dns.GetHostEntry(hostname).AddressList.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork)?.ToString();
            var uri = new Uri(configuration["Consul:ServiceUri"]);

            var serviceId = "hotel-web-service-v1-" + hostname + "-" + uri.Authority;
            var logger = sp.GetRequiredService<ILogger<Program>>();

            logger.LogError($"{uri.Scheme}://{uri.Authority}/health");

            var agentServiceRegistration = new AgentServiceRegistration()
            {
                ID = serviceId,
                Name = "hotel-web-service",
                Address = uri.Host,
                Port = uri.Port,
                Tags = new[] { "api", "hotel", "web" },
                Meta = new Dictionary<string, string>() { { "version", "1.0.0" }, { "tags", "api,hotel" } },
                Checks = new[] {
                    new AgentServiceCheck()
                    {
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(60), //心跳检测失败多久后注销
                        Interval = TimeSpan.FromSeconds(10), //间隔多久心跳检测一次
                        HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}/health", //心跳检查地址，本服务提供的地址
                        Timeout = TimeSpan.FromSeconds(5)  //心跳检测超时时间
                    }
                }
            };
            return agentServiceRegistration;
        }).AddHostedService<AgentServiceRegistrationHostedService>();
    }
}
