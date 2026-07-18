using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;
using WebApp.Api.Data;
using WebApp.Api.DTOs;
using WebApp.Api.Models;
using WebApp.Api.Services.Interfaces;

namespace WebApp.Api.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrderDto> ChackoutAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if(cart == null || !cart.Items.Any())
            {
                throw new InvalidOperationException("Cart is empty.");
            }

            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Items = cart.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    UnitPrice = item.Product.Price,
                    Quantity = item.Quantity,
                }).ToList()
            };

            order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);
            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cart.Items);

            await _context.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userid)
        {
            var orders = await _context.Orders
                .Include(c => c.Items)
                .Where(i => i.UserId == userid)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        } 

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(int userId)
        {
            var orders = await _context.Orders
                .Include(c => c.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        } 

        public async Task<OrderDto?> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.Orders
                .Include(c => c.Items)
                .FirstOrDefaultAsync(i => i.Id == orderId)
            

            if(order == null)
            {
                return null;
            }

            order.Status = status;
            await _context.SaveChangesAsync();
            return _mapper.Map<OrderDto>(order);
        }
    }
}
