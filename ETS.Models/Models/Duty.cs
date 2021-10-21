using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS.Models.Models
{
     public class Duty
    {
        public int id { get; set; }
        public string task_name { get; set; }
        public string assign_to { get; set; }
        public string description { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }

    }
}
