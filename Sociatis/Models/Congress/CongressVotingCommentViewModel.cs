using Common.utilities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices.Helpers;

namespace Sociatis.Models.Congress
{
    public class CongressVotingCommentViewModel
    {
        public string Message { get; set; }
        public string AuthorName { get; set; }
        public string Ago { get; set; }
        public ImageViewModel AuthorAvatar { get; set; }

        public CongressVotingCommentViewModel(CongressVotingComment comment)
        {
            Message = comment.Message;
            Ago = AgoHelper.Ago(GameHelper.CurrentDay, DateTime.Now, comment.Day, comment.Time);
            AuthorName = comment.Citizen.Entity.Name;
            AuthorAvatar = new ImageViewModel(comment.Citizen.Entity.ImgUrl);


        }
    }
}