using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MongoPractice.Host.Models
{
    public class BaseModel
    {
        [BsonId]
        public ObjectId InternalId { get; set; }

        public string Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [BsonDateTimeOptions]
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
