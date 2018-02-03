using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarFinder.Models
{
    public class Car
    {
        public int id { get; set; }

        public string make { get; set; }

        public string model_name { get; set; }

        public string model_trim { get; set; }

        public string model_year { get; set; }

        public string body_style { get; set; }

        public string engine_position { get; set; }

        public string engine_cc { get; set; }

        public string engine_num_cyl { get; set; }

        public string engine_type { get; set; }

        public string engine_valves_per_cyl { get; set; }

        public string engine_power_ps { get; set; }

        public string engine_power_rpm { get; set; }

        public string engine_torque_nm { get; set; }

        public string engine_torque_rpm { get; set; }

        public string engine_bore_mm { get; set; }

        public string engine_stroke_mm { get; set; }

        public string engine_compression { get; set; }

        public string engine_fuel { get; set; }

        public string top_speed_kph { get; set; }

        public string zero_to_100_kph { get; set; }

        public string drive_type { get; set; }

        public string transmission_type { get; set; }

        public string seats { get; set; }

        public string doors { get; set; }

        public string weight_kg { get; set; }

        public string length_mm { get; set; }

        public string width_mm { get; set; }

        public string height_mm { get; set; }

        public string wheelbase { get; set; }

        public string lkm_hwy { get; set; }

        public string lkm_mixed { get; set; }

        public string lkm_city { get; set; }

        public string fuel_capacity_l { get; set; }

        public string sold_in_us { get; set; }

        public string co2 { get; set; }

        public string make_display { get; set; }

    }
}