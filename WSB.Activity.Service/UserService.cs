using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WSB.Activity;
using WSB.Activity.EFModel;
using WSB.Activity.Interface;
using WSB.Activity.Common;

namespace WSB.Activity.Service
{
    public class UserService : BaseService, IUserService
    {
        private Logger _logger = Logger.CreateLogger(typeof(UserService));

        #region identity
        private DbSet<User> _userDbSet = null;
        private DbSet<View_Redpackets> _viewRedpacketsDbSet = null;
        private DbSet<View_UserReceiveRedpackets> _viewUserReceiveRedpacketsDbSet = null;

        public UserService(DbContext dbContext) : base(dbContext)
        {
            this._userDbSet = dbContext.Set<User>();
            this._viewRedpacketsDbSet = dbContext.Set<View_Redpackets>();
            this._viewUserReceiveRedpacketsDbSet = dbContext.Set<View_UserReceiveRedpackets>();
        }
        #endregion

        #region 用户模块
        public User TestUser()
        {
            return this._userDbSet.OrderBy(o => o.CreateTime).First();
        }

        private Tuple<bool, User> HasRegister(string openId)
        {
            var item = this._userDbSet.FirstOrDefault(f => f.OpenId == openId);
            return new Tuple<bool, User>(item != null, item);
        }

        public User Register(string openId, string nickname, string avatar)
        {
            if (string.IsNullOrEmpty(openId))
            {
                throw new Exception("无效的微信openid！");
            }
            Tuple<bool, User> tuple = this.HasRegister(openId);
            User item = tuple.Item2;
            if (item != null)
            {
                //更新操作
                item.Nickname = nickname;
                item.Avatar = avatar;
                item.UpdateTime = DateTime.Now;
                base.Update<User>(item);
                return item;
            }
            else
            {
                //插入操作，同时插入Chance初始化数据
                using (var trans = base.Context.Database.BeginTransaction())
                {
                    try
                    {
                        const int count = 3;
                        User user = base.Insert<User>(new User()
                        {
                            OpenId = openId,
                            Nickname = nickname,
                            Avatar = avatar,
                            RedpacketsChanceCount = count,
                            LottoChanceCount = count,
                            CreateTime = DateTime.Now
                        });
                        IChanceService chanceService = new ChanceService(new ActivityContext());
                        chanceService.AddChance(user.Id, ChanceTypeEnum.RedpacketsChance, (int)RedpacketsRuleTypeEnum.InitRule);
                        chanceService.AddChance(user.Id, ChanceTypeEnum.LottoChance, (int)LottoRuleTypeEnum.InitRule);
                        trans.Commit();
                        return user;
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
        }

        public int GetChanceCount(int userId, ChanceTypeEnum chanceType)
        {
            User user = base.Find<User>(userId);
            if (user == null)
            {
                return 0;
            }
            if (chanceType == ChanceTypeEnum.RedpacketsChance)
            {
                return user.RedpacketsChanceCount;
            }
            else if (chanceType == ChanceTypeEnum.LottoChance)
            {
                return user.LottoChanceCount;
            }
            else
            {
                throw new Exception("无效的机会类型！");
            }
        }

        public int GetTotalNumber(int userId, ChanceTypeEnum chanceType)
        {
            User user = base.Find<User>(userId);
            if (user == null)
            {
                return 0;
            }
            if (chanceType == ChanceTypeEnum.RedpacketsChance)
            {
                return user.RedpacketsTotalNumber;
            }
            else if (chanceType == ChanceTypeEnum.LottoChance)
            {
                return user.LottoTotalNumber;
            }
            else
            {
                throw new Exception("无效的机会类型！");
            }
        }
        #endregion

        #region 积分模块
        public int GetExchangeIntegral(int userId)
        {
            User user = base.Find<User>(userId);
            return user == null ? 0 : user.ExchangeIntegral;
        }

        public int ExchangeIntegral(int userId, int integral)
        {
            User user = base.Find<User>(userId);
            if (user == null)
            {
                throw new Exception("用户不存在！");
            }
            if (integral <= 0)
            {
                return user.ExchangeIntegral;
            }
            using (var trans = base.Context.Database.BeginTransaction())
            {
                try
                {
                    const int count = 1;
                    user.ExchangeIntegral += integral;
                    user.RedpacketsChanceCount += count;
                    user.LottoChanceCount += count;
                    user.UpdateTime = DateTime.Now;
                    base.Update<User>(user);
                    IChanceService chanceService = new ChanceService(new ActivityContext());
                    chanceService.AddChance(userId, ChanceTypeEnum.RedpacketsChance, (int)RedpacketsRuleTypeEnum.TaskRule);
                    chanceService.AddChance(userId, ChanceTypeEnum.LottoChance, (int)LottoRuleTypeEnum.TaskRule);
                    trans.Commit();
                    return user.ExchangeIntegral;
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
        #endregion

        #region 红包模块
        public List<View_UserReceiveRedpackets> GetActivatedRedpackets(int userId)
        {
            return this._viewUserReceiveRedpacketsDbSet
                .Where(w => w.UserId == userId && w.Status == true)
                .OrderByDescending(o => o.ReceiveTime)
                .ToList();
        }

        public List<View_UserReceiveRedpackets> GetUnActivatedRedpackets(int userId)
        {
            return this._viewUserReceiveRedpacketsDbSet
                .Where(w => w.UserId == userId && w.Status == false)
                .OrderByDescending(o => o.ReceiveTime)
                .ToList();
        }
        #endregion

        #region 排行榜模块
        public int GetRankNumber(ChanceTypeEnum chanceType, int id)
        {
            if (id <= 0)
            {
                throw new Exception("无效的编号！");
            }
            IOrderedQueryable<User> items = null;
            if (chanceType == ChanceTypeEnum.RedpacketsChance)
            {
                items = this._userDbSet.OrderByDescending(o => o.RedpacketsTotalNumber);
            }
            else if (chanceType == ChanceTypeEnum.LottoChance)
            {
                items = this._userDbSet.OrderByDescending(o => o.LottoTotalNumber);
            }
            else
            {
                throw new Exception("无效的类型！");
            }
            if (items == null)
            {
                return 0;
            }
            int result = 0;
            int i = 0;
            foreach (var item in items)
            {
                i++;
                if (item.Id == id)
                {
                    result = i;
                }
            }
            return result;
        }

        public PageResult<User> GetRankList(ChanceTypeEnum chanceType, int pageIndex = 1, int pageSize = 10)
        {
            if (chanceType == ChanceTypeEnum.RedpacketsChance)
            {
                return base.QueryPage<User, bool>(null, pageIndex, pageSize, "RedpacketsTotalNumber", true);
            }
            else if (chanceType == ChanceTypeEnum.LottoChance)
            {
                return base.QueryPage<User, bool>(null, pageIndex, pageSize, "LottoTotalNumber", true);
            }
            else
            {
                throw new Exception("无效的类型！");
            }
        }
        #endregion
    }
}
