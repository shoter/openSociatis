using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface IStartService
    {
        /// <summary>
        /// Executed at start of the server
        /// </summary>
        void OnStart();
    }
}
