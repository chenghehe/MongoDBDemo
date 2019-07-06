using MongoDB.Bson.Serialization.Attributes;
using MongoPractice.Host.Tool;
using System;

namespace MongoPractice.Host.Models
{
    /// <summary>
    /// 评论
    /// </summary>
    public class Comment
    {
        public Comment()
        {
            var start = nameof(Comment)[0].ToString();
            Id = StringHelper.GetEntiyId(start);
        }
        public string Id { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        [BsonDateTimeOptions]
        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 这是回复哪一条评论的Id？
        /// </summary>
        public string FormId { get; set; }
    }
}
