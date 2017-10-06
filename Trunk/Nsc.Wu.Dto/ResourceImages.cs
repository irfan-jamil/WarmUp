using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.Dto
{
    public partial class ResourceImages
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FileURL { get; set; }
        public string Base64Data { get; set; }
        public string FileDirPath { get; set; }
        public int UploadedBy { get; set; }
        public string ImageType { get; set; }
        public int OwnerId { get; set; }
        public bool IsProfilePic { get; set; }

    }
}
