using Microsoft.Extensions.Configuration;
using MongoPractice.Host.Models;

namespace MongoPractice.Host.Services
{
    public class CategoryService : BaseService<Category>
    {
        public CategoryService(IConfiguration config) : base(config,nameof(Category))
        {

        }
    }
}
