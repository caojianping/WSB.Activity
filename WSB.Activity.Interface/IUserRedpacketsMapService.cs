using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSB.Activity;
using WSB.Activity.EFModel;

namespace WSB.Activity.Interface
{
    public interface IUserRedpacketsMapService : IBaseService
    {
        /// <summary>
        /// 创建一条领取记录
        /// ps:谁领到多少红包，产生一个随机金额的领取红包记录
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="redpacketsId">大红包编号</param>
        /// <param name="totalCount">总次数</param>
        /// <returns></returns>
        UserRedpacketsMap Create(int userId, int redpacketsId, int totalCount);

        /// <summary>
        /// 激活领到的红包
        /// </summary>
        /// <param name="id">记录编号</param>
        /// <returns></returns>
        bool Activate(int id);

        /// <summary>
        /// 是否已经领取过红包
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="redpacketsId">大红包编号</param>
        /// <returns></returns>
        bool HasReceived(int userId, int redpacketsId);

        /// <summary>
        /// 获取指定大红包被领取的小红包记录
        /// </summary>
        /// <param name="redpacketsId">大红包编号</param>
        /// <returns></returns>
        List<View_UserRedpacketsMap> GetReceiveList(int redpacketsId);
    }
}
