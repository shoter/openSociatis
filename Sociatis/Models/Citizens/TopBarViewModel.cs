using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Citizens
{
    public class TopBarViewModel
    {
        public int CitizenID { get; set; }

        public TopBarViewModel() { }
        public TopBarViewModel(int citizenID)
        {
            CitizenID = citizenID;
        }
    }
}
