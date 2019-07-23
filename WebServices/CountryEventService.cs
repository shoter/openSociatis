using Entities;
using Entities.enums;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Events;
using WebServices.Times;

namespace WebServices
{
    public class CountryEventService : BaseService, ICountryEventService
    {
        private readonly IEventRepository eventRepository;
        public CountryEventService(IEventRepository eventRepository)
        {
            this.eventRepository = eventRepository;
        }
        public void AddVotingEvent(CongressVoting voting, VotingStatusEnum votingStatus)
        {
            var countryEvent = new CountryVotingGameEvent(voting, votingStatus, GameTime.Now);
            eventRepository.Add(countryEvent.CreateEntity());
            ConditionalSaveChanges(eventRepository);
        }
    }
}
