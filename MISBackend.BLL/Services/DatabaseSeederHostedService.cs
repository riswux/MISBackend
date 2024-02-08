using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MISBackend.DAL;
using MISBackend.DAL.Migrations;

namespace MISBackend.BLL.Services
{
    public class DatabaseSeederHostedService : IHostedService
    {
        private readonly IServiceProvider _services;

        public DatabaseSeederHostedService(IServiceProvider services)
        {
            _services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<MISDbContextSeed>();
                await seeder.SeedAsync(); // Tambahkan await di sini
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
