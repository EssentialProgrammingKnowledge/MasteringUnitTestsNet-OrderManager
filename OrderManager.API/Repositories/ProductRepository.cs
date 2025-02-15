using Microsoft.EntityFrameworkCore;
using OrderManager.API.Database;
using OrderManager.API.Models;

namespace OrderManager.API.Repositories
{
    internal sealed class ProductRepository
        (
            OrderContext orderContext
        )
        : IProductRepository
    {
        public async Task<Product> Add(Product product)
        {
            await orderContext.Products.AddAsync(product);
            await orderContext.SaveChangesAsync();
            return product;
        }

        public async Task<bool> Delete(Product product)
        {
            orderContext.Products.Remove(product);
            return await orderContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await orderContext.Products.ToListAsync();
        }

        public async Task<Product?> GetById(int id)
        {
            return await orderContext.Products
                                     .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProductsByIds(List<int> productIds)
        {
            return await orderContext.Products
                                     .Where(p => productIds.Contains(p.Id))
                                     .ToListAsync();
        }

        public async Task<bool> ProductHasBeenOrdered(int productId)
        {
            return await orderContext.OrderItems
                                     .AnyAsync(p => p.ProductId == productId);
        }

        public async Task<Product> Update(Product product)
        {
            orderContext.Products.Update(product);
            await orderContext.SaveChangesAsync();
            return product;
        }

        public async Task UpdateRange(IEnumerable<Product> products)
        {
            orderContext.Products.UpdateRange(products);
            await orderContext.SaveChangesAsync();
        }
    }
}
