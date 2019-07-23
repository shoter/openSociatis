using Entities;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models
{
    public class ShortEntityInfoViewModel
    {
        public string Name { get; set; }
        public int EntityID { get; set; }
        public ImageViewModel Avatar { get; set; }

        public ShortEntityInfoViewModel() { }
        public ShortEntityInfoViewModel(Entity entity)
        {
            Avatar = new ImageViewModel(entity.ImgUrl);
            Name = entity.Name;
            EntityID = entity.EntityID;
        }
    }
}