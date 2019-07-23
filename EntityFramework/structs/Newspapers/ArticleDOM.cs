using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.structs.Newspapers
{
    public class ArticleDom
    {
        public int NewspaperID { get; set; }
        public int ArticleID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string PaidContent { get; set; }
        public string ImgURL { get; set; }
        public int CountryID { get; set; }
        public DateTime CreationDate { get; set; }
        public int VoteScore { get; set; }
        public string ShortDescription { get; set; }
        public int CreationDay { get; set; }
        public double? Price { get; set; }
        public List<ArticleCommentDom> Comments { get; set; } = new List<ArticleCommentDom>();
        public int CommentCount { get; set; }
        public bool UnlockedContent { get; set; }
        public bool Published { get; set; }
    }
}
