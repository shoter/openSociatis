using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class NewspaperJournalistExtension
    {
        public static NewspaperRightsEnum GetRights(this NewspaperJournalist journalist)
        {
            var rights = NewspaperRightsEnum.None;

            if (journalist.CanWriteArticles)
                rights |= NewspaperRightsEnum.CanWriteArticles;
            if (journalist.CanManageArticles)
                rights |= NewspaperRightsEnum.CanManageArticles;
            if (journalist.CanManageJournalists)
                rights |= NewspaperRightsEnum.CanManageJournalists;

            return rights;
        }
    }
}
