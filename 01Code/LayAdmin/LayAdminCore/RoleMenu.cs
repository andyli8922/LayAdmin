using LayAdminModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayAdminCore
{
    public class RoleMenu
    {
        List<MenuData> Menus = new List<MenuData>();
        /// <summary>
        /// 根据角色ID获取菜单
        /// </summary>
        /// <param name="RoleIDs"></param>
        public string GetMenuJson(List<string> RoleIDs)
        {
            using (var db = new LayAdminEntities())
            {
                JsonData data = new JsonData();
                data.code = 0;
                try
                {
                    List<string> RoleMenus = db.coreRoleMenu.Where<coreRoleMenu>(r => RoleIDs.Contains(r.RoleID)).Select<coreRoleMenu, string>(r => r.MenuID).ToList();
                    List<coreMenu> menuList = db.coreMenu.Where<coreMenu>(r =>1==1).ToList();
                    data.data = GetMenu(menuList, RoleMenus, null);
                }
                catch (Exception ex)
                {
                    data.msg = ex.Message;
                }
                return JsonConvert.SerializeObject(data);
            }
        }
        public List<MenuData> GetMenu(List<coreMenu> menuList, List<string> RoleMenus, string MenuID)
        {
            List<MenuData> children = new List<MenuData>();
            foreach (var item in menuList.Where(a => a.ParentMenu == MenuID).ToList<coreMenu>().OrderBy(a => a.Sort))
            {
                MenuData data = new MenuData();
                List<coreMenu> ChildtList = menuList.Where(a => a.ParentMenu == item.MenuID).ToList<coreMenu>();
                if (ChildtList.Count > 0)
                {
                    data.Children = GetMenu(menuList, RoleMenus, item.MenuID);
                }
                else 
                {
                    if (RoleMenus.Contains(item.MenuID))
                    {
                        data.isCheck = true;
                    }
                }
                data.id = item.MenuID;
                data.MenuDesc = item.MenuDesc;
                
                children.Add(data);
            }
            return children;
        }

        /// <summary>
        /// 保存角色权限
        /// </summary>
        /// <returns></returns>
        public string SaveRole(string RoleID, List<string> Menus)
        {
            JsonData data = new JsonData();
            SqlConnection con = new SqlConnection(DBConfig.DBConnString);
            con.Open();
            SqlTransaction trans = con.BeginTransaction();
            try
            {
                int successCount = SqlHelper.ExecuteNonQuery(trans, CommandType.Text, string.Format("delete coreRoleMenu where roleid='{0}'",RoleID));
                foreach (var item in Menus)
                {
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, string.Format("insert into coreRoleMenu values('{0}','{1}','{2}','{3}','{4}','{5}')", RoleID, item, "andy", DateTime.Now, "andy", DateTime.Now));
                }
                data.msg = string.Format("保存成功");
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                data.msg = ex.Message;
            }
            finally
            {
                con.Close();
            }
            return JsonConvert.SerializeObject(data);
        }
    }
    public class JsonData
    {
        private int _code = 0;
        public int code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }
        public string msg { get; set; }
        public List<MenuData> data { get; set; }
    }
    public class MenuData
    {
        /// <summary>
        /// 菜单编号
        /// </summary>
        public string id;
        /// <summary>
        /// 菜单描述
        /// </summary>
        public string MenuDesc;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool isCheck = false;
        /// <summary>
        /// 子菜单
        /// </summary>
        public List<MenuData> Children =new List<MenuData>();
    }
}
