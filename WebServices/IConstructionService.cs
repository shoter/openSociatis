using Common.Operations;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface IConstructionService
    {
        int GetProgressNeededToBuild(ProductTypeEnum constructionType, int quality);

        MethodResult CanFinishOneTimeConstruction(Construction construction, Entity currentEntity, Citizen loggedCitizen);
        object FinishOneTimeConstruction(Construction construction);
    }
}
