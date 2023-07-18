using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSB.Activity.WebHandler
{
    public interface IJingKanHandler : IBaseHandler
    {
        JingKanDataResult GetIntegral(string openId);
    }
}
