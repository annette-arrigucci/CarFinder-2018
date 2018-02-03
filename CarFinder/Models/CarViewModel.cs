using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarFinder.Models
{
    public class CarViewModel
    {
        public Car Car { get; set; }
        public dynamic Recalls { get; set; }
        public string Image { get; set; }
    }
}