using Microsoft.EntityFrameworkCore;
using ShopOnline.Api.Data;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Repositories.Contracts;

namespace ShopOnline.Api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShopOnlineDbContext context;

        public ProductRepository(ShopOnlineDbContext context)
        {
            this.context=context;
        }
        public async Task<IEnumerable<ProductCategory>> GetCategories()
        {
            var categories = await this.context.ProductCategories.ToListAsync();
            return categories;
        }

        public async Task<ProductCategory> GetCategory(int id)
        {
            return await context.ProductCategories.SingleOrDefaultAsync(x=>x.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            var products = await context.Products.ToListAsync();
            return products;
        }

        public async Task<Product> GetProduct(int id)
        {
            return await context.Products.FindAsync(id);
        }
    }
}
