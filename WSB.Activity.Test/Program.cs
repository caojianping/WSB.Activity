using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WSB.Activity.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            const int LOOP_COUNT = 10000;
            Random random = new Random();
            Dictionary<int, int> result = new Dictionary<int, int>();
            for (int i = 0; i < LOOP_COUNT; i++)
            {
                int number = AwardHelper.GetProbRandomNumber(random, null, null);
                if (result.ContainsKey(number))
                    result[number] += 1;
                else
                    result.Add(number, 1);
            }

            Console.WriteLine("\t\t出现次数\t占总共出现次数百分比");

            foreach (KeyValuePair<int, int> kv in result)
            {
                Console.WriteLine(kv.Key + "\t\t" + kv.Value.ToString() + "\t\t" + ((double)kv.Value / (double)LOOP_COUNT).ToString("0.00%"));
            }
            //Console.WriteLine($"result:{result}");
            Console.ReadKey();

            //TestGenericMethods tgm = new TestGenericMethods();
            //tgm.SayHi<Student>();
            //Console.ReadKey();

            //string str = "{\"status\":0,\"Msg\":}";
            //var result = (JingKanDataResult)JsonConvert.DeserializeObject<JingKanDataResult>(str);
            //Console.WriteLine(result);
            //Console.ReadKey();

            //Console.WriteLine(new DateTime(2099, 1, 1));
            //Console.ReadKey();

            //DateTime tomorrow = DateTime.Now.AddDays(1);
            //Console.WriteLine(tomorrow);
            //DateTime expireTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day);
            //Console.WriteLine(expireTime);
            //Console.ReadKey();
        }
    }

    public class AwardHelper
    {
        public static int GetProbRandomNumber(Random random, List<int> data, List<int> weights)
        {
            if (random == null)
            {
                return 0;
            }
            if (data == null)
            {
                data = new List<int>() { 0, 1, 2, 3, 4, 5 };
            }
            if (weights == null)
            {
                weights = new List<int>() { 10, 9, 8, 3, 1, 1 };
            }
            Dictionary<int, int> dict = new Dictionary<int, int>();
            for (int i = data.Count - 1; i >= 0; i--)
            {
                dict.Add(data[i], random.Next(100) * weights[i]);
            }
            Dictionary<int, int> sdict = dict.OrderByDescending(o => o.Value).ToDictionary(k => k.Key, v => v.Value);
            return sdict.First().Key;
        }
    }

    public class JingKanDataResult
    {
        public int Status { get; set; }
        public int Result { get; set; }
        public object Msg { get; set; }
    }
}
