using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSB.Activity.EFModel;

namespace WSB.Activity.Interface
{
    public interface IUserLottoMapService
    {
        /// <summary>
        /// 创建一条抽奖记录
        /// ps:谁抽到多少礼券
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="number">礼券额度</param>
        UserLottoMap Create(int userId, int number);

        /// <summary>
        /// 激活抽到的礼券
        /// </summary>
        /// <param name="id">记录编号</param>
        /// <returns></returns>
        bool Activate(int id);

        /// <summary>
        /// 获取中奖记录
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        List<View_UserLottoMap> GetWinList(int userId);
    }
}
