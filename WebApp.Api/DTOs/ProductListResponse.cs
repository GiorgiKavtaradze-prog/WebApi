namespace WebApp.Api.DTOs
{
    public class ProductListResponse
    {
        public IEnumerable<ProductDto> Data { get; set; } = new List<ProductDto>();

        public int TotalCount { get; set; };
    }
}
