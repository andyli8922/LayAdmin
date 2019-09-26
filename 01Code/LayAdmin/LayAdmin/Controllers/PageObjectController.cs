using LayAdminModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Data.Entity.Infrastructure;
using System.Reflection.Emit;
using System.Reflection;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using LayAdminCore;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Data;

namespace LayAdmin.Controllers
{
    public class PageObjectController : MyController
    {
        private LayAdminEntities db = new LayAdminEntities();
        public ActionResult GridPart(int pageID)
        {            
            if (pageID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            coreDevPage devPage = db.coreDevPage.Find(pageID);
            if (devPage == null)
            {
                return HttpNotFound();
            }
            if (string.IsNullOrEmpty(devPage.pageQueryURL))
            {
                devPage.pageQueryURL = "/PageObject/GetTableData?pageID="+pageID.ToString();
            }
            List<coreToolBar> toolBars = db.coreToolBar.Where<coreToolBar>(r => r.toolbarID == devPage.pageToolBarID || r.toolbarID == devPage.pageRowBarID).ToList();
            if (devPage.pageToolBarID!=null && devPage.pageToolBarID != 0)
            {
                ViewBag.toolBar = toolBars.Find(r => r.toolbarID == devPage.pageToolBarID).toolbarContent;
            }
            if (devPage.pageRowBarID != null && devPage.pageRowBarID != 0)
            {
                ViewBag.rowBar = toolBars.Find(r => r.toolbarID == devPage.pageRowBarID).toolbarContent;
            }
            //获取toolBarJS
            List<coreToolBarEvent> EventList = db.coreToolBarEvent.Where<coreToolBarEvent>(r => r.toolBarID == devPage.pageToolBarID).ToList();
            string toolBarJS = string.Empty;
            foreach (var item in EventList)
            {
                item.EventJS = item.EventJS.Replace("@@pageID", devPage.pageID.ToString());
                toolBarJS += item.EventJS + "\r\n";
            }
            ViewBag.toolBarJS = toolBarJS;
            ViewBag.Total = devPage.pageColomns.Contains("totalRowText") ? "true" : "false";
            List<coreDevPageQuery> fieldsInfo = db.coreDevPageQuery.Where<coreDevPageQuery>(r => r.PAGE_ID == pageID).ToList();
            
            foreach (var item in fieldsInfo)
            {
                if (devPage.pageSearch.ToLower().Contains("@s" + item.PAGE_FIELD.ToLower()))
                {
                    //string str = "";
                    //DataTable dt= LayAdminCore.e
                }
            }
            return View(devPage);
        }
        public ActionResult FieldPart1()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageID"></param>
        /// <param name="type">1-添加，2-修改</param>
        /// <returns></returns>
        public ActionResult FieldPart(int pageID,int type,string key)
        {
            if (pageID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            coreDevPage devPage = db.coreDevPage.Find(pageID);
            if (devPage == null)
            {
                return HttpNotFound();
            }
            devPage.pageModifyURL += "&type=" + type.ToString();
            string whereStr = "";
            if (string.IsNullOrEmpty(key))
            {
                whereStr = devPage.pageWhereExpress;
            }
            else{
                whereStr = devPage.pageEditWhere;
            }
            if (type == 2)
            {
                devPage.pageEdit = devPage.pageEdit.ToLower();
                Dictionary<string, string> FieldValues = GridManager.GetFieldPart(devPage.pageDataSource, whereStr.ToLower(), key);
                foreach (var item in FieldValues)
                {
                    devPage.pageEdit = devPage.pageEdit.Replace("@@" + item.Key, item.Value);
                }
                while (devPage.pageEdit.Contains("@sqloption"))
                {
                    string str = devPage.pageEdit.Substring(devPage.pageEdit.IndexOf("@sqloption"));
                    string strOption = str.Substring(0,str.IndexOf(";")+1);
                    string OptionField=str.Substring(10, str.IndexOf("=") -10);
                    string strSql = str.Substring(str.IndexOf("=")+1, str.IndexOf(";") - str.IndexOf("="));
                    DataSet ds = SqlHelper.ExecuteDataset(DBConfig.DBConnString, CommandType.Text, strSql);
                    string option = "";
                    if (ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0)
                    {
                        foreach (DataRow item in ds.Tables[0].Rows)
                        {
                            if (FieldValues.Keys.Contains(OptionField))
                            {
                                if (item[0].ToString()==FieldValues[OptionField])
                                {
                                    option += string.Format("<option value = \"{0}\" selected> {1} </option >", item[0], item[1]);
                                    continue;
                                }
                            }
                            option += string.Format("<option value = \"{0}\" > {1} </option >", item[0], item[1]);
                        }
                    }
                    devPage.pageEdit= devPage.pageEdit.Replace(strOption, option);
                }
            }
            else {
                devPage.pageEdit = devPage.pageAdd;
            }
            return View(devPage);
        }


        /// <summary>
        /// 采用POST方式，因GET方式重载时有问题
        /// </summary>
        /// <param name="pageID"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public string GetTableData(int pageID, int page, int limit, string key)
        {
            string dataJson= GridManager.GetPageData(pageID, page, limit, key);
            return dataJson;
        }
        [HttpPost]
        public string Save(int pageID, int type,string key)
        {
            string modifyDataSource = string.Empty;
            using (var db = new LayAdminEntities())
            {
                coreDevPage devPage = db.coreDevPage.Find(pageID);
                modifyDataSource = string.IsNullOrEmpty(devPage.pageModifySource) ? devPage.pageDataSource : devPage.pageModifySource;
            }
            return GridManager.Save(modifyDataSource, type, key);
        }
        public string Delete(int pageID,string key)
        {
            if (key.IndexOf("[")!=0)
            {
                key = "[" + key + "]";
            }
            string modifyDataSource = string.Empty;
            using (var db = new LayAdminEntities())
            {
                coreDevPage devPage = db.coreDevPage.Find(pageID);
                modifyDataSource = string.IsNullOrEmpty(devPage.pageModifySource) ? devPage.pageDataSource : devPage.pageModifySource;
            }
            return GridManager.Delete(modifyDataSource, key);
        }
    }
}
