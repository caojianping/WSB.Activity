using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WSB.Activity.Common;

namespace WSB.Activity.WebHandler
{
    public class BaseHandler : IBaseHandler
    {
        private Logger _logger = Logger.CreateLogger(typeof(BaseHandler));

        /// <summary>
        /// 远程服务器地址
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// WebClient实例
        /// </summary>
        public WebClient Client { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="uri"></param>
        public BaseHandler(string uri)
        {
            this.Uri = uri;
            this.Client = new WebClient();
            this.Client.Encoding = Encoding.UTF8;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        public T Deserialize<T>(string uri)
        {
            try
            {
                _logger.Info(string.Format("Deserialize uri:{0}", uri));
                string html = this.Client.GetHtml(uri);
                _logger.Info(string.Format("Deserialize html:{0}", html));
                return JsonConvert.DeserializeObject<T>(html);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
