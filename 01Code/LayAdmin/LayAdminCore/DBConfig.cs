using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayAdminCore
{
   public static class DBConfig
    {
        private static string _DBConnStr = "";
        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        public static string DBConnString
        {
            get { return _DBConnStr;}
            set   { _DBConnStr = value;  }
        }
    }
}
