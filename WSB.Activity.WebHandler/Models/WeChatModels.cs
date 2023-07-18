using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSB.Activity.WebHandler
{
    public class DataResult
    {
        public int status { get; set; }
        public object result { get; set; }
    }

    public class UserInfo
    {
        public string openId { get; set; }
        public string nickname { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string headimgurl { get; set; }
    }

    public class JsConfigData
    {
        public string noncestr { get; set; }
        public string jsapi_ticket { get; set; }
        public string timestamp { get; set; }
        public string url { get; set; }
        public string signature { get; set; }
        public string appID { get; set; }
    }
}
