using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace LayAdmin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Title = "LayAdmin1.0";
            List<Menu> menuList = new List<Menu>();
            Menu MenuItem = new Menu();
            MenuItem.MenuID = "1";
            MenuItem.MenuName = "系统设置";
            //MenuItem.MenuHref = "Views/Syssetting/Icon.html";
            MenuItem.MenuIcon = "&#xe6ae;";
            MenuItem.Sort = 0;
            menuList.Add(MenuItem);
            
            Menu MenuItem2 = new Menu();
            MenuItem2.MenuID = "3";
            MenuItem2.MenuName = "系统图标2";
            MenuItem2.MenuHref = "../unicode.html";
            MenuItem2.MenuIcon = "&#xe696;";
            MenuItem2.ParentMenu = "1";
            MenuItem2.Sort = 2;
            menuList.Add(MenuItem2);
            Menu MenuItem5 = new Menu();
            MenuItem5.MenuID = "6";
            MenuItem5.MenuName = "系统图标1";
            MenuItem5.MenuHref = "../unicode.html";
            MenuItem5.MenuIcon = "&#xe696;";
            MenuItem5.ParentMenu = "1";
            MenuItem5.Sort = 1;
            menuList.Add(MenuItem5);

            Menu MenuItem1 = new Menu();
            MenuItem1.MenuID = "2";
            MenuItem1.MenuName = "角色设置";
            MenuItem1.MenuHref = "/Syssetting/Roles?partID=123";
            MenuItem1.MenuIcon = "&#xe696;";
            menuList.Add(MenuItem1);

            Menu MenuItem3 = new Menu();
            MenuItem3.MenuID = "2";
            MenuItem3.MenuName = "FieldPart";
            MenuItem3.MenuHref = "/PartObjct/FieldPart";
            MenuItem3.MenuIcon = "&#xe696;";
            menuList.Add(MenuItem3);

            ViewBag.MenuStr = GetMenu(menuList,null);
            return  View();
        }

        public string GetMenu(List<Menu> menuList,string MenuID)
        {
            StringBuilder MenuStr = new StringBuilder();
            foreach (var item in menuList.Where(a => a.ParentMenu == MenuID).ToList<Menu>().OrderBy(a => a.Sort))
            {
                MenuStr.Append("<li>");
                List<Menu> ChildtList = menuList.Where(a => a.ParentMenu == item.MenuID).ToList<Menu>();
                if (ChildtList.Count>0)
                {
                    MenuStr.Append("<a href = 'javascript:;'>");
                    MenuStr.Append("<i class='iconfont'>" + item.MenuIcon + "</i>");
                    MenuStr.Append("<cite>" + item.MenuName + "</cite>");
                    MenuStr.Append("<i class='iconfont nav_right'>&#xe697;</i>");
                    MenuStr.Append("</a>");
                    MenuStr.Append("<ul class='sub-menu'>");
                    MenuStr.Append(GetMenu(menuList, item.MenuID));
                    MenuStr.Append("</ul>");
                }
                else
                {
                    MenuStr.Append("<a _href = '"+ item .MenuHref+ "'>");
                    MenuStr.Append("<i class='iconfont'>" + item.MenuIcon + "</i>");
                    MenuStr.Append("<cite>"+ item.MenuName + "</cite>");
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