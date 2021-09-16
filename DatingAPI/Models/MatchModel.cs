using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatingAPI.Models
{
    public class MatchModel
    {
        public bool Status { get; set; }
        public string Message { get; set; }

        public List<string> Email { get; set; }
    }
}