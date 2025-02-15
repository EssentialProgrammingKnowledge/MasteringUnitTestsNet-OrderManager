using OrderManager.UI.Models;
using System.Net.Http.Json;

namespace OrderManager.UI.Services
{
    public class OrderService
        (
            HttpClient httpClient
        )
        : IOrderService
    {
        private const string PATH = "/api/orders";

        public async Task<Result<OrderDetailsDTO?>> Add(AddOrderDTO dto)
        {
            var response = await httpClient.PostAsJsonAsync(PATH, dto);
            if (!response.IsSuccessStatusCode)
            {
                return Result<OrderDetailsDTO?>.Failed(await response.ToErrorMessage());
            }

            return Result<OrderDetailsDTO?>.Success(await response.Content.ReadFromJsonAsync<OrderDetailsDTO>());
        }

        public async Task<Result> ChangeStatus(int id, OrderStatus orderStatus)
        {
            var response = await httpClient.PatchAsJsonAsync($"{PATH}/{id}/status", new
            {
                Id = id,
                OrderStatus = orderStatus
            });
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

        public async Task<Result<List<OrderDTO>>> GetAll()
        {
            var response = await httpClient.GetAsync(PATH);
            if (!response.IsSuccessStatusCode)
            {
                return Result<List<OrderDTO>>.Failed(await response.ToErrorMessage());
            }

            return Result<List<OrderDTO>>.Success(
                (await response.Content.ReadFromJsonAsync<List<OrderDTO>>()) ?? []
            );
        }

        public async Task<Result<OrderDetailsDTO?>> GetById(int id)
        {
            var response = await httpClient.GetAsync($"{PATH}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return Result<OrderDetailsDTO?>.Failed(await response.ToErrorMessage());
            }

            return Result<OrderDetailsDTO?>.Success(await response.Content.ReadFromJsonAsync<OrderDetailsDTO>());
        }

        public async Task<Result<OrderDetailsDTO>> Update(UpdateOrderDTO dto)
        {
            var response = await httpClient.PutAsJsonAsync($"{PATH}/{dto.Id}", dto);
            if (!response.IsSuccessStatusCode)
            {
                return Result<OrderDetailsDTO>.Failed(await response.ToErrorMessage());
            }

            return Result<OrderDetailsDTO>.Success(await response.Content.ReadFromJsonAsync<OrderDetailsDTO>()!);
        }

        public async Task<Result<OrderDetailsDTO>> UpdatePositions(int orderId, IEnumerable<OrderItemDTO> dtos)
        {
            var response = await httpClient.PatchAsJsonAsync($"{PATH}/{orderId}/positions", dtos);
            if (!response.IsSuccessStatusCode)
            {
                return Result<OrderDetailsDTO>.Failed(await response.ToErrorMessage());
            }

            return Result<OrderDetailsDTO>.Success(await response.Content.ReadFromJsonAsync<OrderDetailsDTO>()!);
        }
    }
}
