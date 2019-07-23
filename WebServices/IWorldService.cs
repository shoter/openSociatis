using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface IWorldService
    {
        /// <summary>
        /// it is doing every operation needed on day change.
        /// </summary>
        void ProcessDayChange();
        bool CanChangeDay();
    }
}
