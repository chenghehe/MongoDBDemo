using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoPractice.Host.Tool
{
    public class ModelHelper
    {
        #region 更新实体模型
        /// <summary>
        /// 更新实体模型
        /// </summary>
        /// <typeparam name="In">源实体类型</typeparam>
        /// <typeparam name="Out">最终合并后返回的实体类型</typeparam>
        /// <param name="inModel">源数据实体</param>
        /// <param name="outModel">最终合并后返回的实体</param>
        /// <returns>最终实体</returns>
        public static Out EntityMerge<In, Out>(In inModel, Out outModel)
        {
            Type type = inModel.GetType();
            Type outType = outModel.GetType();
            var properties = type.GetProperties();
            var outProperties = outType.GetProperties();
            foreach (var property in properties)
            {
                foreach (var item in outProperties)
                {
                    if (property.Name == item.Name)
                    {
                        var value = property.GetValue(inModel);
                        item.SetValue(outModel, value, null);
                        break;
                    }
                }
            }
            return outModel;
        }
        #endregion
    }
}
