using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LayAdmin.Controllers
{
    public class PartObjctController : Controller
    {
        // GET: PartObjct
        public ActionResult GridPart()
        {
            return View();
        }

        public ActionResult FieldPart()
        {
            return View();
        }
    }

}