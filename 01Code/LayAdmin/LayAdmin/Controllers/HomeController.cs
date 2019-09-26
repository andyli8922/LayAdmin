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
            string sessionID = (string)Session["CurrentSessionID"];
            string roleID = Session["CurrentRole"].ToString();
            ViewBag.Title = "LayAdmin1.0";
            using (var db=new LayAdminEntities())
            {
                List<string> RoleMenus = db.coreRoleMenu.Where<coreRoleMenu>(r => r.RoleID== roleID).Select<coreRoleMenu, string>(r => r.MenuID).ToList();
                List<coreMenu> menuList = db.coreMenu.Where<coreMenu>(r => RoleMenus.Contains(r.MenuID)).ToList();
                ViewBag.MenuStr = GetMenu(menuList, null);
                return View();
            }
        }
        public ActionResult welcome()
        {
            return View();
        }

        public ActionResult RoleMenu()
        {
            using (var db = new LayAdminEntities())
            {
                List<coreRole> roleList = db.coreRole.Where<coreRole>(r => 1 == 1).ToList();
                string option = "";
                foreach (coreRole item in roleList)
                {
                    option += string.Format("<option value = \"{0}\" > {1} </option >", item.RoleID, item.RoleDesc);
                }
                ViewBag.Roles = option;
                return View();
            }
        }
        
        [HttpPost]
        public string GetJson(string RoleID)
        {
            RoleMenu menus = new LayAdminCore.RoleMenu();
            List<string> RoleIDs = new List<string>();
            RoleIDs.Add(RoleID);
            return menus.GetMenuJson(RoleIDs);
        }
        public string SaveRole(string RoleID,List<string> Menus)
        {
            RoleMenu Role = new LayAdminCore.RoleMenu();
            return Role.SaveRole(RoleID, Menus);
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