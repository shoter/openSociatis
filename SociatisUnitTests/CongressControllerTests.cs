using Entities;
using Entities.Repository;
using Moq;
using Sociatis.Controllers;
using Sociatis.Models.Congress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebServices;

namespace SociatisUnitTests
{
    public class CongressControllerTests
    {
       // private CongressController controller;
        Mock<ICongressVotingRepository> congressVotingRepository = new Mock<ICongressVotingRepository>();
        Mock<ICongressVotingService> congressVotingService = new Mock<ICongressVotingService>();
        Mock<ICongressCandidateVotingRepository> congressCandidateVotingRepository = new Mock<ICongressCandidateVotingRepository>();
        Mock<ICongressCandidateService> congressCandidateService = new Mock<ICongressCandidateService>();
        Mock<ICountryRepository> countryRepository = new Mock<ICountryRepository>();
        Mock<IRegionRepository> regionRepository = new Mock<IRegionRepository>();
        Mock<IPartyRepository> partyRepository = new Mock<IPartyRepository>();

        public CongressControllerTests()
        {
            //controller = new CongressController(congressVotingRepository.Object, congressVotingService.Object,
            //    countryRepository.Object, regionRepository.Object, congressCandidateVotingRepository.Object,
            //    congressCandidateService.Object, partyRepository.Object, );
        }

        /*public StartVoting_VAT_MinusVatTest()
        {
            Session
        }*/

    }
}
