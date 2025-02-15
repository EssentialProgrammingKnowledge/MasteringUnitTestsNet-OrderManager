using OrderManager.UI.Models;
using System.Net.Http.Json;

namespace OrderManager.UI.Services
{
    public class ProductService
        (
            HttpClient httpClient
        )
        : IProductService
    {
        public const string PATH = "/api/products";

        public async Task<Result> Add(ProductDTO product)
        {
            var response = await httpClient.PostAsJsonAsync(PATH, product);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Failed(await response.ToErrorMessage());
            }
            return Result.Success();
        }

        public async Task<Result> Delete(int id)
        {
            var response = await httpClient.DeleteAsync($"{PATH}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return Result.Failed(await response.ToErrorMessage());
            }
            return Result.Success();
        }

        public async Task<Result<List<ProductDTO>>> GetAll()
        {
            var response = await httpClient.GetAsync(PATH);
            if (!response.IsSuccessStatusCode)
            {
                return Result<List<ProductDTO>>.Failed(await response.ToErrorMessage());
            }
            return Result<List<ProductDTO>>.Success(
                (await response.Content.ReadFromJsonAsync<List<ProductDTO>>()) ?? []
            );
        }

        public async Task<Result<ProductDTO?>> GetById(int id)
        {
            var response = await httpClient.GetAsync($"{PATH}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return Result<ProductDTO?>.Failed(await response.ToErrorMessage());
            }
            return Result<ProductDTO?>.Success(await response.Content.ReadFromJsonAsync<ProductDTO>());
        }

        public async Task<Result> Update(ProductDTO product)
        {
            var response = await httpClient.PutAsJsonAsync($"{PATH}/{product.Id}", product);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Failed(await response.ToErrorMessage());
            }
            return Result.Success();
        }
    }
}
