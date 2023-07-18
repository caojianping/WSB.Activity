using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSB.Activity;
using WSB.Activity.EFModel;
using WSB.Activity.Interface;

namespace WSB.Activity.Service
{
    public class UserRedpacketsMapService : BaseService, IUserRedpacketsMapService
    {
        #region identity
        private DbSet<UserRedpacketsMap> _userRedpacketsMapDbSet = null;
        private DbSet<View_UserRedpacketsMap> _viewUserRedpacketsMapDbSet = null;

        public UserRedpacketsMapService(DbContext dbContext) : base(dbContext)
        {
            this._userRedpacketsMapDbSet = dbContext.Set<UserRedpacketsMap>();
            this._viewUserRedpacketsMapDbSet = dbContext.Set<View_UserRedpacketsMap>();
        }
        #endregion

        public UserRedpacketsMap Create(int userId, int redpacketsId, int totalCount)
        {
            Redpackets redpackets = base.Find<Redpackets>(redpacketsId);
            if (redpackets == null)
            {
                throw new Exception("指定的大红包已经不存在或者已经删除！");
            }
            int receiveCount = redpackets.ReceiveCount;
            int sum = base.Query<UserRedpacketsMap>(q => q.RedpacketsId == redpacketsId).Select(s => s.Number).ToList().Sum();
            double remainNum = (double)(redpackets.Total - sum);
            int remainCount = totalCount - (receiveCount + 1);
            using (var trans = base.Context.Database.BeginTransaction())
            {
                try
                {
                    UserRedpacketsMap entity = new UserRedpacketsMap()
                    {
                        UserId = userId,
                        RedpacketsId = redpacketsId,
                        ReceiveTime = DateTime.Now
                    };
                    if (receiveCount == totalCount - 1)
                    {
                        entity.Number = Convert.ToInt32(remainNum);
                        redpackets.ReceiveCount = totalCount;
                        redpackets.ReceiveStatus = 1;
                    }
                    else
                    {
                        double randomNum = Common.RedpacketsHelper.ReceiveRandomNumber(remainNum, remainCount, 1);
                        entity.Number = Convert.ToInt32(randomNum);
                        redpackets.ReceiveCount = receiveCount + 1;
                        redpackets.ReceiveStatus = 0;
                    }
                    //第一步：往UserRedpacketsMap表中，插入一条红包领取记录；
                    var item = base.Insert<UserRedpacketsMap>(entity);
                    //第二步：往Redpackets表中，更新红包的领取个数、领取状态、更新时间。
                    redpackets.UpdateTime = DateTime.Now;
                    base.Update<Redpackets>(redpackets);
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

        public bool Activate(int id)
        {
            if (id <= 0)
            {
                throw new Exception("无效的编号！");
            }
            UserRedpacketsMap item = base.Find<UserRedpacketsMap>(id);
            if (item == null)
            {
                return false;
            }
            using (var trans = base.Context.Database.BeginTransaction())
            {
                try
                {
                    item.Status = true;
                    item.UpdateTime = DateTime.Now;
                    base.Update<UserRedpacketsMap>(item);

                    User user = base.Find<User>(item.UserId);
                    user.RedpacketsTotalNumber += item.Number;
                    user.UpdateTime = DateTime.Now;
                    base.Update<User>(user);
                    trans.Commit();
                    return true;
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

        public bool HasReceived(int userId, int redpacketsId)
        {
            return this._userRedpacketsMapDbSet.FirstOrDefault(f => f.UserId == userId && f.RedpacketsId == redpacketsId) != null;
        }

        public List<View_UserRedpacketsMap> GetReceiveList(int redpacketsId)
        {
            return this._viewUserRedpacketsMapDbSet.Where(w => w.RedpacketsId == redpacketsId).ToList();
        }
    }
}
