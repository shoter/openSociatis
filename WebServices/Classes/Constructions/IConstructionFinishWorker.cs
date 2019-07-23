using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.Classes.Constructions
{
    public interface IConstructionFinishWorker
    {
        void AcumulateDataBeforeCompanyDelete(Construction construction);
        object ExecuteAction();
    }
}
