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
    public static class RedpacketsManager
    {
        /// <summary>
        /// 发红包结果枚举
        /// </summary>
        public enum SendResult
        {
            [RemarkAttribute("用户信息已经过期")]
            SessionExpire = -2,

            [RemarkAttribute("您已经没有机会咯")]
            NoChance = -1,

            [RemarkAttribute("已经发过")]
            HasSent = 0,

            [RemarkAttribute("发送成功")]
            Success = 1,
        }

        public static SendResult Send(this HttpContextBase context, int userId, out Redpackets redpackets)
        {
            if (userId <= 0)
            {
                redpackets = null;
                return SendResult.SessionExpire;
            }
            IUnityContainer uc = DIFactory.GetContainer();
            IUserService userService = uc.Resolve<IUserService>();
            IRedpacketsService redpacketsService = uc.Resolve<IRedpacketsService>();
            int count = userService.GetChanceCount(userId, ChanceTypeEnum.RedpacketsChance);
            if (count <= 0)
            {
                redpackets = null;
                return SendResult.NoChance;
            }
            bool isSent = redpacketsService.HasSent(userId);
            if (isSent)
            {
                redpackets = null;
                return SendResult.HasSent;
            }
            //随机宽带红包在min*size至max*size天数之间
            redpackets = redpacketsService.Create(userId,
                ConfigHelper.BigRedpackets_Min,
                ConfigHelper.BigRedpackets_Max,
                (RedpacketsSizeEnum)ConfigHelper.BigRedpackets_Size);
            return SendResult.Success;
        }


        /// <summary>
        /// 领红包结果枚举
        /// </summary>
        public enum ReceiveResult
        {
            [RemarkAttribute("用户信息已经过期")]
            SessionExpire = -3,

            [RemarkAttribute("这个红包过期了T^T")]
            Expire = -2,

            [RemarkAttribute("手慢啦，该红包已被抢完>_<")]
            Finish = -1,

            [RemarkAttribute("这个红包你已经领取过了哦^o^")]
            Received = 0,

            [RemarkAttribute("领取成功")]
            Success = 1
        }

        public static ReceiveResult Receive(this HttpContextBase context, int redpacketsId, int userId, out int mapId)
        {
            if (redpacketsId <= 0)
            {
                throw new Exception("无效的红包编号！");
            }
            if (userId <= 0)
            {
                mapId = 0;
                return ReceiveResult.SessionExpire;
            }
            IUnityContainer uc = DIFactory.GetContainer();
            IRedpacketsService redpacketsService = uc.Resolve<IRedpacketsService>();
            IUserRedpacketsMapService userRedpacketsMapService = uc.Resolve<IUserRedpacketsMapService>();
            int status = redpacketsService.GetReceiveStatus(redpacketsId);
            if (status == -1)
            {
                mapId = 0;
                return ReceiveResult.Expire;
            }
            if (redpacketsService.GetReceiveStatus(redpacketsId) == 1)
            {
                mapId = 0;
                return ReceiveResult.Finish;
            }
            if (userRedpacketsMapService.HasReceived(userId, redpacketsId))
            {
                mapId = 0;
                return ReceiveResult.Received;
            }
            UserRedpacketsMap item = userRedpacketsMapService.Create(userId, redpacketsId, Properties.Settings.Default.SmallRedpackets_ReceiveCount);
            mapId = item.Id;
            return ReceiveResult.Success;
        }
    }
}