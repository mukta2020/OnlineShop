using ShopOnline.Api.Entities;

namespace ShopOnline.Api.Repositories.Contracts
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<IEnumerable<ProductCategory>> GetCategories();
        Task<Product> GetProduct(int id);
        Task<ProductCategory> GetCategory(int id);
    }
}
