using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JobHealthz.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var r = new Random();
            
            var serviceProvider = new ServiceCollection()
                .AddLogging(l => l.AddConsole())
                .AddSingleton<Healthz>()
                .BuildServiceProvider();

            serviceProvider.GetService<Healthz>()
                .Start(() => new HealthzResult
                {
                    Status = r.Next(0, 10) >= 5 ? HealthzStatus.Ok : HealthzStatus.Fail,
                    Message = r.Next(1, 10).ToString()
                });
            
            Console.WriteLine("Press Enter to exit.");
            try
            {
                Console.ReadLine();
            }
            catch (Exception) { }
        }
    }
}