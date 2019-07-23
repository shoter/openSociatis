using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    [Flags]
    public enum PathFindingModeEnum
    {
        Normal = 1,
        AvoidEnemy = 2,
        AvoidEmbargo = 4
         
    }
}
