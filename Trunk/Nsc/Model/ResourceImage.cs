//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nsc.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class ResourceImage
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FileURL { get; set; }
        public string FileDirPath { get; set; }
        public Nullable<int> UploadedBy { get; set; }
        public string ImageType { get; set; }
        public Nullable<int> OwnerId { get; set; }
        public Nullable<System.DateTime> CreatedUtcDate { get; set; }
        public Nullable<System.DateTime> UpdatedUtcDate { get; set; }
        public Nullable<bool> IsProfilePic { get; set; }
    
        public virtual UserAccount UserAccount { get; set; }
    }
}
