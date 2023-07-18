using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using WSB.Activity.Common;
using WSB.Activity.EFModel;
using WSB.Activity.Interface;
using WSB.Activity.WebHandler;

namespace WSB.Activity.Mvc.Filters
{
    public class WeChatAttribute : ActionFilterAttribute
    {
        private Logger _logger = Logger.CreateLogger(typeof(WeChatAttribute));

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContextBase context = filterContext.HttpContext;
            HttpRequestBase request = context.Request;
            HttpResponseBase response = context.Response;

            string userAgent = request.UserAgent.ToLower();
            bool isWeChat = userAgent.Contains("micromessenger");
            _logger.Info($"isWeChat:{isWeChat}");
            if (!isWeChat)//测试通道
            {
                this.Test(context, request);
            }
            else//非测试通道
            {
                var suser = HttpContext.Current.Session["SUSER"];
                _logger.Info($"Session:{suser}");
                if (suser == null || !(suser is SUSER))
                {
                    string code = request.QueryString["code"];
                    _logger.Info($"code:{code}");
                    if (string.IsNullOrEmpty(code)) this.RedirectToWeChatServer(context, request, response);
                    else this.Done(context, code);
                }
                else return;
            }
        }

        /// <summary>
        /// 测试环境
        /// </summary>
        /// <param name="context"></param>
        /// <param name="request"></param>
        private void Test(HttpContextBase context, HttpRequestBase request)
        {
            var suser = HttpContext.Current.Session["SUSER"];
            _logger.Info($"Session:{suser}");
            if (suser == null || !(suser is SUSER))
            {
                string test = request.QueryString["test"];
                if (test == "1")
                {
                    IUserService service = DIFactory.GetContainer().Resolve<IUserService>();
                    User testUser = service.TestUser();
                    context.Session["SUSER"] = new SUSER()
                    {
                        UserId = testUser.Id,
                        Nickname = testUser.Nickname
                    };
                    context.Session.Timeout = 120;
                }
                else throw new Exception("亲！不支持非微信浏览器的访问！");
            }
            else return;
        }

        /// <summary>
        /// 跳转至微信服务器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void RedirectToWeChatServer(HttpContextBase context, HttpRequestBase request, HttpResponseBase response)
        {
            string path = request.Path;
            _logger.Info($"path:{path}");
            int type = ConfigHelper.DeployType;
            if (type == 0)//走测试公众号微信接口流程
            {
                if (path.StartsWith("/") || path.StartsWith("/User/Index") || path.StartsWith("/User/Rank") ||
                    path.StartsWith("/Redpackets/Detail") || path.StartsWith("/Lotto/Index"))
                {
                    string redirectUrl = BuildRedirectUrl(request, 0);
                    _logger.Info($"redirectUrl:{redirectUrl}");
                    response.Redirect(redirectUrl);
                }
                else throw new Exception("测试通道：不支持跳转至微信服务器的请求地址!");
            }
            else if (type == 1)//走合肥有线公众号微信接口流程
            {
                if (path.StartsWith("/Redpackets/Detail") || path.StartsWith("/Lotto/Index"))
                {
                    string redirectUrl = BuildRedirectUrl(request, 1);
                    _logger.Info($"redirectUrl:{redirectUrl}");
                    response.Redirect(redirectUrl);
                }
                else throw new Exception("不支持跳转至微信服务器的请求地址!");
            }
            else throw new Exception("错误的部署类型!");
        }

        /// <summary>
        /// 生成微信授权的跳转地址
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string BuildRedirectUrl(HttpRequestBase request, int type)
        {
            if (request == null)
            {
                throw new Exception("http请求为空！");
            }
            string url = request.Url.ToString();
            int port = request.Url.Port;
            if (port != 80)
            {
                string sport = string.Format(":{0}", port.ToString());
                url = url.Replace(sport, string.Empty);
            }
            _logger.Info($"BuildRedirectUrl url:{url}");
            string redirectUrl = QueryStringHelper.BuildUrl("https://open.weixin.qq.com/connect/oauth2/authorize?",
                new Dictionary<string, string>()
                {
                    { "appid", type == 0 ? ConfigHelper.TestAppId: ConfigHelper.HfyxAppId },
                    { "redirect_uri", HttpUtility.UrlEncode(url, Encoding.UTF8) },
                    { "response_type", "code" },
                    { "scope", "snsapi_userinfo" },
                    { "state", "1" },
                    { "connect_redirect", "1#wechat_redirect" }
                });
            return redirectUrl;
        }

        /// <summary>
        /// 正常流程处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="code"></param>
        private void Done(HttpContextBase context, string code)
        {
            if (context == null)
            {
                throw new Exception("http上下文为空！");
            }
            if (string.IsNullOrEmpty("code"))
            {
                throw new Exception("code不可以为空！");
            }
            int deployType = ConfigHelper.DeployType;
            WeChatHandler handler = new WeChatHandler(deployType == 1 ? ConfigHelper.HfyxWxUri : ConfigHelper.TestWxUri);
            UserInfo userInfo = handler.GetUserInfo(deployType, code);
            if (userInfo == null)
            {
                throw new Exception("微信用户信息为空！");
            }
            IUserService userService = DIFactory.GetContainer().Resolve<IUserService>();
            User user = userService.Register(userInfo.openId, userInfo.nickname, userInfo.headimgurl);
            context.Session["SUSER"] = new SUSER()
            {
                UserId = user.Id,
                Nickname = user.Nickname,
                OpenId = user.OpenId
            };
            context.Session.Timeout = 24 * 60 * 7;//七天
        }
    }
}