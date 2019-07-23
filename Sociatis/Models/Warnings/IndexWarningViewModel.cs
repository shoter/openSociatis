using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebUtils;

namespace Sociatis.Models.Warnings
{
    public class IndexWarningViewModel
    {
        public List<WarningViewModel> Warnings { get; set; } = new List<WarningViewModel>();
        public PagingParam PagingParam { get; set; }

        public IndexWarningViewModel(List<Entities.Warning> warnings, PagingParam pagingParam)
        {
            PagingParam = pagingParam;

            foreach(var warning in warnings)
            {
                var vm = new WarningViewModel(warning);
                Warnings.Add(vm);
            }
        }

    }
}