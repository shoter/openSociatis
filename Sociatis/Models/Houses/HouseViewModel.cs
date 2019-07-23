using Entities;
using SociatisCommon.Rights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Houses
{
    public class HouseViewModel
    {
        public HouseInfoViewModel Info { get; set; }

        public HouseViewModel(House house, HouseRights houseRights)
        {
            Info = new HouseInfoViewModel(house, houseRights);
        }
    }
}
