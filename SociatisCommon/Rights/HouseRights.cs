using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SociatisCommon.Rights
{
    public class HouseRights
    {
        public bool CanSeeHouse { get; set; }
        public bool CanModifyHouse { get; set; }

        public HouseRights(bool canSeeHouse, bool canModifyHouse)
        {
            CanSeeHouse = canSeeHouse;
            CanModifyHouse = canModifyHouse;
        }

        public HouseRights()
        {
        }
    }
}
