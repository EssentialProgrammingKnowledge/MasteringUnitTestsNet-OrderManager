using OrderManager.UI.Models;

namespace OrderManager.UI.Services
{
    public interface ICartService
    {
        event Action<int> OnCartItemsChanged;

        Task<List<CartItem>> GetCartItems();
        Task AddProductToCart(CartItem cartItem);
        Task RemoveFromCart(int productId);
        Task<int> GetTotalItems();
        Task ClearCart();
        Task IncreaseQuantity(int productId);
        Task DecreaseQuantity(int productId);
    }
}
