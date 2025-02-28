using OrderManager.UI.Models;
using System.Net.Http.Json;

namespace OrderManager.UI.Services
{
    public class CustomerService
        (
            HttpClient httpClient
        ) : ICustomerService
    {
        public const string PATH = "/api/customers";

        public async Task<Result> Add(CustomerDTO customer)
        {
            var response = await httpClient.PostAsJsonAsync(PATH, customer);
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

        public async Task<Result<List<CustomerDTO>>> GetAll()
        {
            var response = await httpClient.GetAsync(PATH);
            if (!response.IsSuccessStatusCode)
            {
                return Result<List<CustomerDTO>>.Failed(await response.ToErrorMessage());
            }
            return Result<List<CustomerDTO>>.Success(
                (await response.Content.ReadFromJsonAsync<List<CustomerDTO>>()) ?? []
            );
        }

        public async Task<Result<CustomerDTO?>> GetById(int id)
        {
            var response = await httpClient.GetAsync($"{PATH}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return Result<CustomerDTO?>.Failed(await response.ToErrorMessage());
            }
            return Result<CustomerDTO?>.Success(await response.Content.ReadFromJsonAsync<CustomerDTO>());
        }

        public async Task<Result> Update(CustomerDTO customer)
        {
            var response = await httpClient.PutAsJsonAsync($"{PATH}/{customer.Id}", customer);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Failed(await response.ToErrorMessage());
            }
            return Result.Success();
        }
    }
}
