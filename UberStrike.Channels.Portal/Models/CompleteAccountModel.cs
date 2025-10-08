using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace UberStrike.Channels.Portal.Models
{
    public class CompleteAccountModel
    {
        [Required(ErrorMessage = "Email required")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password verification required")]
        public string VerifyPassword { get; set; }
    }
}