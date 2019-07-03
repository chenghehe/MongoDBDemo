using MongoPractice.Host.Tool;

namespace MongoPractice.Host.Models
{
    public class Product : BaseModel
    {
        public Product()
        {
            var start = nameof(Product)[0].ToString();
            Id = StringHelper.GetEntiyId(start);
        }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// 产品详情
        /// </summary>
        public string ProductDetails { get; set; } = string.Empty;


        /// <summary>
        /// 产品图片
        /// </summary>
        public string ImgUrl { get; set; } = string.Empty;

    }
}
