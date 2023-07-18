using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSB.Activity.Common;
using WSB.Activity.EFModel;
using WSB.Activity.Interface;
using WSB.Activity.Mvc.Filters;
using WSB.Activity.Mvc.Models;

namespace WSB.Activity.Mvc.Controllers
{
    [ExceptionAttribute]
    public class LottoController : Controller
    {
        private static Logger _logger = Logger.CreateLogger(typeof(LottoController));

        #region identity
        private IUserLottoMapService _userLottoMapService = null;
        private IUserService _userService = null;
        private IChanceService _chanceService = null;

        public LottoController(IUserLottoMapService userLottoMapService, IUserService userService, IChanceService chanceService)
        {
            this._userLottoMapService = userLottoMapService;
            this._userService = userService;
            this._chanceService = chanceService;
        }
        #endregion

        [WeChat]
        [Chance(ChanceType = ChanceTypeEnum.LottoChance)]
        [HttpGet]
        public ActionResult Index()
        {
            SUSER suser = (SUSER)Session["SUSER"];
            return View(this.GetIndexViewModel(suser.UserId));
        }

        [Session]
        [HttpGet]
        public ActionResult Done()
        {
            if (!Request.IsAjaxRequest())
            {
                throw new Exception("不支持的请求方式！");
            }
            SUSER suser = (SUSER)Session["SUSER"];
            int number = 0;
            LottoManager.LottoResult result = this.HttpContext.Done(suser.UserId, out number);
            if (result != LottoManager.LottoResult.Success)
            {
                return Json(new { status = false, msg = result.GetRemark() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, data = number }, JsonRequestBehavior.AllowGet);
        }

        [Session]
        [Redpackets]
        [HttpGet]
        public ActionResult Activate(int id)
        {
            bool result = this._userLottoMapService.Activate(id);
            return RedirectToAction("Index", "Lotto", new { msg = result ? "领取成功" : "领取失败" });
        }

        #region 私有方法
        private LottoIndexViewModel GetIndexViewModel(int userId)
        {
            return new LottoIndexViewModel()
            {
                ChanceCount = this._userService.GetChanceCount(userId, ChanceTypeEnum.LottoChance),
                TotalNumber = this._userService.GetTotalNumber(userId, ChanceTypeEnum.LottoChance),
                LottoList = this._userLottoMapService.GetWinList(userId)
            };
        }
        #endregion
    }
}