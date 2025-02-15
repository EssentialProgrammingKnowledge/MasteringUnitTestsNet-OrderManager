using System.Reflection;

namespace OrderManager.API.Dispatchers
{
    internal sealed class CommandDispatcher(IServiceProvider serviceProvider)
        : ICommandDispatcher
    {
        public async Task Send<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            using var scope = serviceProvider.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetService<ICommandHandler<TCommand>>();
            if (handler != null)
            {
                await handler.Handle(command);
                return;
            }

            throw new InvalidOperationException($"No handler found for command {typeof(TCommand).Name}");
        }

        public async Task<TResult> Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        {
            using var scope = serviceProvider.CreateAsyncScope();
            var commandType = command.GetType();
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResult));
            var handler = GetHandler(handlerType, scope);
            var method = GetHandleMethod(handlerType);

            if (handler != null && method != null)
            {
                return await (Task<TResult>)method.Invoke(handler, [command, cancellationToken])!;
            }

            throw new InvalidOperationException($"No handler found for command {commandType.Name}");
        }

        private object? GetHandler(Type handlerType, IServiceScope scope)
        {
            return scope.ServiceProvider.GetService(handlerType);
        }

        private MethodInfo? GetHandleMethod(Type handlerType)
        {
            return handlerType.GetMethod("Handle");
        }
    }
}
