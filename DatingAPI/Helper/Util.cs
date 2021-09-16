
using DatingAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DatingAPI.Helper
{
    public class Util

    {
        string localpath;
        public Util( string path)
        {
            localpath = path;
        }
        public double CalculateDistance(Location point1, Location point2)
        {
            var d1 = point1.Latitude * (Math.PI / 180.0);
            var num1 = point1.Longitude * (Math.PI / 180.0);
            var d2 = point2.Latitude * (Math.PI / 180.0);
            var num2 = point2.Longitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }


        public List<User> LoadUser()
        {
            List<User> result = new List<User>();
            var listOfUSer = Directory.GetFiles(localpath);
            foreach (var user in listOfUSer)
            {
                var userdat = File.ReadAllText(user);
                result.Add(JsonConvert.DeserializeObject<User>(userdat));
            }
            return result;
        }
    }
}