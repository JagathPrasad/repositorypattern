using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using QUONOW.Libraries;


namespace QUONOW.Models
{
    public class AuthenticationFilter : AuthorizationFilterAttribute
    {

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            Utility util = new Utility();
            var userExist = util.GetUserDetailsByToken(actionContext.Request.Headers.Authorization.ToString());
            if (userExist != null)
            {
                //actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
            }
            else
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
        }

    }
}