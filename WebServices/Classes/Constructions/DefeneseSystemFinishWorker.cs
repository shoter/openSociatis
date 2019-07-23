using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace WebServices.Classes.Constructions
{
    public class DefeneseSystemFinishWorker : IConstructionFinishWorker
    {
        private int quality;
        private Region region;
        public void AcumulateDataBeforeCompanyDelete(Construction construction)
        {
            region = construction.Company.Region;
            quality = construction.Company.Quality;
        }

        public object ExecuteAction()
        {
            region.DefenseSystemQuality = quality;
            return region;
        }
    }
}
