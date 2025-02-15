namespace OrderManager.API.Database
{
    internal sealed class DbInitializer
        (
            IServiceProvider serviceProvider
        )
        : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateAsyncScope();
            var orderContext = scope.ServiceProvider.GetRequiredService<OrderContext>();
            await orderContext.Database.EnsureCreatedAsync(cancellationToken);
            var seedDataProvider = scope.ServiceProvider.GetRequiredService<ISeedDataProvider>();
            await seedDataProvider.SeedData(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
