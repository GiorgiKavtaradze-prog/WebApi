using WebApp.Api.DTOs;
using WebApp.Api.Models;

namespace WebApp.Api.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CheckoutAsync(int userId);
        Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto> UpdateOrderStatusAsync(int orderId, OrderStatus status);
    }
}
