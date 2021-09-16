﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatingAPI.Models
{
    public class Request
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}