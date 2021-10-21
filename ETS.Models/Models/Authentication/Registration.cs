using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS.Models.Models.Authentication
{
   public class Registration
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string address { get; set; }
        public string user_id { get; set; }

        [NotMapped]
        public string password { get; set; }

        [NotMapped]
        public string password_retype { get; set; }
    }
}
