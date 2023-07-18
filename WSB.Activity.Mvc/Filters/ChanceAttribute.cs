using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using WSB.Activity.Interface;
using WSB.Activity.WebHandler;

namespace WSB.Activity.Mvc.Filters
{
    public class ChanceAttribute : ActionFilterAttribute
    {
        public ChanceTypeEnum ChanceType { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var suser = HttpContext.Current.Session["SUSER"];
            if (suser != null && suser is SUSER)
            {
                SUSER tuser = (SUSER)suser;
                WeChatHandler handler = new WeChatHandler(ConfigHelper.DeployType == 1 ?
                    ConfigHelper.HfyxWxUri : ConfigHelper.TestWxUri);
                bool isAttention = handler.HasAttention(tuser.OpenId);
                if (isAttention)
                {
                    IChanceService service = DIFactory.GetContainer().Resolve<IChanceService>();
                    int ruleType;
                    if (this.ChanceType == ChanceTypeEnum.RedpacketsChance)
                    {
                        ruleType = (int)RedpacketsRuleTypeEnum.AttentionRule;
                    }
                    else
                    {
                        ruleType = (int)LottoRuleTypeEnum.AttentionRule;
                    }
                    service.AddChance(tuser.UserId, this.ChanceType, ruleType);
                }
            }
        }
    }
}