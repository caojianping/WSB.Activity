using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WSB.Activity.WebHandler
{
    public class WeChatHandler : BaseHandler, IWeChatHandler
    {
        public WeChatHandler(string uri) : base(uri) { }

        /// <summary>
        /// 获取微信用户信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public UserInfo GetUserInfo(int type, string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new Exception("code不能为空！");
            }
            if (type == 0)
            {
                string uri = string.Format("{0}/{1}?code={2}", base.Uri, "userinfo", code);
                DataResult data = base.Deserialize<DataResult>(uri);
                if (data.status == 0)
                {
                    JObject result = (JObject)data.result;
                    return new UserInfo()
                    {
                        openId = result["openid"].ToString(),
                        nickname = result["nickname"].ToString(),
                        headimgurl = result["headimgurl"].ToString()
                    };
                }
                return null;
            }
            else if (type == 1)
            {
                string uri = string.Format("{0}/{1}?code={2}", base.Uri, "wxUserInfo!getUserInfo", code);
                UserInfo result = base.Deserialize<UserInfo>(uri);
                return result;
            }
            else
            {
                throw new Exception("无法识别的类型！");
            }
        }

        /// <summary>
        /// 获取微信js配置信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public JsConfigData GetJsConfigData(int type, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("url不能为空！");
            }
            string path = type == 1 ? "wxUserInfo!getJsApi" : "getJsConfigData";
            string uri = string.Format("{0}/{1}?url={2}", base.Uri, path, HttpUtility.UrlDecode(url));
            return (JsConfigData)base.Deserialize<JsConfigData>(uri);
        }

        /// <summary>
        /// 是否已经关注
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public bool HasAttention(string openId)
        {
            //if (string.IsNullOrEmpty(openId))
            //{
            //    throw new Exception("openId不能为空！");
            //}
            //string uri = string.Format("{0}/{1}?openId={2}", base.Uri, "wxUserInfo!hasAttention", openId);
            //return (bool)base.Deserialize<bool>(uri);
            return true;
        }
    }
}
