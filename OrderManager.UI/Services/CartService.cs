using Blazored.LocalStorage;
using OrderManager.UI.Models;
using System.Text.Json;

namespace OrderManager.UI.Services
{
    public class CartService(IServiceProvider serviceProvider) : ICartService
    {
        private const string CartKey = "cartItems";
        
        public event Action<int> OnCartItemsChanged = null!;

        public async Task<List<CartItem>> GetCartItems()
        {
            using var scope = serviceProvider.CreateAsyncScope();
            var localStorageService = scope.ServiceProvider.GetRequiredService<ILocalStorageService>();
            return await GetCartItemsInternal(localStorageService);
        }

        public async Task AddProductToCart(CartItem cartItem)
        {
            using var scope = serviceProvider.CreateAsyncScope();
            var localStorageService = scope.ServiceProvider.GetRequiredService<ILocalStorageService>();
            var cartItems = await GetCartItemsInternal(localStorageService);
            var existingCartItem = cartItems.FirstOrDefault(c => c.Product.Id == cartItem.Product.Id);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += cartItem.Quantity;
            }
            else
            {
                cartItems.Add(cartItem);
            }

            await UpdateCartItems(localStorageService, cartItems);
        }

        public async Task RemoveFromCart(int productId)
        {
            using var scope = serviceProvider.CreateAsyncScope();
            var localStorageService = scope.ServiceProvider.GetRequiredService<ILocalStorageService>();
            var cartItems = await GetCartItemsInternal(localStorageService);
            var itemToRemove = cartItems.FirstOrDefault(product => product.Product.Id == productId);

            if (itemToRemove == null)
            {
                return;
            }

            cartItems.Remove(itemToRemove);
            await UpdateCartItems(localStorageService, cartItems);
        }

        public async Task ClearCart()
        {
            using var scope = serviceProvider.CreateAsyncScope();
            var localStorageService = scope.ServiceProvider.GetRequiredService<ILocalStorageService>();
            await localStorageService.RemoveItemAsync(CartKey);
            OnCartItemsChanged?.Invoke(0);
        }

        public async Task<int> GetTotalItems()
        {
            var cartItems = await GetCartItems();
            return GetTotalQuantity(cartItems);
        }
        public async Task IncreaseQuantity(int productId)
        {
            using var scope = serviceProvider.CreateAsyncScope();
            var localStorageService = scope.ServiceProvider.GetRequiredService<ILocalStorageService>();
            var cartItems = await GetCartItemsInternal(localStorageService);
            var cartItem = cartItems.FirstOrDefault(c => c.Product?.Id == productId);
            if (cartItem == null)
            {
                return;
            }

            cartItem.Quantity++;
            await UpdateCartItems(localStorageService, cartItems);

        }

        public async Task DecreaseQuantity(int productId)
        {
            using var scope = serviceProvider.CreateAsyncScope();
            var localStorageService = scope.ServiceProvider.GetRequiredService<ILocalStorageService>();
            var cartItems = await GetCartItemsInternal(localStorageService);
            var cartItem = cartItems.FirstOrDefault(c => c.Product?.Id == productId);
            if (cartItem == null)
            {
                return;
            }

            cartItem.Quantity--;
            await UpdateCartItems(localStorageService, cartItems);
        }

        private async Task<List<CartItem>> GetCartItemsInternal(ILocalStorageService localStorageService)
        {
            var cartItems = await localStorageService.GetItemAsync<string>(CartKey);
            return string.IsNullOrEmpty(cartItems)
                ? []
                : JsonSerializer.Deserialize<List<CartItem>>(cartItems)
                ?? [];
        }

        private async Task UpdateCartItems(ILocalStorageService localStorageService, List<CartItem> cartItems)
        {
            await localStorageService.SetItemAsync(CartKey, cartItems);
            OnCartItemsChanged?.Invoke(GetTotalQuantity(cartItems));
        }

        private int GetTotalQuantity(IEnumerable<CartItem> cartItems) => cartItems.Sum(item => item.Quantity);
    }
}
