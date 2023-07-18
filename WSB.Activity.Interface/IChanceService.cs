using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSB.Activity.Interface
{
    /// <summary>
    /// 机会类型枚举
    /// </summary>
    public enum ChanceTypeEnum
    {
        RedpacketsChance = 1,
        LottoChance = 2
    }

    /// <summary>
    /// 红包规则类型枚举
    /// </summary>
    public enum RedpacketsRuleTypeEnum
    {
        InitRule = 1,
        AttentionRule = 2,
        TaskRule = 3
    }

    /// <summary>
    /// 抽奖规则类型枚举
    /// </summary>
    public enum LottoRuleTypeEnum
    {
        InitRule = 1,
        AttentionRule = 2,
        TaskRule = 3
    }

    public interface IChanceService
    {
        void AddChance(int userId, ChanceTypeEnum chanceType, int ruleType);

        void UpdateChance(int userId, ChanceTypeEnum chanceType);
    }
}
