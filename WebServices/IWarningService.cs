using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface IWarningService
    {
        void SendWarningToCongress(Country country, string message);
        void AddWarning(int entityID, string message);
    }
}
