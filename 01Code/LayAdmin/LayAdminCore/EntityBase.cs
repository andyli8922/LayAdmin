using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayAdminCore
{
    public class EntityBase
    {
        private string _Source = string.Empty;
        private Dictionary<string, object> _Items;
        public EntityBase(string Source)
        {
            _Source = Source;
            //取表或试图对应的字段
            DataTable table = SqlHelper.ExecuteDataset(DBConfig.DBConnString, CommandType.Text, "select col.name  from sys.columns col where object_id in(object_id('" + Source + "'))").Tables[0];
            if (table ==null || table.Rows.Count<=0)
            {
                throw new Exception(string.Format("不存在表或试图{0}", Source));
            }
            foreach (var item in table.Rows)
            {

            }
        }
    }
}
