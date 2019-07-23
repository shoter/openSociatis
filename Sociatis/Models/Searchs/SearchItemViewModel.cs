using Entities;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Searchs
{
    public class SearchItemViewModel
    {
        public ImageViewModel Avatar { get; set; }
        public string Name { get; set; }
        public int ID { get; set; }


        public SearchItemViewModel(Entity entity)
        {
            Avatar = new ImageViewModel(entity.ImgUrl);
            Name = entity.Name;
            ID = entity.EntityID;
        }
    }
}