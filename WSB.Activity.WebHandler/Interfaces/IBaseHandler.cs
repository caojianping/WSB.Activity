using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSB.Activity.WebHandler
{
    public interface IBaseHandler
    {
        string Uri { get; set; }
        WebClient Client { get; set; }
        T Deserialize<T>(string url);
    }
}
