using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSB.Activity.EFModel;
using WSB.Activity.Interface;
using WSB.Activity.Common;
using WSB.Activity.Mvc.Models;
using WSB.Activity.Mvc.Filters;
using WSB.Activity.WebHandler;

namespace WSB.Activity.Mvc.Controllers
{
    [ExceptionAttribute]
    public class UserController : Controller
    {
        private static Logger _logger = Logger.CreateLogger(typeof(UserController));

        #region identity
        private IUserService _userService = null;
        private IRedpacketsService _redpacketsService = null;

        public UserController(IUserService userService, IRedpacketsService redpacketsService)
        {
            this._userService = userService;
            this._redpacketsService = redpacketsService;
        }
        #endregion

        [WeChat(Order = 1)]
        [Session(Order = 2)]
        [Redpackets(Order = 3)]
        [HttpGet]
        public ActionResult Index()
        {
            SUSER suser = (SUSER)Session["SUSER"];
            int userId = suser.UserId;
            UserIndexViewModel model = new UserIndexViewModel()
            {
                User = this._userService.Find<User>(userId),
                UnActivatedList = this._userService.GetUnActivatedRedpackets(userId),
                ActivatedList = this._userService.GetActivatedRedpackets(userId),
                SendList = this._redpacketsService.GetSendRedpackets(userId)
            };
            ViewBag.Msg = Request.QueryString["msg"];
            return View(model);
        }

        /// <summary>
        /// 获取微信分享组件签名数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [Session]
        [HttpPost]
        public JsonResult GetJsConfigData(string url)
        {
            if (!Request.IsAjaxRequest())
            {
                throw new Exception("不支持的请求方式！");
            }
            int deployType = ConfigHelper.DeployType;
            WeChatHandler handler = new WeChatHandler(deployType == 1 ? ConfigHelper.HfyxWxUri : ConfigHelper.TestWxUri);
            var result = handler.GetJsConfigData(deployType, url);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取合肥有线用户绑定到经看平台的用户积分
        /// </summary>
        /// <returns></returns>
        [Session]
        [HttpGet]
        public JsonResult GetIntegral()
        {
            if (!Request.IsAjaxRequest())
            {
                throw new Exception("不支持的请求方式！");
            }
            SUSER suser = (SUSER)Session["SUSER"];
            var result = new JingKanHandler(ConfigHelper.JingKanUri).GetIntegral(suser.OpenId);
            if (result.status == 1)
            {
                int totalIntegral = result.result;
                int exchangeIntegral = this._userService.GetExchangeIntegral(suser.UserId);
                result.result = totalIntegral - exchangeIntegral;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 兑换积分
        /// </summary>
        /// <param name="integral"></param>
        /// <returns></returns>
        [Session]
        [HttpPost]
        public JsonResult ExchangeIntegral(int integral)
        {
            if (!Request.IsAjaxRequest())
            {
                throw new Exception("不支持的请求方式！");
            }
            SUSER suser = (SUSER)Session["SUSER"];
            var result = new JingKanHandler(ConfigHelper.JingKanUri).GetIntegral(suser.OpenId);
            if (result.status == 1)
            {
                int totalIntegral = result.result;
                int exchangeIntegral = this._userService.ExchangeIntegral(suser.UserId, integral);
                return Json(totalIntegral - exchangeIntegral, JsonRequestBehavior.AllowGet);
            }
            else return Json(0, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 排行榜视图
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [WeChat(Order = 1)]
        [Session(Order = 2)]
        [HttpGet]
        public ActionResult Rank(int pageIndex = 1, int pageSize = 10)
        {
            SUSER suser = (SUSER)Session["SUSER"];
            int UserId = suser.UserId;
            ChanceTypeEnum chanceType = ChanceTypeEnum.RedpacketsChance;
            RankViewModel model = new RankViewModel()
            {
                User = this._userService.Find<User>(UserId),
                RankNumber = this._userService.GetRankNumber(chanceType, UserId),
                PageResult = this._userService.GetRankList(chanceType, pageIndex, pageSize)
            };
            return View(model);
        }
    }
}