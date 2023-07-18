using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSB.Activity.Common
{
    /// <summary>
    /// 查询参数帮助类
    /// </summary>
    public class QueryStringHelper
    {
        public static string BuildUrl(string host, Dictionary<string, string> args)
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, string> kv in args)
            {
                string item = string.Format("{0}={1}", kv.Key, kv.Value);
                list.Add(item);
            }
            return host + string.Join("&", list);
        }
    }
}
