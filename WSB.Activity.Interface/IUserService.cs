using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSB.Activity;
using WSB.Activity.EFModel;

namespace WSB.Activity.Interface
{
    public interface IUserService : IBaseService
    {
        #region 用户模块
        /// <summary>
        /// 测试用户
        /// </summary>
        /// <returns></returns>
        User TestUser();

        /// <summary>
        /// 注册用户（微信openid、微信昵称、微信头像）
        /// </summary>
        /// <param name="openId">微信openid</param>
        /// <param name="nickname">微信昵称</param>
        /// <param name="avatar">微信头像</param>
        /// <returns></returns>
        User Register(string openId, string nickname, string avatar);

        /// <summary>
        /// 获取机会次数
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="chanceType"></param>
        /// <returns></returns>
        int GetChanceCount(int userId, ChanceTypeEnum chanceType);

        /// <summary>
        /// 获取总额度
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="chanceType"></param>
        /// <returns></returns>
        int GetTotalNumber(int userId, ChanceTypeEnum chanceType);
        #endregion

        #region 积分模块
        /// <summary>
        /// 获取兑换积分值
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetExchangeIntegral(int userId);

        /// <summary>
        /// 兑换积分
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="integral">积分增量</param>
        /// <returns></returns>
        int ExchangeIntegral(int userId, int integral);
        #endregion

        #region 红包模块
        /// <summary>
        /// 获取已经激活的的小红包列表
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        List<EFModel.View_UserReceiveRedpackets> GetActivatedRedpackets(int userId);

        /// <summary>
        /// 获取未激活的小红包列表
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        List<EFModel.View_UserReceiveRedpackets> GetUnActivatedRedpackets(int userId);
        #endregion

        #region 排行榜模块
        /// <summary>
        /// 获取排名名次
        /// </summary>
        /// <param name="chanceType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        int GetRankNumber(ChanceTypeEnum chanceType, int id);

        /// <summary>
        /// 获取排行榜榜单数据
        /// </summary>
        /// <param name="chanceType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PageResult<User> GetRankList(ChanceTypeEnum chanceType, int pageIndex = 1, int pageSize = 10);
        #endregion
    }
}
