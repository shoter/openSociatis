//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class ArticleComment
    {
        public int ID { get; set; }
        public int ArticleID { get; set; }
        public int AuthorID { get; set; }
        public string Content { get; set; }
        public System.DateTime CreationDate { get; set; }
        public int CreationDay { get; set; }
    
        public virtual Article Article { get; set; }
        public virtual Entity Author { get; set; }
    }
}