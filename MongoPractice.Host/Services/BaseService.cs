using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoPractice.Host.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoPractice.Host.Services
{
    public class BaseService<T> where T : BaseModel
    {
        private readonly IMongoCollection<T> _collection;   //数据表操作对象

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config"></param>
        public BaseService(IConfiguration config,string tableName)
        {
            var client = new MongoClient(config.GetConnectionString("MongoConnection"));    //获取链接字符串
            var database = client.GetDatabase(config.GetConnectionString("DataBase"));   //数据库名 （不存在自动创建）
            //获取对特定数据表集合中的数据的访问
            _collection = database.GetCollection<T>(tableName);     // （不存在自动创建）
        }

        //Find<T> – 返回集合中与提供的搜索条件匹配的所有文档。
        //InsertOne – 插入提供的对象作为集合中的新文档。
        //ReplaceOne – 将与提供的搜索条件匹配的单个文档替换为提供的对象。
        //DeleteOne – 删除与提供的搜索条件匹配的单个文档。

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> Get() => await _collection.Find(T => true).ToListAsync();

        /// <summary>
        /// 获取单个
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> Get(string id)
        {
            return await _collection.Find<T>(T => T.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public async Task<T> Create(T T)
        {
            await _collection.InsertOneAsync(T);
            return T;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public async Task Create(IEnumerable<T> T) => await _collection.InsertManyAsync(T);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="TIn"></param>
        public async Task<bool> Update(string id, T TIn)
        {
            var result = await _collection.ReplaceOneAsync(T => T.Id == id, TIn);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        /// <summary>
        /// 删除
        /// </summary>
        public async Task<bool> Remove(string id)
        {
            var result = await _collection.DeleteOneAsync(x => x.Id == id); /*DeleteOneAsync(Builders<T>.Filter.Eq("Id", id);*/
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
        public async Task<bool> Remove()
        {
            var result = await _collection.DeleteManyAsync(new BsonDocument());
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<T> Get<S>(System.Linq.Expressions.Expression<Func<T, S>> orderBy, int pageIndex = 1, int pageSize = 10) => _collection.AsQueryable().OrderBy(orderBy).Skip(pageSize * (pageIndex - 1)).Take(pageSize);

    }
}