using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public class StartService : BaseService, IStartService
    {
        private readonly IShoutBoxService shoutBoxService;
        private readonly IHospitalService hospitalService;
        private readonly IEntityRepository entityRepository;

        public StartService(IShoutBoxService shoutBoxService, IHospitalService hospitalService, IEntityRepository entityRepository)
        {
            this.shoutBoxService = Attach(shoutBoxService);
            this.hospitalService = Attach(hospitalService);
            this.entityRepository = entityRepository;
        }
        public void OnStart()
        {
            using (NoSaveChanges)
            {
                shoutBoxService.AutoCreateChannels();
                hospitalService.ReserveNamesForNationalHospitals();
            }
            entityRepository.SaveChanges();
         
        }
    }
}
