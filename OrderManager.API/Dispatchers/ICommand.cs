namespace OrderManager.API.Dispatchers
{
    public interface ICommand { }
    public interface ICommand<out TResult> { }
}
