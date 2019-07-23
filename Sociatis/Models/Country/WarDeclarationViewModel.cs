using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Country
{
    public class WarDeclarationViewModel
    {
        public int SelectedCountryID { get; set; }
        public int AttackerCountryID { get; set; }

        public WarDeclarationViewModel() { }
    }
}