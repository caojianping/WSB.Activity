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
    public class UserLottoMapService : BaseService, IUserLottoMapService
    {
        #region identity
        private DbSet<UserLottoMap> _userLottoMapDbSet = null;
        private DbSet<View_UserLottoMap> _viewUserLottoMapDbSet = null;

        public UserLottoMapService(DbContext dbContext) : base(dbContext)
        {
            this._userLottoMapDbSet = dbContext.Set<UserLottoMap>();
            this._viewUserLottoMapDbSet = dbContext.Set<View_UserLottoMap>();
        }
        #endregion

        public UserLottoMap Create(int userId, int number)
        {
            if (userId <= 0)
            {
                throw new Exception("无效的用户编号！");
            }
            using (var trans = base.Context.Database.BeginTransaction())
            {
                try
                {
                    
                    UserLottoMap item = base.Insert<UserLottoMap>(new UserLottoMap()
                    {
                        UserId = userId,
                        Number = number,
                        LottoTime = DateTime.Now,
                        Status = false
                    });
                    new ChanceService(new ActivityContext()).UpdateChance(userId, ChanceTypeEnum.LottoChance);
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
            UserLottoMap item = base.Find<UserLottoMap>(id);
            if (item == null)
            {
                return false;
            }
            if (item.Status == true)
            {
                //throw new Exception("礼券已经激活！");
                return true;
            }
            using (var trans = base.Context.Database.BeginTransaction())
            {
                try
                {
                    item.Status = true;
                    item.UpdateTime = DateTime.Now;
                    base.Update<UserLottoMap>(item);
                    User user = base.Find<User>(item.UserId);
                    user.LottoTotalNumber += item.Number;
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

        public List<View_UserLottoMap> GetWinList(int userId)
        {
            return this._viewUserLottoMapDbSet
                .Where(w => w.UserId == userId)
                .ToList();
        }
    }
}
