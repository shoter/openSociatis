using Entities.enums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Controllers
{
    public class ContractController : ControllerBase
    {
        IContractRepository contractRepository;

        public ContractController(IContractRepository contractRepository, IPopupService popupService) : base(popupService)
        {
            this.contractRepository = contractRepository;
        }

        [Route("{contractID:int}")]
        [Route("Index/{contractID:int}")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Index(int contractID)
        {
            var contract = contractRepository.GetById(contractID);

            IndexContractViewModel vm = new IndexContractViewModel(contract);

            return View(vm);

        }
    }
}
