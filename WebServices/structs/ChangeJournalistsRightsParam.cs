using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs
{
    public class ChangeJournalistsRightsParam
    {
        public Newspaper Newspaper { get; set; }
        public IDictionary<int, NewspaperRightsEnum> Rights { get; set; }

        public ChangeJournalistsRightsParam(Newspaper newspaper, IDictionary<int, NewspaperRightsEnum> rights)
        {
            Newspaper = newspaper;
            Rights = rights;
        }

        public ChangeJournalistsRightsParam() { }
    }
}
