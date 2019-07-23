using Common.EntityFramework;
using Entities.enums;
using Entities.Models.Events;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class CountryEventRepository : RepositoryBase<CountryEvent, SociatisEntities>, ICountryEventRepository
    {
        public CountryEventRepository(SociatisEntities context) : base(context)
        {
        }

        public List<EventModel> GetCountryEvents(int countryID, IEnumerable<Country> countries, int? count)
        {
            List<EventModel> events = new List<EventModel>();
            var query =
                Where(e => e.CountryID == countryID)
                .Include(e => e.CountryVotingEvent)
                .Include(e => e.Event)
                .Select(e => new
                {
                    Day = e.Event.Day,
                    Time = e.Event.Time,

                    EventID = e.EventID,
                    CountryEventTypeID = e.CountryEventTypeID,
                    CountryID = e.CountryID,
                    //voting events
                    VotingID = e.CountryVotingEvent.VotingID,
                    VotingStatusID = e.CountryVotingEvent.VotingStatusID
                });
            query = query.OrderByDescending(e => e.EventID);

            if (count.HasValue)
                query = query.Take(count.Value);

            var countryEvents = query.ToList();

            foreach (var e in countryEvents)
            {
                //narazie same congress voting to sobie ułtawie
                events.Add(new CountryVotingEventModel()
                {
                    EventID = e.EventID,
                    CountryID = e.CountryID,
                    EventTypeID = (int)EventTypeEnum.Country,
                     Day = e.Day,
                     Time = e.Time,
                     CountryName = countries.Single(c => e.CountryID == c.ID).Entity.Name,
                     VotingID = e.VotingID,
                     VotingStatusID = e.VotingStatusID
                });
            }


            return events;

        }
    }
}
