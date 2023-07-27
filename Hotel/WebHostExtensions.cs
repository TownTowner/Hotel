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
            };
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
