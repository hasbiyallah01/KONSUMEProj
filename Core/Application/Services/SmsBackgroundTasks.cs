

namespace DaticianProj.Core.Application.Services
{
    public class SmsBackgroundTasks : BackgroundService
    {
        public SmsBackgroundTasks()
        {
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("ASSSSSSSDFGHJMMMMMMMM");
            return Task.CompletedTask;
        }
    }
}

