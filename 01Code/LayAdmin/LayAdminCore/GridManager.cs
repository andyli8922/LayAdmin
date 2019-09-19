using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayAdminModels;
using Newtonsoft.Json.Converters;
using System.Reflection;

namespace LayAdminCore
{
    public class GridManager
    {
        /// <summary>
        /// 获取table数据
        /// </summary>
        /// <param name="PageID">页面编号</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="limit">每页记录数</param>
        /// <param name="key">查询条件(Json)</param>
        /// <returns></returns>
        public static string GetPageData(int PageID, int pageIndex, int limit, string key)
        {
            tableData data = new tableData();
            string whereStr = "1=1";
            string fields = "*";
            string orderBy = "";
            try
            {
                List<SqlParameter> sqlParams = new List<SqlParameter>();
                using (var db = new LayAdminEntities())
                {
                    coreDevPage devPage = db.coreDevPage.Find(PageID);
                    if (devPage == null)
                    {
                        throw new Exception("页面编号不正确");
                    }
                    //取表或试图对应的字段
                    DataTable table = SqlHelper.ExecuteDataset(DBConfig.DBConnString, CommandType.Text, "select lower(col.name) as FieldName, tp.name FieldType from sys.columns col left join dbo.systypes tp on col.user_type_id = tp.xusertype where object_id in(object_id('" + devPage.pageDataSource + "'))").Tables[0];
                    if (table == null || table.Rows.Count <= 0)
                    {
                        throw new Exception("表或试图不存在字段");
                    }
                    //拼接where条件
                    if (string.IsNullOrEmpty(devPage.pageWhereExpress)==false)
                    {
                        whereStr += " and " + devPage.pageWhereExpress;
                    }
                    if (key != null)
                    {
                        JObject jObj = JsonConvert.DeserializeObject(key) as JObject;
                        foreach (var item in jObj)
                        {
                            if (string.IsNullOrEmpty(item.Value.ToString()) == false)
                            {
                                var d = from r in table.AsEnumerable()
                                        where r.Field<string>("FieldName") == item.Key.ToString().ToLower()
                                        select r.Field<string>("FieldType");
                                if (d.FirstOrDefault().ToString() == "datetime")
                                {
                                    whereStr += string.Format(" and {0}>=@{1}", item.Key.ToString(),"BEGIN"+ item.Key.ToString());
                                    whereStr += string.Format(" and {0}<@{1}", item.Key.ToString(), "END" + item.Key.ToString());
                                    SqlParameter param = new SqlParameter();
                                    param.ParameterName = "@" + "BEGIN" + item.Key.ToString();
                                    param.SqlDbType = GetSqlDbType(d.FirstOrDefault().ToString());
                                    param.Value = item.Value.ToString();
                                    sqlParams.Add(param);
                                    SqlParameter param1 = new SqlParameter();
                                    param1.ParameterName = "@" + "END" + item.Key.ToString();
                                    param1.SqlDbType = GetSqlDbType(d.FirstOrDefault().ToString());
                                    param1.Value =DateTime.Parse( item.Value.ToString()).AddDays(1);
                                    sqlParams.Add(param1);
                                }
                                else
                                {
                                    whereStr += string.Format(" and {0}=@{1}", item.Key.ToString(), item.Key.ToString());
                                    SqlParameter param = new SqlParameter();
                                    param.ParameterName = "@" +item.Key.ToString();
                                    param.SqlDbType = GetSqlDbType(d.FirstOrDefault().ToString());
                                    param.Value = item.Value.ToString();
                                    sqlParams.Add(param);
                                }
                            }
                        }
                    }
                    //获取查询字段
                    if (string.IsNullOrEmpty(devPage.pageQueryFields) == false)
                    {
                        fields = devPage.pageQueryFields;
                    }
                    //获取排序字段，若无排序，则取表的第一个字段倒序
                    if (string.IsNullOrEmpty(devPage.pageOrderBy) == false)
                    {
                        orderBy = devPage.pageOrderBy;
                    }
                    else
                    {
                        orderBy = table.Rows[0][0].ToString() + " desc";
                    }
                    data.count = (int)SqlHelper.ExecuteScalar(DBConfig.DBConnString, CommandType.Text, "select count(1) from " + devPage.pageDataSource + " where  " + whereStr + "", sqlParams.ToArray());
                    DataSet ds = SqlHelper.ExecuteDataset(DBConfig.DBConnString, CommandType.Text, string.Format("SELECT " + fields + " FROM  " + devPage.pageDataSource + " where  " + whereStr + " ORDER BY " + orderBy + " OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", (pageIndex - 1) * limit, limit==0? data.count : limit), sqlParams.ToArray());
                    data.data = ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                data.code = 1;
                data.msg = ex.Message;
            }
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            return JsonConvert.SerializeObject(data,timeFormat);
        }
        public static string Delete(string DataSource, string keys)
        {
            tableData data = new tableData();
            string whereStr = "1=0";
            try
            {
                List<SqlParameter> sqlParams = new List<SqlParameter>();
                DataTable table = SqlHelper.ExecuteDataset(DBConfig.DBConnString, CommandType.Text, "select lower(field) as field,fieldType from v_coreTableInfo where isPK=1 and tableName='" + DataSource + "'").Tables[0];
                if (table == null || table.Rows.Count <= 0)
                {
                    throw new Exception("删除失败：未找到主键");
                }
                int i = 0; //参数索引
                JArray rows = JsonConvert.DeserializeObject(keys) as JArray;
                foreach (JObject item in rows)
                {
                    whereStr += " OR ( 1=1";
                    foreach (DataRow row in table.Rows)
                    {
                        whereStr += string.Format(" AND {0}=@{1}", row["field"].ToString(), i.ToString());
                        var d = from r in table.AsEnumerable()
                                where r.Field<string>("field") == row["field"].ToString()
                                select r.Field<string>("fieldType");

                        foreach (var jsonItem in item)
                        {
                            if (jsonItem.Key.ToLower()== row["field"].ToString())
                            {
                                SqlParameter param = new SqlParameter();
                                param.ParameterName = "@" + i.ToString();
                                param.SqlDbType = GetSqlDbType(d.FirstOrDefault().ToString());
                                param.Value = jsonItem.Value;
                                sqlParams.Add(param);
                                i++;
                            }
                        }
                    }
                    whereStr += ")";
                }
                int successCount = SqlHelper.ExecuteNonQuery(DBConfig.DBConnString, CommandType.Text, string.Format("delete " + DataSource + " where  " + whereStr), sqlParams.ToArray());
                if (successCount > 0)
                {
                    data.msg = string.Format("删除成功，共删除{0}条", successCount);
                }
                else
                {
                    data.msg = string.Format("删除失败", successCount);
                }
            }
            catch (Exception ex)
            {
                data.msg = ex.Message;
            }
            return JsonConvert.SerializeObject(data);
        }

        public static string Save(string DataSource,int type, string key)
        {
            tableData data = new tableData();
            string SaveSql = "";
            try
            {
                List<SqlParameter> sqlParams = new List<SqlParameter>();
                //新增
                if (type == 1)
                {
                    List<string> insertFields = new List<string>();
                    List<string> valueFields = new List<string>();
                    DataTable table = SqlHelper.ExecuteDataset(DBConfig.DBConnString, CommandType.Text, "select lower(field) as field,fieldType,IsIdentity,isPK from v_coreTableInfo where tableName='" + DataSource + "'").Tables[0];
                    int i = 0; //参数索引
                    JObject json = JsonConvert.DeserializeObject(key) as JObject;
                    foreach (var item in json)
                    {
                        IEnumerable<DataRow> rows = from g in table.AsEnumerable()
                                                    where g.Field<string>("field") == item.Key.ToLower()
                                                    select g;

                        if (rows.FirstOrDefault<DataRow>()["IsIdentity"].ToString() != "1")
                        {
                            SqlParameter param = new SqlParameter();
                            param.ParameterName = "@" + i.ToString();
                            param.SqlDbType = GetSqlDbType(rows.FirstOrDefault<DataRow>()["fieldType"].ToString());
                            param.Value = item.Value.ToString();
                            sqlParams.Add(param);
                            insertFields.Add(item.Key);
                            valueFields.Add("@" + i.ToString());
                            i++;
                        }
                    }
                    //增加创建和修改信息
                    if (table.AsEnumerable().Where(r=>(r.Field<string>("field")=="createdate")).Count()>0)
                    {
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@" + i.ToString();
                        param.SqlDbType = SqlDbType.DateTime;
                        param.Value = System.DateTime.Now;
                        sqlParams.Add(param);
                        insertFields.Add("createdate");
                        valueFields.Add("@" + i.ToString());
                        i++;
                    }
                    if (table.AsEnumerable().Where(r => (r.Field<string>("field") == "modifydate")).Count() > 0)
                    {
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@" + i.ToString();
                        param.SqlDbType = SqlDbType.DateTime;
                        param.Value = System.DateTime.Now;
                        sqlParams.Add(param);
                        insertFields.Add("ModifyDate");
                        valueFields.Add("@" + i.ToString());
                        i++;
                    }
                    if (table.AsEnumerable().Where(r => (r.Field<string>("field") == "createuser")).Count() > 0)
                    {
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@" + i.ToString();
                        param.SqlDbType = SqlDbType.NVarChar;
                        param.Value = "ANDY";
                        sqlParams.Add(param);
                        insertFields.Add("createuser");
                        valueFields.Add("@" + i.ToString());
                        i++;
                    }
                    if (table.AsEnumerable().Where(r => (r.Field<string>("field") == "modifyuser")).Count() > 0)
                    {
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@" + i.ToString();
                        param.SqlDbType = SqlDbType.NVarChar;
                        param.Value = "ANDY";
                        sqlParams.Add(param);
                        insertFields.Add("modifyuser");
                        valueFields.Add("@" + i.ToString());
                        i++;
                    }
                    SaveSql = "insert into " + DataSource + " (" + string.Join(",", insertFields) + ") values  (" + string.Join(",", valueFields) + ")";
                }
                else
                {
                    List<string> UpdateFields = new List<string>();
                    string whereStr = "1=1";
                    //更新
                    DataTable table = SqlHelper.ExecuteDataset(DBConfig.DBConnString, CommandType.Text, "select lower(field) as field,fieldType,IsIdentity,isPK from v_coreTableInfo where tableName='" + DataSource + "'").Tables[0];
                    int i = 0; //参数索引
                    JObject json = JsonConvert.DeserializeObject(key) as JObject;
                    foreach (var item in json)
                    {
                        if (item.Key == "createuser" || item.Key == "createdate"||item.Key== "modifyuser" || item.Key== "modifydate")
                        {
                            continue;
                        }
                        IEnumerable<DataRow> rows = from g in table.AsEnumerable()
                                                    where g.Field<string>("field") ==item.Key.ToLower()
                                                    select g;

                        if (rows == null || rows.Count()<=0)
                        {
                            break;
                        }
                        if (rows.FirstOrDefault<DataRow>()["isPK"].ToString() != "1")
                        {
                            UpdateFields.Add(item.Key + "=" + "@" + i.ToString());
                        }
                        else
                        {
                            whereStr += " and " + item.Key + "=" + "@" + i.ToString();
                        }
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@" + i.ToString();
                        param.SqlDbType = GetSqlDbType(rows.FirstOrDefault<DataRow>()["fieldType"].ToString());
                        param.Value = item.Value.ToString();
                        sqlParams.Add(param);
                        i++;
                    }
                    if (table.AsEnumerable().Where(r => (r.Field<string>("field") == "modifydate")).Count() > 0)
                    {
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@" + i.ToString();
                        param.SqlDbType =SqlDbType.DateTime;
                        param.Value = System.DateTime.Now;
                        sqlParams.Add(param);
                        UpdateFields.Add("modifydate=" + "@" + i.ToString());
                        i++;
                    }
                    if (table.AsEnumerable().Where(r => (r.Field<string>("field") == "modifyuser")).Count() > 0)
                    {
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@" + i.ToString();
                        param.SqlDbType =SqlDbType.NVarChar;
                        param.Value = "ANDY";
                        sqlParams.Add(param);
                        UpdateFields.Add("modifyuser=" + "@" + i.ToString());
                        i++;
                    }
                    SaveSql = "update " + DataSource + " set " + string.Join(",", UpdateFields) + " where " + whereStr;
                }
                

                int successCount = SqlHelper.ExecuteNonQuery(DBConfig.DBConnString, CommandType.Text, SaveSql, sqlParams.ToArray());
                if (successCount > 0)
                {
                    data.msg = string.Format("保存成功");
                }
                else
                {
                    data.msg = string.Format("保存失败");
                }
            }
            catch (Exception ex)
            {
                data.msg = ex.Message;
            }
            return JsonConvert.SerializeObject(data);
        }
        public static Dictionary<string,string> GetFieldPart(string DataSource,string whereStr,string key)
        {
            Dictionary<string, string> EntityData = new Dictionary<string, string>();
            try
            {
                List<SqlParameter> sqlParams = new List<SqlParameter>();
                if (string.IsNullOrEmpty(key)==false)
                {
                    JObject json = JsonConvert.DeserializeObject(key) as JObject;
                    foreach (var item in json)
                    {
                        whereStr = whereStr.Replace("@@" + item.Key.ToLower(), item.Value.ToString());
                    }
                }
                DataTable partTable = SqlHelper.ExecuteDataset(DBConfig.DBConnString, CommandType.Text, string.Format("select top(1) * from " + DataSource + " where  " + whereStr)).Tables[0];
                foreach (DataColumn col in partTable.Columns)
                {
                    EntityData.Add(col.ColumnName.ToLower(), partTable.Rows[0][col.ColumnName].ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return EntityData;
        }
        /// <summary>
        /// 数据库类型描述
        /// </summary>
        /// <param name="value"></param>
        public static SqlDbType GetSqlDbType(string value)
        {
            switch (value)
            {
                case "int":
                    return SqlDbType.Int;
                case "datetime":
                    return SqlDbType.DateTime;
                case "nvarchar":
                    return SqlDbType.NVarChar;
                case "decimal":
                    return SqlDbType.Decimal;
                default:
                    return SqlDbType.NText;
            }
        }
    }
    public class tableData
    {
        private int _code=0;
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
        public int count { get; set; }
        public DataTable data { get; set; }
    }
}
