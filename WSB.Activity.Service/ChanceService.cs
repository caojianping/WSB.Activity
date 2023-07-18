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
    public class ChanceService : BaseService, IChanceService
    {
        private Logger _logger = Logger.CreateLogger(typeof(RedpacketsService));

        #region identity
        private DbSet<Chance> _chanceDbSet = null;

        public ChanceService(DbContext dbContext) : base(dbContext)
        {
            this._chanceDbSet = dbContext.Set<Chance>();
        }
        #endregion

        #region 添加机会模块
        public void AddChance(int userId, ChanceTypeEnum chanceType, int ruleType)
        {
            if (userId <= 0)
            {
                throw new Exception("无效的用户编号！");
            }
            if (chanceType == ChanceTypeEnum.RedpacketsChance)
            {
                AddRedpacketsChance(userId, (RedpacketsRuleTypeEnum)ruleType);
            }
            else if (chanceType == ChanceTypeEnum.LottoChance)
            {
                AddLottoChance(userId, (LottoRuleTypeEnum)ruleType);
            }
            else
            {
                throw new Exception("无效的机会类型！");
            }
        }

        private void AddRedpacketsChance(int userId, RedpacketsRuleTypeEnum redpacketsRuleType)
        {
            if (userId <= 0)
            {
                throw new Exception("无效的用户编号！");
            }
            const int chanceType = (int)ChanceTypeEnum.RedpacketsChance;
            Chance chance = new Chance()
            {
                UserId = userId,
                ChanceType = chanceType,
                RuleType = (int)redpacketsRuleType,
                Status = true,
                CreateTime = DateTime.Now
            };
            switch (redpacketsRuleType)
            {
                case RedpacketsRuleTypeEnum.InitRule:
                    chance.Count = 3;
                    ExecuteInitRule(userId, chanceType, chance);
                    break;
                case RedpacketsRuleTypeEnum.AttentionRule:
                    chance.Count = 1;
                    ExecuteAttentionRule(userId, chanceType, chance);
                    break;
                case RedpacketsRuleTypeEnum.TaskRule:
                    chance.Count = 1;
                    ExecuteTaskRule(userId, chanceType, chance);
                    break;
                default:
                    throw new Exception("无效的红包规则类型！");
            }
        }

        private void AddLottoChance(int userId, LottoRuleTypeEnum lottoRuleType)
        {
            if (userId <= 0)
            {
                throw new Exception("无效的用户编号！");
            }
            const int chanceType = (int)ChanceTypeEnum.LottoChance;
            Chance chance = new Chance()
            {
                UserId = userId,
                ChanceType = chanceType,
                RuleType = (int)lottoRuleType,
                Status = true,
                CreateTime = DateTime.Now
            };
            switch (lottoRuleType)
            {
                case LottoRuleTypeEnum.InitRule:
                    chance.Count = 3;
                    ExecuteInitRule(userId, chanceType, chance);
                    break;
                case LottoRuleTypeEnum.AttentionRule:
                    chance.Count = 1;
                    ExecuteAttentionRule(userId, chanceType, chance);
                    break;
                case LottoRuleTypeEnum.TaskRule:
                    chance.Count = 1;
                    ExecuteTaskRule(userId, chanceType, chance);
                    break;
                default:
                    throw new Exception("无效的抽奖规则类型！");
            }
        }

        private void ExecuteInitRule(int userId, int chanceType, Chance chance)
        {
            const int ruleType = (int)RedpacketsRuleTypeEnum.InitRule;
            Tuple<bool, Chance> existTuple = HasExist(userId, chanceType, ruleType);
            if (existTuple.Item1)
            {
                return;
            }
            base.Insert<Chance>(chance);
        }

        private void ExecuteAttentionRule(int userId, int chanceType, Chance chance)
        {
            User user = base.Find<User>(userId);
            if (user == null)
            {
                throw new Exception("用户不存在！");
            }
            using (var trans = base.Context.Database.BeginTransaction())
            {
                try
                {
                    //判断一：判断是否存在以前未使用的规则2（关注规则）的机会数据？
                    //如果已经存在，那么就更新此机会数据的状态，同时更新用户表的红包机会次数字段(-1)；如果不存在，那么不操作。
                    //判断二：判断是否已经添加过今天的规则2（关注规则）的机会数据？
                    //如果已经添加，那么不操作；如果没有添加，那么添加一条新记录，同时更新用户表的红包机会次数字段(+1)。
                    ChanceTypeEnum chanceTypeEnum = (ChanceTypeEnum)chanceType;
                    Tuple<bool, Chance> UnusedTuple = HasBeforeAttentionRuleUnused(userId, chanceTypeEnum);
                    Tuple<bool, Chance> ExistTuple = HasTodayAttentionRuleExist(userId, chanceTypeEnum);
                    if (UnusedTuple.Item1 && !ExistTuple.Item1)
                    {
                        //将以前未使用机会的状态置为无效
                        UnusedTuple.Item2.Count -= 1;
                        UnusedTuple.Item2.Status = false;
                        UnusedTuple.Item2.UpdateTime = DateTime.Now;
                        base.Update<Chance>(UnusedTuple.Item2);
                        //添加一条新的规则2机会记录                        
                        base.Insert<Chance>(chance);
                    }
                    else if (UnusedTuple.Item1 && ExistTuple.Item1)
                    {
                        //将以前未使用机会的状态置为无效
                        UnusedTuple.Item2.Count -= 1;
                        UnusedTuple.Item2.Status = false;
                        UnusedTuple.Item2.UpdateTime = DateTime.Now;
                        base.Update<Chance>(UnusedTuple.Item2);
                        //用户表红包/抽奖机会次数-1
                        if (chanceTypeEnum == ChanceTypeEnum.RedpacketsChance)
                        {
                            user.RedpacketsChanceCount -= 1;
                        }
                        else
                        {
                            user.LottoChanceCount -= 1;
                        }
                        user.UpdateTime = DateTime.Now;
                        base.Update<User>(user);
                    }
                    else if (!UnusedTuple.Item1 && !ExistTuple.Item1)
                    {
                        //添加一条新的规则2机会记录
                        base.Insert<Chance>(chance);
                        //用户表红包机会次数+1
                        if (chanceTypeEnum == ChanceTypeEnum.RedpacketsChance)
                        {
                            user.RedpacketsChanceCount += 1;
                        }
                        else
                        {
                            user.LottoChanceCount += 1;
                        }
                        user.UpdateTime = DateTime.Now;
                        base.Update<User>(user);
                    }
                    else return;
                    trans.Commit();
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

        private void ExecuteTaskRule(int userId, int chanceType, Chance chance)
        {
            const int ruleType = (int)RedpacketsRuleTypeEnum.TaskRule;
            Tuple<bool, Chance> existTuple = HasExist(userId, chanceType, ruleType);
            if (existTuple.Item1)
            {
                existTuple.Item2.Count += 1;
                existTuple.Item2.UpdateTime = DateTime.Now;
                base.Update<Chance>(existTuple.Item2);
            }
            else
            {
                base.Insert<Chance>(chance);
            }
        }

        private Tuple<bool, Chance> HasExist(int userId, int chanceType, int ruleType)
        {
            if (userId <= 0)
            {
                throw new Exception("无效的用户编号！");
            }
            if (chanceType <= 0)
            {
                throw new Exception("无效的机会类型！");
            }
            if (ruleType <= 0)
            {
                throw new Exception("无效的规则类型！");
            }
            var item = this._chanceDbSet.FirstOrDefault(f => f.UserId == userId
                && f.ChanceType == chanceType
                && f.RuleType == ruleType);
            return new Tuple<bool, Chance>(item != null, item);
        }

        private Tuple<bool, Chance> HasBeforeAttentionRuleUnused(int userId, ChanceTypeEnum chanceType)
        {
            if (userId <= 0)
            {
                throw new Exception("无效的用户编号！");
            }
            if (chanceType != ChanceTypeEnum.RedpacketsChance && chanceType != ChanceTypeEnum.LottoChance)
            {
                throw new Exception("无效的机会类型！");
            }
            DateTime now = DateTime.Now;
            DateTime todayMorning = new DateTime(now.Year, now.Month, now.Day);
            var item = this._chanceDbSet.FirstOrDefault(f => f.UserId == userId
                && f.ChanceType == (int)chanceType
                && f.RuleType == 2
                && f.Count > 0
                && f.Status == true
                && f.CreateTime < todayMorning);
            return new Tuple<bool, Chance>(item != null, item);
        }

        private Tuple<bool, Chance> HasTodayAttentionRuleExist(int userId, ChanceTypeEnum chanceType)
        {
            if (userId <= 0)
            {
                throw new Exception("无效的用户编号！");
            }
            if (chanceType != ChanceTypeEnum.RedpacketsChance && chanceType != ChanceTypeEnum.LottoChance)
            {
                throw new Exception("无效的机会类型！");
            }
            DateTime now = DateTime.Now;
            DateTime todayMorning = new DateTime(now.Year, now.Month, now.Day);
            DateTime tomorrowMorning = todayMorning.AddDays(1);
            var item = this._chanceDbSet.FirstOrDefault(f => f.UserId == userId
                && f.ChanceType == (int)chanceType
                && f.RuleType == 2
                && f.CreateTime >= todayMorning
                && f.CreateTime < tomorrowMorning);
            return new Tuple<bool, Chance>(item != null, item);
        }
        #endregion

        #region 更新机会模块
        public void UpdateChance(int userId, ChanceTypeEnum chanceType)
        {
            if (userId <= 0)
            {
                throw new Exception("无效的用户编号！");
            }
            if (chanceType == ChanceTypeEnum.RedpacketsChance)
            {
                UpdateRedpacketsChance(userId);
            }
            else if (chanceType == ChanceTypeEnum.LottoChance)
            {
                UpdateLottoChance(userId);
            }
            else
            {
                throw new Exception("无效的机会类型！");
            }
        }

        private void UpdateRedpacketsChance(int userId)
        {
            if (userId <= 0)
            {
                throw new Exception("无效的用户编号！");
            }
            const int chanceType = (int)ChanceTypeEnum.RedpacketsChance;
            User user = base.Find<User>(userId);
            Chance chance = null;
            //一级优先级：规则2
            DateTime now = DateTime.Now;
            DateTime todayMorning = new DateTime(now.Year, now.Month, now.Day);
            DateTime tomorrowMorning = todayMorning.AddDays(1);
            var attentionChance = this._chanceDbSet.FirstOrDefault(f => f.UserId == userId
                && f.ChanceType == chanceType
                && f.RuleType == (int)RedpacketsRuleTypeEnum.AttentionRule
                && f.Count > 0
                && f.Status == true
                && f.CreateTime >= todayMorning
                && f.CreateTime < tomorrowMorning);
            if (attentionChance != null)
            {
                attentionChance.Status = false;
                chance = attentionChance;
            }
            else
            {
                //二级优先级：规则3
                var taskChance = this._chanceDbSet.FirstOrDefault(f => f.UserId == userId
                    && f.ChanceType == chanceType
                    && f.RuleType == (int)RedpacketsRuleTypeEnum.TaskRule
                    && f.Count > 0
                    && f.Status == true);
                if (taskChance != null)
                {
                    chance = taskChance;
                }
                else
                {
                    //三级优先级：规则1
                    var initChance = this._chanceDbSet.FirstOrDefault(f => f.UserId == userId
                        && f.ChanceType == chanceType
                        && f.RuleType == (int)RedpacketsRuleTypeEnum.InitRule
                        && f.Count > 0
                        && f.Status == true);
                    if (initChance != null)
                    {
                        chance = initChance;
                    }
                }
            }
            if (chance == null)
            {
                return;
            }
            chance.Count -= 1;
            chance.UpdateTime = DateTime.Now;
            base.Update<Chance>(chance);
            user.RedpacketsChanceCount -= 1;
            user.UpdateTime = DateTime.Now;
            base.Update<User>(user);
        }

        private void UpdateLottoChance(int userId)
        {
            if (userId <= 0)
            {
                throw new Exception("无效的用户编号！");
            }
            const int chanceType = (int)ChanceTypeEnum.LottoChance;
            User user = base.Find<User>(userId);
            Chance chance = null;
            //一级优先级：规则2
            DateTime now = DateTime.Now;
            DateTime todayMorning = new DateTime(now.Year, now.Month, now.Day);
            DateTime tomorrowMorning = todayMorning.AddDays(1);
            var attentionChance = this._chanceDbSet.FirstOrDefault(f => f.UserId == userId
                && f.ChanceType == chanceType
                && f.RuleType == (int)LottoRuleTypeEnum.AttentionRule
                && f.Count > 0
                && f.Status == true
                && f.CreateTime >= todayMorning
                && f.CreateTime < tomorrowMorning);
            if (attentionChance != null)
            {
                attentionChance.Status = false;
                chance = attentionChance;
            }
            else
            {
                //二级优先级：规则3
                var taskChance = this._chanceDbSet.FirstOrDefault(f => f.UserId == userId
                    && f.ChanceType == chanceType
                    && f.RuleType == (int)LottoRuleTypeEnum.TaskRule
                    && f.Count > 0
                    && f.Status == true);
                if (taskChance != null)
                {
                    chance = taskChance;
                }
                else
                {
                    //三级优先级：规则1
                    var initChance = this._chanceDbSet.FirstOrDefault(f => f.UserId == userId
                        && f.ChanceType == chanceType
                        && f.RuleType == (int)LottoRuleTypeEnum.InitRule
                        && f.Count > 0
                        && f.Status == true);
                    if (initChance != null)
                    {
                        chance = initChance;
                    }
                }
            }
            if (chance == null)
            {
                return;
            }
            chance.Count -= 1;
            chance.UpdateTime = DateTime.Now;
            base.Update<Chance>(chance);
            user.LottoChanceCount -= 1;
            user.UpdateTime = DateTime.Now;
            base.Update<User>(user);
        }
        #endregion
    }
}
