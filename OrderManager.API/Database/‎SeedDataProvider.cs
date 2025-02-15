using Microsoft.EntityFrameworkCore;
using OrderManager.API.Models;

namespace OrderManager.API.Database
{
    public class SeedDataProvider
        (
            OrderContext orderContext
        ) : ISeedDataProvider
    {
        public async Task SeedData(CancellationToken cancellationToken = default)
        {
            await SeedCustomers(cancellationToken);
            await SeedProducts(cancellationToken);
        }

        private async Task SeedCustomers(CancellationToken cancellationToken = default)
        {
            if (await orderContext.Customers.AnyAsync(cancellationToken))
            {
                return;
            }

            var customers = new List<Customer>
            {
                new() { Email = "jan.kowalski@email.com", FirstName = "Jan", LastName = "Kowalski" },
                new() { Email = "piotr.stefanski@email.com", FirstName = "Piotr", LastName = "Stefański" },
                new() { Email = "andrzej.twardy@email.com", FirstName = "Andrzej", LastName = "Twardy" }
            };

            await orderContext.Customers.AddRangeAsync(customers, cancellationToken);
            await orderContext.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedProducts(CancellationToken cancellationToken = default)
        {
            if (await orderContext.Products.AnyAsync(cancellationToken))
            {
                return;
            }

            var products = new List<Product>
            {
                new() { ProductName = "Kurs programowania", IsDigital = true, Price = 200 },
                new() { ProductName = "Laptop HP", IsDigital = false, Price = 8000, ProductStock = new() { Quantity = 100 } },
                new() { ProductName = "Smartphone Samsung", IsDigital = false, Price = 4000, ProductStock = new() { Quantity = 50 } }
            };

            await orderContext.Products.AddRangeAsync(products, cancellationToken);
            await orderContext.SaveChangesAsync(cancellationToken);
        }
    }
}
