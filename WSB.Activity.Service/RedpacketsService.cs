using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSB.Activity;
using WSB.Activity.EFModel;
using WSB.Activity.Interface;
using WSB.Activity.Common;

namespace WSB.Activity.Service
{
    public class RedpacketsService : BaseService, IRedpacketsService
    {
        private Logger _logger = Logger.CreateLogger(typeof(RedpacketsService));

        #region identity
        private DbSet<Redpackets> _redpacketsDbSet = null;
        private DbSet<View_Redpackets> _viewRedpacketsDbSet = null;

        public RedpacketsService(DbContext dbContext) : base(dbContext)
        {
            this._redpacketsDbSet = dbContext.Set<Redpackets>();
            this._viewRedpacketsDbSet = dbContext.Set<View_Redpackets>();
        }
        #endregion

        public Redpackets Create(int userId, int min, int max, RedpacketsSizeEnum size)
        {
            if (userId <= 0)
            {
                throw new Exception("无效的编号！");
            }
            if (min <= 0 || max <= 0)
            {
                throw new Exception("无效的红包配置参数！");
            }
            using (var trans = base.Context.Database.BeginTransaction())
            {
                try
                {
                    Redpackets entity = new Redpackets()
                    {
                        UserId = userId,
                        Total = RedpacketsHelper.SendRandomNumber(min, max, size),
                        ReceiveCount = 0,
                        ReceiveStatus = 0,
                        CreateTime = DateTime.Now
                    };
                    Redpackets item = base.Insert<Redpackets>(entity);
                    IChanceService chanceService = new ChanceService(new ActivityContext());
                    chanceService.UpdateChance(userId, ChanceTypeEnum.RedpacketsChance);
                    trans.Commit();
                    return item;
                }
                catch (Exception ex)
                {
                    if (trans != null)
                    {
                        trans.Rollback();
                    }
                    throw ex;
                }
            }
        }

        public View_Redpackets Detail(int id)
        {
            //return base.Find<View_Redpackets>(id);//此处的视图对象没有主键，所以Find方法不可用
            return this._viewRedpacketsDbSet.FirstOrDefault(f => f.Id == id);
        }

        public int GetReceiveStatus(int id)
        {
            Redpackets item = this.Find<Redpackets>(id);
            if (item == null)
            {
                throw new Exception("红包不存在或已删除！");
            }
            return item.ReceiveStatus;
        }

        public Redpackets GetOldRedpackets(int userId)
        {
            return this._redpacketsDbSet.FirstOrDefault(f => f.UserId == userId && f.ReceiveStatus == 0);
        }

        public bool HasSent(int userId)
        {
            return this._redpacketsDbSet.FirstOrDefault(f => f.UserId == userId && f.ReceiveStatus == 0) != null;
        }

        public void SetInvalid(int userId, int expireTime)
        {
            //理论上，一个用户只会存在一条最新的红包数据
            Redpackets item = this._redpacketsDbSet.FirstOrDefault(f => f.UserId == userId && f.ReceiveStatus == 0 && DbFunctions.AddMinutes(f.CreateTime, expireTime) < DateTime.Now);
            if (item != null)
            {
                item.ReceiveStatus = -1;
                item.UpdateTime = DateTime.Now;
                base.Update<Redpackets>(item);
            }
        }

        public List<View_Redpackets> GetSendRedpackets(int userId)
        {
            return this._viewRedpacketsDbSet
                .Where(w => w.UserId == userId)
                .OrderByDescending(o => o.CreateTime)
                .ToList();
        }
    }
}
