using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web.Helpers;
using System.IO;
using System.Text;

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
        [HttpGet]
        public JsonResult GetList()
        {
            tableData data = new tableData();
            string url = Request.Url.ToString();
            int pageIndex =int.Parse(Request.Params.Get("page"));
            int pageCount = int.Parse(Request.Params.Get("limit"));
            data.code = "0";
            data.msg = "";
            data.count = "50";
            int BeginIndex = (pageIndex - 1) * pageCount + 1;
            int EndIndex = BeginIndex+pageCount;
            if (EndIndex>51)
            {
                EndIndex = 51;
            }

            List<tableData.rowData> rowDatas = new List<tableData.rowData>();
            for (int i=BeginIndex; i < EndIndex; i++)
            {
                tableData.rowData dd = new tableData.rowData();
                dd.id = i.ToString();
                dd.username = "andy" + i.ToString();
                rowDatas.Add(dd);
            }
            data.data = rowDatas;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult Delete(string id)
        {
            tableData data = new tableData();
            //StreamReader reader = new StreamReader(Request.GetBufferedInputStream(), Encoding.GetEncoding("UTF-8"));
            //string paramStr = reader.ReadToEnd();
                //Dim reader As New StreamReader(context.Request.GetBufferlessInputStream(), Encoding.GetEncoding("UTF-8"))
                //    Dim  As String = reader.ReadToEnd()
            data.code = "0";
            data.msg = "删除成功1";
            data.count = "50";
            data.url = "/Syssetting/Roles?partID=123";
            //List<tableData.rowData> rowDatas = new List<tableData.rowData>();
            //for (int i = 1; i < 51; i++)
            //{
            //    tableData.rowData dd = new tableData.rowData();
            //    dd.id = i.ToString();
            //    dd.username = "andy" + i.ToString();
            //    rowDatas.Add(dd);
            //}
            //data.data = rowDatas;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
    public class tableData
    {
        public string code { get; set; }
        public string msg { get; set; }
        public string url { get; set; }
        public string count { get; set; }
        public List<rowData> data { get; set; }

        public class rowData
        {
            public string id { get; set; }
            public string username { get; set; }
        }
    }
}