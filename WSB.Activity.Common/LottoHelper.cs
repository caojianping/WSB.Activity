using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSB.Activity.Common
{
    /// <summary>
    /// 抽奖帮助类
    /// </summary>
    public class LottoHelper
    {
        /// <summary>
        /// 获取概率性的随机数字
        /// </summary>
        /// <param name="random"></param>
        /// <param name="data"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static int GetProbRandomNumber(Random random, List<int> data, List<int> weights)
        {
            if (random == null)
            {
                return 0;
            }
            if (data == null)
            {
                data = new List<int>() { 0, 1, 5, 10, 20, 50 };
            }
            if (weights == null)
            {
                weights = new List<int>() { 10, 9, 8, 3, 1, 1 };
            }
            Dictionary<int, int> dict = new Dictionary<int, int>();
            for (int i = data.Count - 1; i >= 0; i--)
            {
                dict.Add(data[i], random.Next(100) * weights[i]);
            }
            Dictionary<int, int> sdict = dict.OrderByDescending(o => o.Value).ToDictionary(k => k.Key, v => v.Value);
            return sdict.First().Key;
        }
    }
}
