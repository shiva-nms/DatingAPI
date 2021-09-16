using DatingAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using DatingAPI.Helper;

namespace DatingAPI.Controllers
{
    public class DatingController : ApiController
    {

        [HttpGet]
        [ActionName("Index")]
        public HttpResponseMessage Index()
        {   
            var response = new HttpResponseMessage();
            response.Content = new StringContent("{\"Status\" : \"Running\"}");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;
        }


        [HttpPost]
        [ActionName("Create")]
        public HttpResponseMessage Create([FromUri] Request request)
        {
            var response = new HttpResponseMessage();
            try
            {
                int iUploadedCnt = 0;

                string sPath = ConfigurationManager.AppSettings["FileUploadLocation"]; 
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    if (hpf.ContentLength > 0)
                    {   
                        if (!File.Exists(sPath + Path.GetFileName(hpf.FileName)))
                        {
                            hpf.SaveAs(sPath + Path.GetFileName(hpf.FileName));
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                    }
                }
                response.Content = new StringContent("{\"Status\" : \"Files Uploaded Successfully\"}");
                
                User user = new User();
                user.Location = new Location { Latitude = request.Latitude, Longitude = request.Longitude };
                user.FullName = request.Name;
                user.Email = request.Email;
                user.Gender = request.Gender;


            }
            catch (Exception ex)
            {
                response.Content = new StringContent("{\"Status\" : \"Internal Error\"}");
            }
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;
        }

        [HttpPost]
        [ActionName("Match")]
        public HttpResponseMessage Match([FromUri] string Name, int distnaceinKM )
        {
            var response = new HttpResponseMessage();
            try
            {
                Util util = new Util();
                double distance = distnaceinKM * 1000.0;
                var userdat = File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/UserData.txt"));
                var Users = JsonConvert.DeserializeObject<List<User>>(userdat);
                var matchUser = Users.FirstOrDefault(x => x.FullName == Name);

                MatchModel matchModel = new MatchModel();

                var value = Users.Where(u => u.FullName != matchUser.FullName).
                                  Select(x => new { Distance = util.CalculateDistance(x.Location, matchUser.Location), x.FullName, x.Email , x.Gender}).ToList();

               var matchresult = value.Where(x => x.Gender != matchUser.Gender && x.Distance <= distance).ToList();

                if (matchresult.Count > 0)
                {
                    matchModel.Email = matchresult.Select(x => x.Email).ToList();
                    matchModel.Status = true;
                    matchModel.Message = "Match Found";
                }
                else
                {   
                    matchModel.Status = false;
                    matchModel.Message = "Match didn't Found";
                }

                response.Content = new StringContent(JsonConvert.SerializeObject(matchModel),System.Text.Encoding.UTF8, "application/json");
            }
            catch (Exception ex)
            {
                response.Content = new StringContent("{\"Status\" : \"Internal Error\"}");
            }
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;
        }



    }
}
