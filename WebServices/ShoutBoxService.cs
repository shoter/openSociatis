using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Repository;
using WebServices.Helpers;
using Common.Operations;
using System.Data.Entity;

namespace WebServices
{
    public class ShoutBoxService : BaseService, IShoutBoxService
    {
        private readonly IShoutboxMessageRepository shoutboxMessageRepository;
        private readonly IShoutboxChannelRepository shoutboxChannelRepository;
        private readonly ICountryRepository countryRepository;
        private readonly IConfigurationRepository configurationRepository;
        public ShoutBoxService(IShoutboxMessageRepository shoutboxMessageRepository, ICountryRepository countryRepository,
            IShoutboxChannelRepository shoutboxChannelRepository, IConfigurationRepository configurationRepository)
        {
            this.shoutboxMessageRepository = shoutboxMessageRepository;
            this.countryRepository = countryRepository;
            this.shoutboxChannelRepository = shoutboxChannelRepository;
            this.configurationRepository = configurationRepository;
        }

        public void AutoCreateChannels()
        {
            var countryWithoutShoutboxes = countryRepository
                .Where(c => c.NationalShoutboxChannelID.HasValue == false || c.NationalHelpShoutboxChannelID.HasValue == false ||
                c.NationalTradeShoutboxChannelID.HasValue == false)
                .Include(c => c.Entity)
                .ToList();

            using (NoSaveChanges)
            {
                foreach (var country in countryWithoutShoutboxes)
                {
                    if (country.NationalShoutboxChannel == null)
                        country.NationalShoutboxChannel = CreateChannel($"{country.Entity.Name} - Main");
                    if (country.NationalHelpShoutboxChannel == null)
                        country.NationalHelpShoutboxChannel = CreateChannel($"{country.Entity.Name} - Help");
                    if (country.NationalTradeShoutboxChannel == null)
                        country.NationalTradeShoutboxChannel = CreateChannel($"{country.Entity.Name} - Trade");
                }


                var config = configurationRepository.GetConfiguration();

                if (config.ShoutboxChannel == null)
                    config.ShoutboxChannel = CreateChannel("Global");
            }

            shoutboxChannelRepository.SaveChanges();
            countryRepository.SaveChanges();
        }

        public ShoutboxChannel CreateChannel(string name)
        {
            var channel = new ShoutboxChannel()
            {
                Name = name
            };

            shoutboxChannelRepository.Add(channel);
            ConditionalSaveChanges(shoutboxChannelRepository);
            return channel;
        }

        public ShoutboxMessage SendMessage(string content, Entity author, ShoutboxChannel channel)
        {
            var message = new ShoutboxMessage()
            {
                AuthorID = author.EntityID,
                ChannelID = channel.ID,
                Message = content,
                Time = DateTime.Now,
                Day = GameHelper.CurrentDay

            };



            shoutboxMessageRepository.Add(message);
            ConditionalSaveChanges(shoutboxMessageRepository);

            return message;
        }

        public ShoutboxMessage SendMessage(string content, Entity author, ShoutboxMessage parent)
        {
            var message = new ShoutboxMessage()
            {
                AuthorID = author.EntityID,
                ChannelID = parent.ChannelID,
                ParentID = parent.ID,
                Message = content,
                Time = DateTime.Now,
                Day = GameHelper.CurrentDay
            };

            shoutboxMessageRepository.Add(message);
            ConditionalSaveChanges(shoutboxMessageRepository);

            return message;
        }

        public MethodResult CanSendMessage(string content, ShoutboxChannel channel, Entity author)
        {
            if (author == null)
                return new MethodResult("Author does not exist!");
            if (channel == null)
                return new MethodResult("Channel does not exist!");

            if (string.IsNullOrWhiteSpace(content))
                return new MethodResult("You need to write something!");

            return MethodResult.Success;
        }

        public void MergeSameNameChannels()
        {
            //najpier probuj mergowac mainy

            var mainPairs = shoutboxChannelRepository
                 .GroupBy(c => c.Name)
                .Where(c => c.Count() > 1)
                .Where(c => c.Any(cc => cc.MainChannelCountries.Any()))
                .ToList()
                .ToDictionary(g => g.FirstOrDefault(c => c.MainChannelCountries.Any()), g => g.ToList());

            foreach (var channelPair in mainPairs)
            {
                var main = channelPair.Key;
                mergeChannels(main, channelPair.Value);
            }

            shoutboxChannelRepository.SaveChanges();

            //najpier probuj mergowac trade

            var tradePairs = shoutboxChannelRepository
                 .GroupBy(c => c.Name)
                .Where(c => c.Count() > 1)
                .Where(c => c.Any(cc => cc.TradeChannelCountries.Any()))
                .ToList()
                .ToDictionary(g => g.FirstOrDefault(c => c.TradeChannelCountries.Any()), g => g.ToList());

            foreach (var channelPair in tradePairs)
            {
                var main = channelPair.Key;
                mergeChannels(main, channelPair.Value);
            }

            shoutboxChannelRepository.SaveChanges();
            //najpier probuj mergowac help

            var helpPairs = shoutboxChannelRepository
                 .GroupBy(c => c.Name)
                .Where(c => c.Count() > 1)
                .Where(c => c.Any(cc => cc.HelpChannelCountries.Any()))
                .ToList()
                .ToDictionary(g => g.FirstOrDefault(c => c.HelpChannelCountries.Any()), g => g.ToList());

            foreach (var channelPair in helpPairs)
            {
                var main = channelPair.Key;
                mergeChannels(main, channelPair.Value);
            }

            shoutboxChannelRepository.SaveChanges();
            //najpier probuj mergowac globala

            var globalPairs = shoutboxChannelRepository
                 .GroupBy(c => c.Name)
                .Where(c => c.Count() > 1)
                .Where(c => c.Any(cc => cc.ConfigurationTables.Any()))
                .ToList()
                .ToDictionary(g => g.FirstOrDefault(c => c.ConfigurationTables.Any()), g => g.ToList());

            foreach (var channelPair in globalPairs)
            {
                var main = channelPair.Key;
                mergeChannels(main, channelPair.Value);
            }

            shoutboxChannelRepository.SaveChanges();

            //potem nationale

            var channelPairs = shoutboxChannelRepository
                .GroupBy(c => c.Name)
                .Where(c => c.Count() > 1)
                .ToList()
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var channelPair in channelPairs)
            {
                var main = channelPair.Value[0];
                mergeChannels(main, channelPair.Value);
            }

            shoutboxChannelRepository.SaveChanges();

        }

        public void mergeChannels(ShoutboxChannel main, IEnumerable<ShoutboxChannel> channels)
        {
            foreach(var channel in channels)
            {
                if (channel.ID == main.ID)
                    continue;

                foreach (var msg in channel.ShoutboxMessages.ToList())
                {
                    msg.ChannelID = main.ID;
                }

                shoutboxChannelRepository.Remove(channel);
            }
        }
    }
}
