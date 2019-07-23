using Entities;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTests.UoW
{
    public class UnitOfWork
    {
        public static UnitOfWork Instance { private set; get; }
        private readonly SociatisEntities context;
        public readonly IHospitalRepository HospitalRepository;
        public readonly ICitizenRepository CitizenRepository;

        public UnitOfWork()
        {
            Instance = this;
            context = new SociatisEntities();
            HospitalRepository = new HospitalRepository(context);
            CitizenRepository = new CitizenRepository(context);
        }

        public Citizen GetTestCitizen()
        {
            return CitizenRepository
                .Where(c => c.Entity.Name == "test2")
                .First();
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
