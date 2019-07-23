using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Hotels
{
    public class HotelManagerModel
    {
        public int ManagerID { get; set; }
        public string ManagerName { get; set; }
        public string ImgURL { get; set; }
        public HotelRights HotelRights { get; set; }
    }


}
