using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Edura.WebUI.Models
{
    public class OrderDetails
    {
        [Required(ErrorMessage ="Please enter the necessary fields")]
        public string AddressTitle { get; set; }

        [Required(ErrorMessage = "Please enter the necessary fields")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please enter the necessary fields")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter the necessary fields")]
        public string District { get; set; }

        [Required(ErrorMessage = "Please enter the necessary fields")]
        public string Telephone { get; set; }

    }
}
