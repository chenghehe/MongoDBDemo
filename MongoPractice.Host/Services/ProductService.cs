using Microsoft.Extensions.Configuration;
using MongoPractice.Host.Models;

namespace MongoPractice.Host.Services
{
    public class ProductService : BaseService<Product>
    {
        public ProductService(IConfiguration config) : base(config, nameof(Product))
        {

        }
    }
}
