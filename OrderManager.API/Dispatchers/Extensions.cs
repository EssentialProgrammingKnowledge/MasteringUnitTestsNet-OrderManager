using OrderManager.API.Handlers.Orders;

namespace OrderManager.API.Dispatchers
{
    public static class Extensions
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
            return services.Scan(scan => scan
                           .FromAssemblyOf<AddOrder>()
                           .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
                           .AsImplementedInterfaces()
                           .WithScopedLifetime()
                           .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
                           .AsImplementedInterfaces()
                           .WithScopedLifetime());
        }
    }
}
