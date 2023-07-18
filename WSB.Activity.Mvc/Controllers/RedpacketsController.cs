using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class RedpacketsController : Controller
    {
        private static Logger _logger = Logger.CreateLogger(typeof(RedpacketsController));

        #region identity
        private IRedpacketsService _redpacketsService = null;
        private IUserRedpacketsMapService _userRedpacketsMapService = null;
        private IUserService _userService = null;
        private IChanceService _chanceService = null;

        public RedpacketsController(IRedpacketsService redpacketsService,
            IUserRedpacketsMapService userRedpacketsMapService,
            IUserService userService,
            IChanceService chanceService)
        {
            this._redpacketsService = redpacketsService;
            this._userRedpacketsMapService = userRedpacketsMapService;
            this._userService = userService;
            this._chanceService = chanceService;
        }
        #endregion

        /// <summary>
        /// 发红包视图
        /// </summary>
        /// <returns></returns>
        [Session]
        [Chance(ChanceType = ChanceTypeEnum.RedpacketsChance)]
        [Redpackets]
        [HttpGet]
        public ActionResult Send()
        {
            SUSER suser = (SUSER)Session["SUSER"];
            return View(this.GetSendViewModel(suser.UserId));
        }

        /// <summary>
        /// 发红包动作
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [Session]
        [Redpackets]
        [HttpPost]
        public ActionResult Send(SendForm form)
        {
            SUSER suser = (SUSER)Session["SUSER"];
            int userId = suser.UserId;
            Redpackets redpackets = null;
            RedpacketsManager.SendResult result = this.HttpContext.Send(userId, out redpackets);
            if (result != RedpacketsManager.SendResult.Success)
            {
                ViewBag.Msg = result.GetRemark();
                return View(this.GetSendViewModel(userId));
            }
            RedpacketsSendViewModel model = this.GetSendViewModel(userId);
            model.NewRedpackets = redpackets;
            return View(model);
        }

        /// <summary>
        /// 红包详情视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WeChat]
        [Redpackets]
        [HttpGet]
        public ActionResult Detail(int id)
        {
            return View(GetDetailViewModel(id));
        }

        /// <summary>
        /// 抢红包动作
        /// </summary>
        /// <param name="receiveForm"></param>
        /// <returns></returns>
        [Session]
        [Redpackets]
        [HttpPost]
        public ActionResult Detail(ReceiveForm receiveForm)
        {
            SUSER suser = (SUSER)Session["SUSER"];
            User user = this._userService.Find<User>(suser.UserId);
            int mapId;
            RedpacketsManager.ReceiveResult result = this.HttpContext.Receive(receiveForm.Id, user.Id, out mapId);
            if (result != RedpacketsManager.ReceiveResult.Success)
            {
                ViewBag.Msg = result.GetRemark();
                return View(GetDetailViewModel(receiveForm.Id));
            }
            return RedirectToAction("Receive", new { id = mapId, redpacketsId = receiveForm.Id });
        }

        /// <summary>
        /// 抢红包视图
        /// </summary>
        /// <param name="id"></param>
        /// <param name="redpacketsId"></param>
        /// <returns></returns>
        [Session]
        [Redpackets]
        [HttpGet]
        public ActionResult Receive(int id, int redpacketsId)
        {
            RedpacketsReceiveViewModel model = new RedpacketsReceiveViewModel()
            {
                RedpacketsId = redpacketsId,
                UserRedpacketsMap = this._userRedpacketsMapService.Find<UserRedpacketsMap>(id),
                ReceiveList = this._userRedpacketsMapService.GetReceiveList(redpacketsId)
            };
            return View(model);
        }

        /// <summary>
        /// 激活红包
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Session]
        [Redpackets]
        [HttpGet]
        public ActionResult Activate(int id)
        {
            bool result = this._userRedpacketsMapService.Activate(id);
            return RedirectToAction("Index", "User", new { msg = result ? "领取成功" : "领取失败" });
        }

        /// <summary>
        /// 活动任务视图
        /// </summary>
        /// <returns></returns>
        [Session]
        [HttpGet]
        public ActionResult Task()
        {
            SUSER suser = (SUSER)Session["SUSER"];
            ViewBag.OpenId = suser.OpenId;
            return View();
        }

        /// <summary>
        /// 玩法介绍视图
        /// </summary>
        /// <returns></returns>
        [Session]
        [HttpGet]
        public ActionResult Introduce()
        {
            return View();
        }

        #region 私有方法
        private RedpacketsSendViewModel GetSendViewModel(int userId)
        {
            RedpacketsSendViewModel model = new RedpacketsSendViewModel()
            {
                ChanceCount = this._userService.GetChanceCount(userId, ChanceTypeEnum.RedpacketsChance),
                OldRedpackets = this._redpacketsService.GetOldRedpackets(userId)
            };
            return model;
        }

        private RedpacketsDetailViewModel GetDetailViewModel(int id)
        {
            string url = Request.Url.ToString();
            return new RedpacketsDetailViewModel()
            {
                Redpackets = this._redpacketsService.Detail(id),
                ReceiveList = this._userRedpacketsMapService.GetReceiveList(id),
                ReceiveForm = new ReceiveForm()
            };
        }
        #endregion
    }
}