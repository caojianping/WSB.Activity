using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WSB.Activity.EFModel;
using WSB.Activity.Interface;
using System.ComponentModel.DataAnnotations;

namespace WSB.Activity.Mvc.Models
{
    public class LoginForm
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }
    }

    public class RegisterForm
    {
        [Required]
        [Display(Name = "手机号码")]
        [StringLength(11, ErrorMessage = "手机号码格式错误")]
        public string Phone { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0}必须至少包含{2}个字符。", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }

    public class UserIndexViewModel
    {
        public User User { get; set; }
        public int Total { get; set; }
        public List<View_Redpackets> SendList { get; set; }
        public List<View_UserReceiveRedpackets> ActivatedList { get; set; }
        public List<View_UserReceiveRedpackets> UnActivatedList { get; set; }
    }

    public class RankViewModel
    {
        public User User { get; set; }
        public int RankNumber { get; set; }
        public PageResult<User> PageResult { get; set; }
    }
}