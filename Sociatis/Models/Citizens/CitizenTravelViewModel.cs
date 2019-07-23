using Entities;
using Entities.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Models.Citizens
{
    public class CitizenTravelViewModel
    {
        public CitizenInfoViewModel Info { get; set; }

        public List<SelectListItem> Countries { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> AvailableMovingTicketQualities { get; set; } = new List<SelectListItem>();

        public CitizenTravelViewModel(Entities.Citizen citizen, IList<MovingTicket> availableTickets, IFriendService friendService)
        {
            Info = new CitizenInfoViewModel(citizen, friendService);

            IEnumerable<Entities.Country> countries = Persistent.Countries.GetAll();

            Countries.Add(new SelectListItem()
            {
                Text = "-- Select country --",
                Value = "null"
            });

            foreach(var country in countries)
            {
                Countries.Add(new SelectListItem()
                {
                    Text = country.Entity.Name,
                    Value = country.ID.ToString()
                });
            }

            foreach(var ticket in availableTickets.OrderBy(t => t.Quality))
            {
                AvailableMovingTicketQualities.Add(new SelectListItem()
                {
                    Text = ticket.Quality.ToString(),
                    Value = ticket.Quality.ToString()
                });
            }
        }
    }
}