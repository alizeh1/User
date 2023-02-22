using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace User.Models
{
    public class UserDetails
    {
        public int UserDetailsId { get; set; }

        [Required]
        [DisplayName("Full Name")]
        public string FullName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [DisplayName("Mobile No")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Enter 10 Digit phone number")]

        public string MobileNo { get; set; }

        [Required]
        [DisplayName("Date Of Birth")]
        [DisplayFormat()]
        public DateTime DOB { get; set; }

        [Required]
        public string Address { get; set; }
        
        public int CityId { get; set; }

       
        [DisplayName("City Name")]
        public string CityName { get; set; }
        
        public int CountryId { get; set; }

       
        [DisplayName("Country Name")]
        public string CountryName { get; set; }
        
        public int StateId { get; set; }

       
        [DisplayName("State Name")]
        public string StateName { get; set; }

    }
}