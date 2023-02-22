using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace User.Models
{
    public class SignIn
    {

        //[DataType(DataType.EmailAddress,ErrorMessage = "Please enter your email address")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Please enter the password")]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool Rememberme { get; set; }

    }
}