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
    public class HomeController : MyController
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Title = "LayAdmin1.0";
            using (var db=new LayAdminEntities())
            {
                List<coreMenu> menuList = db.coreMenu.Where<coreMenu>(r => 1 == 1).ToList();
                ViewBag.MenuStr = GetMenu(menuList, null);
                return View();
            }
        }

        public ActionResult Tree()
        {
            return View();
        }

        public string GetMenu(List<coreMenu> menuList,string MenuID)
        {
            StringBuilder MenuStr = new StringBuilder();
            foreach (var item in menuList.Where(a => a.ParentMenu == MenuID).ToList<coreMenu>().OrderBy(a => a.Sort))
            {
                MenuStr.Append("<li>");
                List<coreMenu> ChildtList = menuList.Where(a => a.ParentMenu == item.MenuID).ToList<coreMenu>();
                if (ChildtList.Count>0)
                {
                    MenuStr.Append("<a href = 'javascript:;'>");
                    MenuStr.Append("<i class='iconfont'>" + item.MenuIcon + "</i>");
                    MenuStr.Append("<cite>" + item.MenuDesc + "</cite>");
                    MenuStr.Append("<i class='iconfont nav_right'>&#xe697;</i>");
                    MenuStr.Append("</a>");
                    MenuStr.Append("<ul class='sub-menu'>");
                    MenuStr.Append(GetMenu(menuList, item.MenuID));
                    MenuStr.Append("</ul>");
                }
                else
                {
                    MenuStr.Append("<a _href = '"+ item .MenuHref+ "'>");
                    MenuStr.Append("<i class='iconfont'>&#xe6a7;</i>");
                    //if (string.IsNullOrEmpty(item.MenuIcon))
                    //{
                    //    MenuStr.Append("<i class='iconfont'>&#xe6a7;</i>");
                    //}
                    //else { 
                    //    MenuStr.Append("<i class='iconfont'>" + item.MenuIcon + "</i>");
                    //}
                    MenuStr.Append("<cite>"+ item.MenuDesc + "</cite>");
                    MenuStr.Append("</a>");
                }
                MenuStr.Append("</li>");
            }
            return MenuStr.ToString();
        }
    }
    public class Menu
    {
        /// <summary>
        /// 菜单编号
        /// </summary>
        public string MenuID{get;set;}
        /// <summary>
        /// 菜单描述
        /// </summary>
        public string MenuName { get; set; }
        /// <summary>
        /// 菜单链接
        /// </summary>
        public string MenuHref { get; set; }
        /// <summary>
        /// 菜单图标
        /// </summary>
        public string MenuIcon { get; set; }
        /// <summary>
        /// 菜单命令
        /// </summary>
        public string MenuCommand { get; set; }
        /// <summary>
        /// 上级菜单
        /// </summary>
        public string ParentMenu { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int Sort { get; set; }
    }

}