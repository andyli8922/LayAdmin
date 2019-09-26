using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using LayAdminModels;
using LayAdminCore;

namespace LayAdmin.Controllers
{
    public class MyController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (Session["CurrentSessionID"] == null)
            {
                filterContext.Result = Redirect("/Login/Login");//  没有返回值， 所以不是return   是filterContexr.Result    
            }
        }
    }
}