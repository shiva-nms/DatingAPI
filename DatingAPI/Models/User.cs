using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatingAPI.Models
{
    public class User
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public Location Location { get; set; }

        public Double TempDistance { get; set; }
    }

 
}


 