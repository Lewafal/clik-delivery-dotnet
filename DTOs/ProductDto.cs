namespace ProductService.DTOs
{
    public class UpdateProductStockDto
    {
        public Guid ProductId { get; set; }
        public int QuantityToAdd { get; set; }
    }
}