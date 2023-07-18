using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSB.Activity.WebHandler
{
    public class JingKanHandler : BaseHandler, IJingKanHandler
    {
        public JingKanHandler(string uri) : base(uri) { }

        /// <summary>
        /// 获取合肥有线用户绑定到经看平台的用户积分
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public JingKanDataResult GetIntegral(string openId)
        {
            if (string.IsNullOrEmpty(openId))
            {
                throw new Exception("openId不能为空！");
            }
            string uri = string.Format("{0}/{1}/{2}", base.Uri, "phone/getexpgoal", openId);
            return (JingKanDataResult)base.Deserialize<JingKanDataResult>(uri);
        }
    }
}