using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server;


await Host.CreateDefaultBuilder(args)
          .ConfigureServices((hostContext, services) =>
          {
              services.AddHostedService<SendMessageBackgroundService>();
          }).RunConsoleAsync();

await Worker.HandleRequests(Worker.InitServer());