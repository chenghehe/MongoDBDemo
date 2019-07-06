using MongoPractice.Host.Tool;
using System.Collections.Generic;

namespace MongoPractice.Host.Models
{
    /// <summary>
    ///文章
    /// </summary>
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

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 分类ID
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// 评论
        /// </summary>
        public List<Comment> Comments { get; set; } = new List<Comment>();

        /// <summary>
        /// 点赞的用户
        /// </summary>
        public List<string> Fabulous { get; set; } = new List<string>();
    }
}
