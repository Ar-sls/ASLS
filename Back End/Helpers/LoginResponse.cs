using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApi.Helpers
{
    public class LoginResponse
    {
        public LoginResponse()
        {

            this.Token = "";
            this.responseMsg = new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.Unauthorized };
        }

        public string Token { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public HttpResponseMessage responseMsg { get; set; }
    }
}
