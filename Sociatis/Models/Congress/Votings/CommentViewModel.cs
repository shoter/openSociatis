using Common.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices.Helpers;

namespace Sociatis.Models.Congress.Votings
{
    public class CommentViewModel
    {
        public string CreatorName { get; set; }
        public ImageViewModel CreatorAvatar { get; set; }
        public string Ago { get; set; }
        public string Message { get; set; }

        public CommentViewModel(Entities.CongressVotingComment comment)
        {
            CreatorAvatar = new ImageViewModel(comment.Citizen.Entity.ImgUrl);
            CreatorName = comment.Citizen.Entity.Name;

            Ago = AgoHelper.Ago(GameHelper.CurrentDay, DateTime.Now, comment.Day, comment.Time);
            Message = comment.Message;
        }
    }
}