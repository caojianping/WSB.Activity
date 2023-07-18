using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSB.Activity.Common
{
    /// <summary>
    /// 红包大小枚举
    /// </summary>
    public enum RedpacketsSizeEnum
    {
        TEN = 10,
        HUNDRED = 100,
        THOUSAND = 1000
    }

    /// <summary>
    /// 红包帮助类
    /// </summary>
    public class RedpacketsHelper
    {
        /// <summary>
        /// 发送一个随机大红包
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int SendRandomNumber(int min, int max, RedpacketsSizeEnum size)
        {
            if (min < 1 || min > 10)
            {
                min = 1;
            }
            if (max < 1 || max > 10)
            {
                max = 10;
            }
            int isize = (int)size;
            Random random = new Random();
            return random.Next(min, max) * isize;
        }

        /// <summary>
        /// 接收整体上随机小红包
        /// </summary>
        /// <param name="remainNum"></param>
        /// <param name="remainCount"></param>
        /// <param name="minNum"></param>
        /// <returns></returns>
        public static double ReceiveRandomNumber(double remainNum, int remainCount, double minNum)
        {
            if (remainNum <= 0 || remainCount <= 0 || minNum <= 0)
            {
                return 0;
            }
            double safeNum = (remainNum - remainCount * minNum) / remainCount;
            double randomNum = new Random().NextDouble() * (safeNum * 100 - minNum * 100) + minNum * 100;
            randomNum = randomNum / 100;
            randomNum = Math.Round(randomNum, 2, MidpointRounding.AwayFromZero);
            return randomNum;
        }
    }
}
