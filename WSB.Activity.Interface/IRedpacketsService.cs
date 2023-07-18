using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSB.Activity;

namespace WSB.Activity.Interface
{
    public interface IRedpacketsService : IBaseService
    {
        /// <summary>
        /// 创建一个大红包
        /// ps:产生一条随机金额的大红包
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="size">尺寸</param>
        /// <returns></returns>
        EFModel.Redpackets Create(int userId, int min, int max, Common.RedpacketsSizeEnum size);

        /// <summary>
        /// 获取大红包详情
        /// </summary>
        /// <param name="id">记录编号</param>
        /// <returns></returns>
        EFModel.View_Redpackets Detail(int id);

        /// <summary>
        /// 获取大红包的领取状态
        /// </summary>
        /// <param name="id">记录编号</param>
        /// <returns></returns>
        int GetReceiveStatus(int id);

        /// <summary>
        /// 获取老红包
        /// ps:获取状态为0的大红包
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        EFModel.Redpackets GetOldRedpackets(int userId);

        /// <summary>
        /// 是否已经发过大红包
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        bool HasSent(int userId);

        /// <summary>
        /// 设置大红包失效
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="expireTime">过期时间，单位：分</param>
        void SetInvalid(int userId, int expireTime);

        /// <summary>
        /// 获取我发出的大红包列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<EFModel.View_Redpackets> GetSendRedpackets(int userId);
    }
}