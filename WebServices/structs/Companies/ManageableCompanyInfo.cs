using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Companies
{
    public class ManageableCompanyInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public int ProductID { get; set; }
        public int UnreadWarnings { get; set; }
        public int UnreadMessages { get; set; }

    }
}
