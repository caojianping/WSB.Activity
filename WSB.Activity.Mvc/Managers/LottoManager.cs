using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using WSB.Activity.Common;
using WSB.Activity.EFModel;
using WSB.Activity.Interface;

namespace WSB.Activity.Mvc
{
    public static class LottoManager
    {
        public enum LottoResult
        {
            [RemarkAttribute("用户信息已经过期")]
            SessionExpire = -1,

            [RemarkAttribute("您已经没有抽奖机会咯")]
            NoChance = 0,

            [RemarkAttribute("抽奖成功")]
            Success = 1
        }

        public static LottoResult Done(this HttpContextBase context, int userId, out int number)
        {
            if (userId <= 0)
            {
                number = 0;
                return LottoResult.SessionExpire;
            }
            IUnityContainer uc = DIFactory.GetContainer();
            IUserService userService = uc.Resolve<IUserService>();
            IUserLottoMapService userLottoMapService = uc.Resolve<IUserLottoMapService>();
            int count = userService.GetChanceCount(userId, ChanceTypeEnum.LottoChance);
            if (count <= 0)
            {
                number = 0;
                return LottoResult.NoChance;
            }
            UserLottoMap item = userLottoMapService.Create(userId,
                LottoHelper.GetProbRandomNumber(new Random(), null, null));
            number = item.Number;
            return LottoResult.Success;
        }
    }
}