using Entities.enums;
using Entities.structs.Newspapers;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Newspapers
{
    public class ArticleCommentViewModel
    {
        public int AuthorID { get; set; }
        public string AuthorName { get; set; }
        public ImageViewModel AuthorImage { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }

        public ArticleCommentViewModel(ArticleCommentDom comment)
        {
            AuthorID = comment.AuthorID;
            AuthorName = comment.AuthorName;
            Content = comment.Content;
            Date = string.Format("day {0} {1}", comment.CreationDay, comment.CreationDate.ToShortTimeString());

            if(comment.EntityTypeID == (int)EntityTypeEnum.Country)
            {
                AuthorImage = Images.GetCountryFlag(comment.AuthorID).VM;
            }
            else
            {
                AuthorImage = new ImageViewModel(comment.AuthorImgURL);
            }
        }
    }
}