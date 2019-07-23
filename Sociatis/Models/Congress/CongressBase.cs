using Sociatis.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Congress
{
    public class CongressBase : ViewModelBase
    {
        public CongressInfoViewModel Info { get; set; }

        public CongressBase(Entities.Country country)
        {
            Init(country);
        }

        public CongressBase()
        {

        }

        public void Init(Entities.Country country)
        {
            Info = new CongressInfoViewModel(country);
        }
    }
}