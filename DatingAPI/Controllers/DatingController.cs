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
