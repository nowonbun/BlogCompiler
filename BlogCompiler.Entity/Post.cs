//------------------------------------------------------------------------------
// <auto-generated>
//    このコードはテンプレートから生成されました。
//
//    このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//    このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

namespace BlogCompiler.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class Post
    {
        public int IDX { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string LOCATION { get; set; }
        public int CHANGEFREG { get; set; }
        public System.DateTime CREATEDATED { get; set; }
        public System.DateTime LAST_UPDATED { get; set; }
        public int PRIORITY { get; set; }
        public string TITLE { get; set; }
        public string FILEPATH { get; set; }
        public string GUID { get; set; }
        public byte[] IMAGE { get; set; }
        public string IMAGE_COMMENT { get; set; }
        public string SUMMARY { get; set; }
        public bool ISDELETED { get; set; }
    
        public virtual Category Category { get; set; }
    }
}
