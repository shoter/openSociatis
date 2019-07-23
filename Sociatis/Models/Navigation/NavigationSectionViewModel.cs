using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Navigation
{
    /// <summary>
    /// It will display partialName with CORRECT view model
    /// </summary>
    public class NavigationSectionViewModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public List<NavigationSectionViewModel> Children { get; set; } = new List<NavigationSectionViewModel>();

        public NavigationSectionViewModel()
        {
        }

        public override string ToString()
        {
            return Name;
        }


    }
}