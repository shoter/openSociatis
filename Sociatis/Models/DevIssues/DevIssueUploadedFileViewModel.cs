using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.DevIssues
{
    public class DevIssueUploadedFileViewModel
    {
        public string OriginalFilename { get; set; }
        public long ID { get; set; }

        public DevIssueUploadedFileViewModel(DevIssueUploadedFile uploadedFile)
        {
            OriginalFilename = uploadedFile.OriginalFilename;
            ID = uploadedFile.ID;
        }
    }
}