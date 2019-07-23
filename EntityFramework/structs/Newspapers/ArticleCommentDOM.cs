using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.structs.Newspapers
{
    public class ArticleCommentDom
    {
        public int AuthorID { get; set; }
        public string AuthorName { get; set; }
        public int EntityTypeID { get; set; }
        public string AuthorImgURL { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
        public int CreationDay { get; set; }
    }
}
