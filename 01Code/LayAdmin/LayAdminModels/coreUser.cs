//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace LayAdminModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class coreUser
    {
        public string USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public string USER_PASSWORD { get; set; }
        public string USER_ROLE { get; set; }
        public string USER_PHONE { get; set; }
        public string USER_ADDRESS { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string ModifyUser { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
    }
}