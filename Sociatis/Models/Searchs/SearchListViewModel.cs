using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Searchs
{
    public class SearchListViewModel
    {
        public List<SearchItemViewModel> Items { get; set; }

        public SearchListViewModel(List<Entity> entities)
        {
            Items = entities.Select(e => new SearchItemViewModel(e)).ToList();
        }
    }
}