using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Times;

namespace WebServices.Events
{
    public class CountryGameEvent : GameEvent
    {
        public int CountryID { get; set; }
        public CountryEventTypeEnum CountryEventType { get; set; }
        public CountryGameEvent(CountryEvent countryEvent) : base(countryEvent.Event)
        {
            CountryID = countryEvent.CountryID;
            CountryEventType = (CountryEventTypeEnum)countryEvent.CountryEventTypeID;
        }

        public CountryGameEvent(Country country, CountryEventTypeEnum countryEventType, GameTime gameTime)
            : base(EventTypeEnum.Country, gameTime.Day, gameTime.Time)
        {
            CountryID = country.ID;
            CountryEventType = countryEventType;
        }

        public override Event CreateEntity()
        {
            var e = base.CreateEntity();

            e.CountryEvent = new CountryEvent()
            {
                CountryID = CountryID,
                CountryEventTypeID = (int)CountryEventType
            };

            return e;
        }
    }
}
