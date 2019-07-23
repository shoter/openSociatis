using Entities;
using Entities.enums;
using Entities.Extensions;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Entit
{
    public class SmallEntityAvatarViewModel
    {
        public string Name { get; set; }
        public int? ID { get; set; }
        public ImageViewModel Avatar { get; set; }

        public bool IncludeName { get; set; } = true;

        public string Classname { get; set; }

        public SmallEntityAvatarViewModel(Entity entity)
        : this(entity.EntityID, entity.Name, entity.ImgUrl) {
            if (entity.GetEntityType() == EntityTypeEnum.Country)
                Avatar = Images.GetCountryFlag(entity.EntityID).VM;

        }

        public SmallEntityAvatarViewModel(int? entityID, string name, string imgUrl)
        {
            ID = entityID;
            Name = name;
            Avatar = new ImageViewModel(imgUrl);
        }

        public SmallEntityAvatarViewModel DisableNameInclude()
        {
            IncludeName = false;
            return this;
        }
    }
}