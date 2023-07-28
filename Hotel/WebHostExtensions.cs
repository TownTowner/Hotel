using Serilog.Sinks.Elasticsearch;
using Serilog;
using System.Reflection;
using System;
using Serilog.Exceptions;
using Microsoft.Extensions.Hosting;

namespace Hotel;

public static class WebHostExtensions
{
    public static IHostBuilder UseSerilogHotel(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, loggingConfiguration) =>
        {
            //// Get the environment which the application is running on
            //var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var env = context.HostingEnvironment.EnvironmentName;
            //// Get the configuration 
            //var configuration = new ConfigurationBuilder()
            //        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //        .Build();

            var esOptions = new ElasticsearchSinkOptions(new Uri(context.Configuration["ELKConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower()}-{env.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
                //,ModifyConnectionSettings = (conn) =>
                //{
                //    conn.ServerCertificateValidationCallback((source, cert, chain, error) => true);
                //    conn.BasicAuthentication("elastic", "elastic");
                //    return conn;
                //}
                //,EmitEventFailure = EmitEventFailureHandling.RaiseCallback
            };
            //Entich.FromLogContext 这里的作用将一些环境信息写入日志中 
            //new ElasticsearchSinkOptions设置了ES的地址； 
            //IndexFormat 默认是到天（hotel-[development/production]-{0:yyyy.MM.dd}），我修改成到月。不同IndexFormat的日志会存储在不同索引中；
            //EmitEventFailure 设置了当失败时调用FailureCallback
            //ModifyConnectionSettings这里需要设置了2个地方，一个是忽略证书, 一个是设置了 BasicAuthentioncation，也就是账号密码 。若没有设置密码，则无需要配置。

            // Create Logger
            //Log.Logger = new LoggerConfiguration()
            loggingConfiguration
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails() // Adds details exception
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(esOptions)
                //.CreateLogger()
                ;
        });
    }

}
