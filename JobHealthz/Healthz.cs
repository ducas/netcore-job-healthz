using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace JobHealthz
{
    public class Healthz
    {
        private readonly ILogger<Healthz> _logger;

        public Healthz(ILogger<Healthz> logger)
        {
            _logger = logger;
        }
        public Healthz()
        {
            _logger = new NullLogger<Healthz>();
        }

        public void Start(IHealthCheck check)
        {
            Start(check.Check);
        }
        
        public void Start(Func<HealthzResult> check)
        {
            var host =  new WebHostBuilder()
                .UseKestrel()
                .Configure(app => app.Map("/healthz",
                    a =>
                    {
                        a.Run(async context =>
                        {
                            _logger.LogTrace("Begin healthz check");
                            var result = check();
                            _logger.LogInformation("Healthz: {status} - {message}", result.Status, result.Message);
                            context.Response.StatusCode = (int) result.Status;
                            await context.Response.WriteAsync(result.Message);
                            _logger.LogTrace("End healthz check");
                        });
                    })
                )
                .Build();
            host.Start();
            var urls = string.Join(",", host.ServerFeatures.Get<IServerAddressesFeature>().Addresses);
            Console.WriteLine($"Health checks listening on {urls} at /healthz.");
        }
    }

    public interface IHealthCheck
    {
        HealthzResult Check();
    }

    public class HealthzResult
    {
        public HealthzStatus Status { get; set; } = HealthzStatus.Ok;
        public string Message { get; set; } = string.Empty;
    }

    public enum HealthzStatus
    {
        Ok = 200,
        Fail = 500
    }
}
