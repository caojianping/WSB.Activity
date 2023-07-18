using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using WSB.Activity.EFModel;

namespace WSB.Activity.Mvc.Models
{
    public class LottoIndexViewModel
    {
        public int ChanceCount { get; set; }
        public int TotalNumber { get; set; }
        public List<View_UserLottoMap> LottoList { get; set; }
    }
}