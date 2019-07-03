using System;
using System.Text;

namespace MongoPractice.Host.Tool
{
    public class StringHelper
    {
        public static string GetEntiyId(string startStr)
        {
            lock ("MongoPractice.Host.Tool.GetEntiyId_2019年7月3日")
            {
                return $"{startStr}{DateTime.Now.Ticks}{GetRandom(3)}";
            }
        }

        public static string GetRandom(int length, RandomModel randomModel = RandomModel.Number)
        {
            string str = string.Empty;
            switch (randomModel)
            {
                case RandomModel.CharacterNumber:
                    str = "ABCDEFGHJKMNPQRSTUVWXYZ0123456789";
                    break;
                case RandomModel.Character:
                    str = "ABCDEFGHJKMNPQRSTUVWXYZ";
                    break;
                default:
                    str = "0123456789";
                    break;
            }
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(str[new Random(Guid.NewGuid().GetHashCode()).Next(0, str.Length - 1)]);
            }
            return sb.ToString();
        }
    }
    public enum RandomModel
    {
        /// <summary>
        /// 字母
        /// </summary>
        Character,

        /// <summary>
        /// 数字
        /// </summary>
        Number,

        /// <summary>
        /// 字母+数据
        /// </summary>
        CharacterNumber
    }
}
