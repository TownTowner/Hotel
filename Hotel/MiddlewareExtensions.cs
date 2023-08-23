using Microsoft.AspNetCore.Builder;
using Prometheus;

namespace Hotel;

public static class MiddlewareExtensions
{
    //https://medium.com/@sddkal/creating-a-simple-dockerized-net-core-api-with-prometheus-grafana-monitoring-275da0878412
    public static void UseMetricPathCounter(this IApplicationBuilder app)
    {
        app.Use((context, next) =>
        {
            // Http Context
            var counter = Metrics.CreateCounter
            ("PathCounter", "Count request",
            new CounterConfiguration
            {
                LabelNames = new[] { "method", "endpoint" }
            });
            // method: GET, POST etc.
            // endpoint: Requested path
            counter.WithLabels(context.Request.Method, context.Request.Path).Inc();
            return next();
        });
    }
}
