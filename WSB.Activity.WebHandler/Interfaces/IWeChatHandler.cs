using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSB.Activity.WebHandler
{
    public interface IWeChatHandler : IBaseHandler
    {        
        UserInfo GetUserInfo(int type, string code);
        JsConfigData GetJsConfigData(int type, string url);
        bool HasAttention(string openId);
    }
}