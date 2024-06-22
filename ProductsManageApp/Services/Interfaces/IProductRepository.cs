using ProductsManageApp.Models;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductsAsync(string searchString = null, int? page = null, int pageSize = 10);
    Task<int> GetTotalProductsAsync(string searchString = null);
    Task<Product> GetProductByIdAsync(int id);
    Task CreateProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
}
