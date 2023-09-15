using Consul;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;

namespace Hotel.Configuration;

public class HotelAgentServiceRegistrationHostedService : IHostedService
{
    private readonly IConsulClient _consulClient;

    private readonly IServer _server;

    private readonly AgentServiceRegistration _serviceRegistration;

    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger _logger;

    public HotelAgentServiceRegistrationHostedService(IConsulClient consulClient, AgentServiceRegistration serviceRegistration,
        IServer server, IHostApplicationLifetime lifetime, ILogger logger)
    {
        _consulClient = consulClient;
        _serviceRegistration = serviceRegistration;
        _server = server;
        _lifetime = lifetime;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _lifetime.ApplicationStarted.Register(async () =>
        {
            // the address should called after application started
            // !! It will be get the container IP when you set your app in a container  !!
            var features = _server.Features.Get<IServerAddressesFeature>();
            var address = features.Addresses.FirstOrDefault();
            var uri = new Uri(address);

            _logger.LogDebug("Detected IServerAddressesFeature address:" + address);

            if (string.IsNullOrEmpty(_serviceRegistration.Address))
            {
                if ((_serviceRegistration.ID + "").EndsWith(uri.Authority) == false)
                    _serviceRegistration.ID += "-" + uri.Authority;
                _serviceRegistration.Address = uri.Host;
                _serviceRegistration.Port = uri.Port;

                if ((_serviceRegistration.Checks?.Any() ?? false) == false)
                {
                    //心跳检测设置
                    var httpCheck = new AgentServiceCheck()
                    {
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(60), //心跳检测失败多久后注销
                        Interval = TimeSpan.FromSeconds(10), //间隔多久心跳检测一次
                        HTTP = $"{uri.Scheme}://{uri.Authority}/health", //心跳检查地址，本服务提供的地址
                        Timeout = TimeSpan.FromSeconds(5)  //心跳检测超时时间
                    };
                    _serviceRegistration.Checks = new[] { httpCheck };
                }
            }

            await _consulClient.Agent.ServiceRegister(_serviceRegistration, cancellationToken);
        });
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _consulClient.Agent.ServiceDeregister(_serviceRegistration.ID, cancellationToken);
    }
}