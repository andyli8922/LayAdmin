using System;
using System.Web.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Newtonsoft.Json.Linq;
using LayAdminCore;
using LayAdminModels;

namespace LayAdmin.Controllers
{
    public class SysSettingController : Controller
    {
        // GET: Syssetting
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Roles()
        {
            ViewBag.Title = "";
            return View();
        }
        public class tableData1
        {
            public int code
            {
                get { return 0; }
            }
            public string msg { get; set; }
            public int count { get; set; }
            public List<dynamic> data { get; set; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page">页索引</param>
        /// <param name="limit">每页记录数</param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public string GetList(int page, int limit, string key)
        {

            return GridManager.GetPageData(1, page, limit, key);
        }
        public JsonResult Delete(string id)
        {
            tableData data = new tableData();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public static tableData1 GetPageDate<T, TKey>(int pageIndex, int pageSize, Expression<Func<T, dynamic>> select, Expression<Func<T, bool>> where, Expression<Func<T, TKey>> order) where T : class
        {
            using (var db = new LayAdminEntities())
            {
                tableData1 data = new tableData1();
                data.count = db.Set<T>().Where(where).Count();
                data.data = db.Set<T>().Where(where).OrderByDescending(order).Select(select).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return data;
            }
        }
    }

    
}