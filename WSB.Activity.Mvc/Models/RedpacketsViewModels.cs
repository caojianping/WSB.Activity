using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using WSB.Activity.EFModel;

namespace WSB.Activity.Mvc.Models
{
    public class RedpacketsSendViewModel
    {
        public int ChanceCount { get; set; }
        public Redpackets OldRedpackets { get; set; }
        public Redpackets NewRedpackets { get; set; }
    }

    public class SendForm
    {
        [Required]
        public int userId { get; set; }
        //public string verifyCode { get; set; }
    }

    public class RedpacketsDetailViewModel
    {
        public View_Redpackets Redpackets { get; set; }
        public List<View_UserRedpacketsMap> ReceiveList { get; set; }
        public ReceiveForm ReceiveForm { get; set; }
    }

    public class RedpacketsReceiveViewModel
    {
        public int RedpacketsId { get; set; }
        public UserRedpacketsMap UserRedpacketsMap { get; set; }
        public List<View_UserRedpacketsMap> ReceiveList { get; set; }
    }

    public class ReceiveForm
    {
        [Required]
        public int Id { get; set; }
        //public string Phone { get; set; }
    }
}