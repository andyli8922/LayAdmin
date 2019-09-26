using LayAdminCore;
using LayAdminModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LayAdmin.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public string UserLogin(string userid, string password)
        {
            JsonData data = new JsonData();
            using (var db = new LayAdminEntities())
            {
                List<coreUser> user = db.coreUser.Where<coreUser>(r => r.USER_ID==userid && r.USER_PASSWORD==password).ToList();
                if (user.Count <= 0)
                {
                    data.code = 1;
                    data.msg = "用户名或密码错误";
                }
                else
                {
                    Session["CurrentUserID"] = userid.ToUpper();
                    Session["CurrentRole"] = user[0].USER_ROLE;
                    Session["CurrentSessionID"] = Session.SessionID;
                    //Redirect("/Home/Index");
                }
                return JsonConvert.SerializeObject(data);
            }

        }
    }
}