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
        Util Util = null;
        public DatingController()
        {
            string AppPath = HttpContext.Current.Server.MapPath("~/App_Data/");
            Util = new Util(AppPath);
        }

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
                User user = new User();
                user.ID = Guid.NewGuid();

                string sPath = ConfigurationManager.AppSettings["FileUploadLocation"]; 
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                if (hfc.Count == 1)
                {
                    for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                    {
                        System.Web.HttpPostedFile hpf = hfc[iCnt];
                        if (hpf.ContentLength > 0)
                        {
                            if (!File.Exists(sPath + Path.GetFileName(hpf.FileName)))
                            {
                                hpf.SaveAs(sPath + Path.GetFileName(hpf.FileName));
                                iUploadedCnt = iUploadedCnt + 1;
                                user.Filename = hpf.FileName;
                            }
                            else
                            {
                                var extension = Path.GetExtension(hpf.FileName);
                                var filename = Path.GetFileName(user.ID.ToString() + $"{extension}");
                                hpf.SaveAs(sPath + filename);
                                iUploadedCnt = iUploadedCnt + 1;
                                user.Filename = filename;
                            }
                        }
                    }
                    response.Content = new StringContent("{\"Status\" : \"Files Uploaded Successfully\"}");

                    user.Location = new Location { Latitude = request.Latitude, Longitude = request.Longitude };
                    user.FullName = request.Name;
                    user.Email = request.Email;
                    user.Gender = request.Gender;


                    var path = HttpContext.Current.Server.MapPath($"~/App_Data/{user.ID}.txt");
                    var filevalue = JsonConvert.SerializeObject(user);
                    File.WriteAllText(path, filevalue);
                }
                else 
                {
                    response.Content = new StringContent("{\"Status\" : \"Upload Single File\"}");
                }
            }
            catch (Exception ex)
            {
                response.Content = new StringContent("{\"Status\" : \"Internal Error\"}");
            }
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;
        }


        //User matches
        [HttpPost]
        [ActionName("Match")]
        public HttpResponseMessage Match([FromUri] string MatchName)
        {
            var response = new HttpResponseMessage();
            try
            {
                MatchModel matchModel = new MatchModel();
                var AllUsers = Util.LoadUser();
                var matchUser = AllUsers.FirstOrDefault(x => x.FullName == MatchName);

                
                var qualidefuser = AllUsers.Where(x => !string.IsNullOrEmpty(x.FullName)
                                                    && !string.IsNullOrEmpty(x.Email)
                                                    && !string.IsNullOrEmpty(x.Gender) && x.Gender != matchUser.Gender
                                                    && !string.IsNullOrEmpty(x.Filename)
                                                    && x.Location != null
                                                    && x.IntrestedUser.Contains(matchUser.ID)). //only if both user has intrested in each oether and both user has all data match
                                            Select(X => X.Email).ToList();

                if (qualidefuser.Count > 0)
                {
                    matchModel.Email = qualidefuser;
                    matchModel.Status = true;
                    matchModel.Message = "Match Found";
                }
                else
                {
                    matchModel.Status = false;
                    matchModel.Message = "Match didn't Found";
                }

            }
            catch (Exception ex)
            {
                response.Content = new StringContent("{\"Status\" : \"Internal Error\"}");
            }
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;
        }


        [HttpPost]
        [ActionName("List")]
        public HttpResponseMessage List([FromUri] string UserName, string gender, int distnaceinKM )
        {
            var response = new HttpResponseMessage();
            try
            {   
                List<User> Users = Util.LoadUser();
                double distance = distnaceinKM * 1000.0;
               
                var matchUser = Users.FirstOrDefault(x => x.FullName == UserName);
                MatchModel matchModel = new MatchModel();

                var value = Users.Where(u => u.FullName != matchUser.FullName).
                                  Select(x => new { Distance = Util.CalculateDistance(x.Location, matchUser.Location), x.FullName, x.Email , x.Gender}).ToList();

               var matchresult = value.Where(x => x.Gender != matchUser.Gender && x.Distance <= distance).ToList();

                if (matchresult.Count > 0)
                {
                    matchModel.Email = matchresult.Select(x => x.Email).ToList();
                    matchModel.Status = true;
                    matchModel.Message = "List Found";
                }
                else
                {   
                    matchModel.Status = false;
                    matchModel.Message = "List didn't Found";
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
