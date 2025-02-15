using Microsoft.EntityFrameworkCore;

namespace OrderManager.API.Database
{
    public static class Extensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddDbContext<OrderContext>(options =>
                options.UseInMemoryDatabase("OrderDb"));
            services.AddTransient<ISeedDataProvider, SeedDataProvider>();
            services.AddHostedService<DbInitializer>();
            return services;
        }
    }
}
