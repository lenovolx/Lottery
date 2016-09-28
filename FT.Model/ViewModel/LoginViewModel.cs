using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FT.Model.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "帐号")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }
    }
}