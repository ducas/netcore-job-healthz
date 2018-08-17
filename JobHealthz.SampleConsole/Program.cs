using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JobHealthz.SampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var r = new Random();
            
            // HealthzWithoutLogging() // Simple initializer for Healthz without service provider or logging
            HealthzWithLogging()       // Configure logging using ServiceProvider DI and resolve Healthz 
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

        private static Healthz HealthzWithLogging()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(l => l.AddConsole())    // Add console logging with default settings
                .AddSingleton<Healthz>()            // Add Healthz as a singleton
                .BuildServiceProvider();

            return serviceProvider.GetService<Healthz>(); // Resolve an instance of Healthz - this will include logging
        }

        private static Healthz HealthzWithoutLogging()
        {
            return new Healthz();
        }
    }
}