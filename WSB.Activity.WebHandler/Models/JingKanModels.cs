using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSB.Activity.WebHandler
{
    public class JingKanDataResult
    {
        /// <summary>
        /// 处理状态：-1表示服务器处理异常；0表示未绑定经看平台；1表示成功数据；
        /// </summary>
        public int status { get; set; }
        public int result { get; set; }
        public object msg { get; set; }
    }
}
