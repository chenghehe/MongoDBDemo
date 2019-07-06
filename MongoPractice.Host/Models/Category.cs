using MongoPractice.Host.Tool;

namespace MongoPractice.Host.Models
{
    public class Category : BaseModel
    {
        /// <summary>
        /// 类别
        /// </summary>
        public Category()
        {
            var start = nameof(Category)[0].ToString();
            Id = StringHelper.GetEntiyId(start);
        }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 介绍
        /// </summary>
        public string Details { get; set; } = string.Empty;
    }
}
