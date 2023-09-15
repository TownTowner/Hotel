using Consul;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net;
using Hotel.Configuration;
using System;
using Consul.AspNetCore;
using Microsoft.Extensions.Configuration;

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
            var agentServiceRegistration = new AgentServiceRegistration()
            {
                ID = serviceId,
                Name = "hotel-web-service",
                Address = uri.Host,
                Port = uri.Port,
                Tags = new[] { "api", "hotel", "web" },
                Meta = new Dictionary<string, string>() { { "version", "1.0.0" }, { "tags", "api,hotel" } }
            };
            return agentServiceRegistration;
        }).AddHostedService<HotelAgentServiceRegistrationHostedService>();
    }
}
