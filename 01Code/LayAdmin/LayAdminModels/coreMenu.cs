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
    
    public partial class coreMenu
    {
        public string MenuID { get; set; }
        public string MenuDesc { get; set; }
        public string MenuHref { get; set; }
        public string MenuIcon { get; set; }
        public string MenuCommand { get; set; }
        public string ParentMenu { get; set; }
        public Nullable<int> Sort { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string ModifyUser { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
    }
}
