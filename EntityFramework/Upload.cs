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
    
    public partial class Upload
    {
        public long ID { get; set; }
        public string Filename { get; set; }
        public int UploadLocationID { get; set; }
        public int Day { get; set; }
        public System.DateTime Time { get; set; }
        public int UploadedByCitizenID { get; set; }
    
        public virtual Citizen Citizen { get; set; }
        public virtual UploadLocation UploadLocation { get; set; }
    }
}