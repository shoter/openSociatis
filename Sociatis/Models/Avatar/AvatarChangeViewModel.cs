using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Avatar
{
    public class AvatarChangeViewModel
    {
        public int EntityID { get; set; }

        public AvatarChangeViewModel(int entityID)
        {

            EntityID = entityID;
        }
    }
}