using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Game
{
    public class EntityViewModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int ID { get; set; }
        public int WalletID { get; set; }
        public int EquipmentID { get; set; }
        public string ImgURL { get; set; }

        public EntityViewModel(Entities.Entity entity)
        {
            Name = entity.Name;
            Type = ((EntityTypeEnum)entity.EntityTypeID).ToString();
            ID = entity.EntityID;
            WalletID = entity.WalletID;
            EquipmentID = entity.EquipmentID ?? -1;
            ImgURL = entity.ImgUrl;
        }
    }
}