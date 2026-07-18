using WebApp.Api.DTOs;

namespace WebApp.Api.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);
        Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto);
        Task<CartDto?> UpdateCartItemAsync(int userId, int productId, UpdateCartItemDto dto);
        Task<bool> RemoveFromCartAsync(int userId, int productId);
    }
}
