using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AADB2C.WebClientMvc.Models
{
    public class OrderModel
    {
        public string OrderID { get; set; }
        [Display(Name = "Shipper")]
        public string ShipperName { get; set; }
        [Display(Name = "Shipper City")]
        public string ShipperCity { get; set; }
        public DateTimeOffset TS { get; set; }
    }
}