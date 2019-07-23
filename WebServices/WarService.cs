using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Operations;
using Entities;
using WebServices.Helpers;
using Entities.Extensions;
using Entities.Repository;
using Entities.QueryEnums;
using WebServices.structs;
using Common;
using Entities.enums;
using WebServices.structs.Battles;
using System.Diagnostics;
using Weber.Html;
using WebServices.Html;
using Common.Extensions;
using System.Transactions;

namespace WebServices
{
    public class WarService : BaseService, IWarService
    {
        private readonly IWarRepository warRepository;
        private readonly Entities.Repository.IWalletRepository walletRepository;
        private readonly ICountryRepository countryRepository;
        private readonly ITransactionsService transactionService;
        private readonly IWarningService warningService;
        private readonly ICitizenRepository citizenRepository;
        private readonly ICitizenService citizenService;
        private readonly IPopupService popupService;
        private readonly IWalletService walletService;
        private readonly IBattleRepository battleRepository;
        private readonly IWarEventService warEventService;
        public WarService(IWarRepository warRepository, Entities.Repository.IWalletRepository walletRepository, ICountryRepository countryRepository, ITransactionsService transactionService,
            IWarningService warningService, ICitizenRepository citizenRepository, ICitizenService citizenService, IPopupService popupService, IWalletService walletService,
            IBattleRepository battleRepository, IWarEventService warEventService)
        {
            this.warRepository = warRepository;
            this.walletRepository = walletRepository;
            this.countryRepository = countryRepository;
            this.transactionService = Attach(transactionService);
            this.warningService = Attach(warningService);
            this.citizenRepository = citizenRepository;
            this.citizenService = citizenService;
            this.popupService = Attach(popupService);
            this.walletService = Attach(walletService);
            this.battleRepository = battleRepository;
            this.warEventService = warEventService;
        }

        public MethodResult CanDeclareWar(Country attacker, Country defender)
        {
            var directWars = warRepository.GetDirectWarsForCountry(attacker.ID, WarActivitySearchCriteria.Active);

            if (directWars.Any(w => w.AttackerCountryID == attacker.ID && w.DefenderCountryID == defender.ID))
                return (MethodResultError)string.Format("Your country is already in war with {0}", defender.Entity.Name);

            double goldNeeded = GetNeededGoldToStartWar(attacker, defender);

            if (attacker.Entity.Wallet.HaveMoney(Persistent.Currencies.Gold.ID, goldNeeded) == false)
                return (MethodResultError)"You do not have enough gold to start this war";

            var neigbhours = countryRepository.GetNeighbourCountries(attacker.ID);

            if (neigbhours.Any(n => n.ID == defender.ID) == false)
                return (MethodResultError)"You do not have border with this country to attack it!";



            return MethodResult.Success;
        }

        public double GetGoldNeededToStartBattle(War war, Region region)
        {
            var activeBattleCount = war.Battles
                .Where(b => b.Active)
                .Count();

            double goldNeeded = getGoldNeededToStartBatle(region, activeBattleCount);

            return goldNeeded;
        }

        private static double getGoldNeededToStartBatle(Region region, int activeBattleCount)
        {
            double goldNeeded = Utils.Fibbonaci(activeBattleCount + 1) + (double)region.Development / 3 + (double)region.DefenseSystemQuality;
            goldNeeded = Math.Round(goldNeeded, 2);
            return goldNeeded;
        }

        public void TryToCreateTrainingWar()
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
                if (warRepository.Any(w => w.IsTrainingWar) == false)
                {
                    var anyCountry = countryRepository.First();
                    var anyRegion = anyCountry.Regions.First();
                    var trainingWar = new War()
                    {
                        AttackerCountryID = anyCountry.ID,
                        DefenderCountryID = anyCountry.ID,
                        Active = true,
                        StartDay = GameHelper.CurrentDay,
                        IsTrainingWar = true
                    };

                    warRepository.Add(trainingWar);
                    warRepository.SaveChanges();

                    var battle = new Battle()
                    {
                        Active = true,
                        WarID = trainingWar.ID,
                        WallHealth = 99999,
                        RegionID = anyRegion.ID,
                        StartTime = DateTime.Now,
                        StartDay = GameHelper.CurrentDay,

                    };

                    battleRepository.Add(battle);
                    battleRepository.SaveChanges();

                    trs.Complete();
                }
        }

        public MethodResult CanStartRessistanceBattle(Entity entity, Region region)
        {
            if (entity.GetEntityType() != EntityTypeEnum.Citizen)
                return new MethodResult("Only citizen can start battle!");

            return CanStartRessistanceBattle(entity.Citizen, region);
        }
        public MethodResult CanStartRessistanceBattle(Citizen citizen, Region region)
        {
            if (region == null)
                return new MethodResult("You cannot start war here!");
            if (region.CountryID.HasValue == false)
                return new MethodResult("There is no one here to fight against!");
            if (citizen.CitizenshipID != region.CountryCoreID)
                return new MethodResult("It is not your core region!");
            if (region.CountryCoreID == region.CountryID)
                return new MethodResult("Your country occupies this region. You cannot start ressistance war here!");
            if (citizen.Country.Regions.Count > 0)
                return new MethodResult("You cannot start ressistance war when your country possess at least 1 region!");
            if (citizen.RegionID != region.ID)
                return new MethodResult("You can start ressistance only in region where you are actually!");

            var activeRessistanceWar = GetActiveRessistanceWar(citizen.Country, region.CountryID.Value);
            if (activeRessistanceWar == null)
            {
                Money money = GetNeededMoneyToStartRessistanceWar(region);
                if (walletService.HaveMoney(citizen.Entity.WalletID, money) == false)
                    return new MethodResult("You do not have enough money to start ressistance war!");
            }
            else
            {
                if (activeRessistanceWar.Battles.Any(r => r.RegionID == region.ID))
                    return new MethodResult("Single war cannot have two ressistance battles in same region!");
                Money money = GetMoneyNeededToStartResistanceBattle(region);
                if (walletService.HaveMoney(citizen.Entity.WalletID, money) == false)
                    return new MethodResult("You do not have enough money to start ressistance battle!");
            }

            return MethodResult.Success;
        }



        public War StartRessistanceWar(Citizen startingCitizen, Country attackingCountry, Region defendingRegion, IBattleService battleService)
        {
            var defendingCountry = defendingRegion.Country;
            var war = new War()
            {
                Attacker = attackingCountry,
                Defender = defendingCountry,
                StartDay = GameHelper.CurrentDay,
                Active = true,
                EndDay = null,
                IsRessistanceWar = true,
                RessistanceStarterID = startingCitizen.ID
            };

            List<CountryInWar> participatingCountries = getParticipatingCountries(attackingCountry, defendingCountry, isRessistanceWar: true);

            war.CountryInWars = participatingCountries;


            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                warRepository.Add(war);
                ConditionalSaveChanges(warRepository);

                transactionService.PayForResistanceWar(startingCitizen, defendingRegion, this);
                battleService.CreateBattle(war, defendingRegion.ID, WarSideEnum.Attacker);

                trs.Complete();
                return war;
            }
        }

        public Battle StartRessistanceBattle(Citizen startingCitizen, Region defendingRegion, IBattleService battleService)
        {
            Country attackingCountry = startingCitizen.Country;
            var activeWar = GetActiveRessistanceWar(attackingCountry, defendingRegion.CountryID.Value);

            if (activeWar == null)
            {
                return StartRessistanceWar(startingCitizen, attackingCountry, defendingRegion, battleService).Battles.First();
            }
            else
            {
                using (var trs = transactionScopeProvider.CreateTransactionScope())
                {
                    transactionService.PayForResistanceBattle(startingCitizen, defendingRegion, this);
                    var battle = battleService.CreateBattle(activeWar, defendingRegion.ID, WarSideEnum.Attacker);

                    trs.Complete();
                    return battle;
                }
            }
        }

        public Money GetNeededMoneyToStartRessistanceWar(Region region)
        {
            decimal warFee = ConfigurationHelper.Configuration.RessistanceWarStartCost;
            decimal battleFee = (decimal)getGoldNeededToStartBatle(region, 0);
            var money = new Money(GameHelper.Gold, warFee + battleFee);
            return money;
        }

        public Money GetMoneyNeededToStartResistanceBattle(Region region)
        {
            var activeWar = GetActiveRessistanceWar(region.CoreCountry, region.CountryID.Value);
            var battleCount = activeWar?.Battles?.Count() ?? 0;
            if (battleCount == 0)
                return GetNeededMoneyToStartRessistanceWar(region);

            decimal battleFee = (decimal)getGoldNeededToStartBatle(region, battleCount / 2) / 2m;
            var money = new Money(GameHelper.Gold, battleFee);
            return money;
        }

        public virtual War GetActiveRessistanceWar(Country country, int defenderCountryID)
        {
            var active = warRepository
                .FirstOrDefault(war => war.AttackerCountryID == country.ID
                && war.Active && war.IsRessistanceWar && war.DefenderCountryID == defenderCountryID);

            return active;
        }

        public MethodResult CanStartBattle(Entity entity, Country attackingCountry, War war, Region region)
        {
            var warSide = GetWarSide(war, entity);
            if (war.IsRessistanceWar && warSide == WarSideEnum.Attacker && region.CountryCoreID != attackingCountry.ID)
                return new MethodResult("You can only attack your core regions!");

            MethodResult result;
            if ((result = CanStartBattle(entity, attackingCountry, war)).IsError)
            {
                return result;
            }



            return MethodResult.Success;
        }

        public MethodResult CanStartBattle(Entity entity, Country attackingCountry, War war)
        {
            if (war.Active == false)
                return new MethodResult("You cannot start battle. War is not active!");

            var operatedCountry = GetControlledCountryInWar(entity, war);
            if (operatedCountry == null || attackingCountry.PresidentID != operatedCountry.PresidentID)
                return new MethodResult("You are not a president of attacking country. You cannot start battle!");

            if (war.AttackerCountryID != attackingCountry.ID && war.DefenderCountryID != attackingCountry.ID)
                return new MethodResult("This country do not take part in the war. Send info about this error to adminsitration.");

            var warSide = GetWarSide(war, entity);
            if ((warSide == WarSideEnum.Attacker && war.AttackerOfferedSurrender == true) || (warSide == WarSideEnum.Defender && war.AttackerOfferedSurrender == false))
                return new MethodResult("You cannot attack after you send surrender offer!");

            if (warSide == WarSideEnum.None)
                return new MethodResult("You are not participating in this war!");

            if (warSide == WarSideEnum.Attacker && war.IsRessistanceWar)
                return new MethodResult("You cannot do that!");

            return MethodResult.Success;
        }



        public double GetNeededGoldToStartWar(Country attacker, Country defender)
        {
            return 5.0 + attacker.Regions.Count() * 2 + defender.Regions.Count();
        }


        public MethodResult<int> DeclareWar(Country attacker, Country defender)
        {

            var war = new War()
            {
                Attacker = attacker,
                Defender = defender,
                StartDay = GameHelper.CurrentDay,
                Active = true,
                EndDay = null
            };

            List<CountryInWar> participatingCountries = getParticipatingCountries(attacker, defender);

            war.CountryInWars = participatingCountries;


            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                warRepository.Add(war);
                ConditionalSaveChanges(warRepository);

                transactionService.PayForWar(attacker, defender, this);

                using (NoSaveChanges)
                    warEventService.AddEvent(war, WarStatusEnum.Started);

                trs.Complete();
            }

            return war.ID;
        }

        private static List<CountryInWar> getParticipatingCountries(Country attacker, Country defender, bool isRessistanceWar = false)
        {
            List<CountryInWar> participatingCountries = new List<CountryInWar>();

            List<Country> attackerAllies = attacker.GetAllies();
            List<Country> defenderAllies = defender.GetAllies();

            if (isRessistanceWar == true)
                foreach (var attackerAlly in attackerAllies)
                {
                    var ciw = new CountryInWar()
                    {
                        Country = attackerAlly,
                        IsAttacker = true
                    };

                    participatingCountries.Add(ciw);
                }

            foreach (var defenderAlly in defenderAllies)
            {
                var ciw = new CountryInWar()
                {
                    Country = defenderAlly,
                    IsAttacker = false
                };

                participatingCountries.Add(ciw);
            }

            //Eliminate clashing ones

            for (int att = 0; att < attackerAllies.Count; ++att)
                for (int def = 0; def < defenderAllies.Count; ++def)
                {
                    var attC = attackerAllies[att];
                    var defC = defenderAllies[def];

                    if (attC.ID == defC.ID)
                    {
                        attackerAllies.Remove(attC);
                        defenderAllies.Remove(defC);
                        --att;
                        break;
                    }

                }

            return participatingCountries;
        }



        public MethodResult CanSurrenderWar(War war, Entity entity)
        {
            var hasNotFinishedBattles = war.Battles
                .Where(b => b.Active)
                .Any();

            if (hasNotFinishedBattles)
                return new MethodResult("War has still active battles!");

            if (war.Attacker.PresidentID != entity.EntityID && war.Defender.PresidentID != entity.EntityID)
                return new MethodResult("You are not a president to end this war!");

            if (war.Active == false)
                return new MethodResult("You cannot end war that had ended!");

            var country = GetControlledCountryInWar(entity, war);

            if (war.AttackerOfferedSurrender == true && country.ID == war.AttackerCountryID)
                return new MethodResult("You already offered surrender!");

            if (war.AttackerOfferedSurrender == false && country.ID == war.DefenderCountryID)
                return new MethodResult("You already offered surrender!");

            return MethodResult.Success;
        }

        public MethodResult CanCancelSurrender(War war, Entity entity)
        {
            if (war.AttackerOfferedSurrender.HasValue == false)
                return new MethodResult("There is no surrender offer in the war!");

            var country = GetControlledCountryInWar(entity, war);

            if (country == null)
                return new MethodResult("You do not have control over country in war");

            if ((war.AttackerOfferedSurrender == true && country.ID != war.AttackerCountryID) ||
                war.AttackerOfferedSurrender == false && country.ID != war.DefenderCountryID)
                return new MethodResult("You cannot cancel surrender offer of another country!");

            return MethodResult.Success;

        }

        public void CancelSurrender(War war)
        {
            var warLink = WarLinkCreator.Create(war);
            if (war.AttackerOfferedSurrender == true)
            {
                var attackerLink = EntityLinkCreator.Create(war.Attacker.Entity);
                string message = $"{attackerLink} canceled surrender offer in {warLink}";

                SendMessageToEveryoneInWar(war, message, war.AttackerCountryID);
            }
            else
            {
                var defenderLink = EntityLinkCreator.Create(war.Defender.Entity);
                string message = $"{defenderLink} canceled surrender offer in {warLink}";

                SendMessageToEveryoneInWar(war, message, war.DefenderCountryID);
            }
            war.AttackerOfferedSurrender = null;
            popupService.AddSuccess("You canceled surrender offer.");

            warRepository.SaveChanges();
        }

        public void SurrenderWar(War war, Entity entity)
        {
#if DEBUG
            Debug.Assert(CanSurrenderWar(war, entity).isSuccess);
#endif

            var country = GetControlledCountryInWar(entity, war);
            bool isAttacker = war.AttackerCountryID == country.ID;

            surrenderWar(war, isAttacker);

        }

        private void surrenderWar(War war, bool isAttacker)
        {
            if (war.AttackerOfferedSurrender == true)
            {
                if (isAttacker == false)
                {
                    SendInformationAboutSurrenderAccept(war, war.Defender);
                    EndWar(war, informatAboutWarEnd: false);
                    popupService.AddSuccess("War has ended.");

                }
            }
            else if (war.AttackerOfferedSurrender == false)
            {
                if (isAttacker == true)
                {
                    SendInformationAboutSurrenderAccept(war, war.Attacker);
                    EndWar(war, informatAboutWarEnd: false);
                    popupService.AddSuccess("War has ended.");
                }
            }
            else
            {
                war.AttackerOfferedSurrender = isAttacker;
                using (NoSaveChanges)
                    SendInformationAboutSideSurrender(war, isAttacker ? war.Attacker : war.Defender);
                popupService.AddSuccess("You have created proposition to surrender war.");
            }

            warRepository.SaveChanges();
        }


        public void SendInformationAboutSideSurrender(War war, Country surrenderingCountry)
        {
            var countryLink = EntityLinkCreator.Create(surrenderingCountry.Entity);
            var warLink = WarLinkCreator.Create(war);


            string message = $"{countryLink} offered surrender in {warLink}.";
            SendMessageToEveryoneInWar(war, message, surrenderingCountry.ID);
        }

        public void SendInformationAboutSurrenderAccept(War war, Country acceptingCountry)
        {
            var countryLink = EntityLinkCreator.Create(acceptingCountry.Entity);
            var warLink = WarLinkCreator.Create(war);


            string message = $"{countryLink} accepted surrender in {warLink}.";
            SendMessageToEveryoneInWar(war, message, acceptingCountry.ID);
        }

        public void SendMessageToEveryoneInWar(War war, string message, params int[] excluding)
        {
            List<int> recipientsIDs = new List<int>()
            {
                war.AttackerCountryID,
                war.DefenderCountryID,
            };

            recipientsIDs.AddRange(war.CountryInWars.Select(c => c.CountryID).ToList());

            recipientsIDs = recipientsIDs.Where(id => excluding.Contains(id) == false).Distinct().ToList();

            foreach (var id in recipientsIDs)
                warningService.AddWarning(id, message);
        }



        public void EndWar(War war, bool informatAboutWarEnd = true)
        {
            war.Active = false;
            war.EndDay = GameHelper.CurrentDay;

            var attackerHero = GetWarHero(war, isAttacker: true);
            var defenderHero = GetWarHero(war, isAttacker: false);

            if (attackerHero != null)
                citizenService.ReceiveWarHeroMedal(attackerHero.Citizen);
            if (defenderHero != null)
                citizenService.ReceiveWarHeroMedal(defenderHero.Citizen);

            if (informatAboutWarEnd)
            {
                string warLink = WarLinkCreator.Create(war).ToHtmlString();
                string message = $"{warLink.FirstUpper()} has ended.";
                using (NoSaveChanges)
                    SendMessageToEveryoneInWar(war, message);
            }

            using (NoSaveChanges)
                warEventService.AddEvent(war, WarStatusEnum.Finished);

            ConditionalSaveChanges(warRepository);
        }

        public BattleHero GetWarHero(War war, bool isAttacker)
        {
            var hero = war.Battles
                .SelectMany(b => b.BattleParticipants)
                .Where(p => p.IsAttacker == isAttacker)
                .GroupBy(p => p.CitizenID)
                .Select(group => new
                {
                    CitizenID = group.Key,
                    Damage = group.Sum(p => p.DamageDealt)
                })
                .OrderByDescending(p => p.Damage)
                .FirstOrDefault();

            if (hero == null)
                return null;

            var citizen = citizenRepository.GetById(hero.CitizenID);

            return new BattleHero()
            {
                Citizen = citizen,
                DamageDealt = (double)hero.Damage
            };
        }

        public WarSideEnum GetWarSide(War war, Entity entity)
        {
            if (war.Attacker.PresidentID == entity.EntityID)
                return WarSideEnum.Attacker;
            if (war.Defender.PresidentID == entity.EntityID)
                return WarSideEnum.Defender;

            if (war.CountryInWars.Where(ciw => ciw.IsAttacker == true).Any(c => c.Country.PresidentID == entity.EntityID))
                return WarSideEnum.Attacker;
            if (war.CountryInWars.Where(ciw => ciw.IsAttacker == false).Any(c => c.Country.PresidentID == entity.EntityID))
                return WarSideEnum.Defender;


            return WarSideEnum.None;
        }

        public WarSideEnum GetFightingSide(War war, Citizen citizen)
        {
            var countryID = citizen.Entity.GetCurrentCountry().ID;

            if (war.AttackerCountryID == countryID || war.AttackerCountryID == citizen.CitizenshipID)
                return WarSideEnum.Attacker;
            if (war.DefenderCountryID == countryID)
                return WarSideEnum.Defender;

            if (war.CountryInWars.Where(ciw => ciw.IsAttacker == true).Any(c => c.CountryID == countryID || c.CountryID == citizen.CitizenshipID))
                return WarSideEnum.Attacker;
            if (war.CountryInWars.Where(ciw => ciw.IsAttacker == false).Any(c => c.CountryID == countryID))
                return WarSideEnum.Defender;


            return WarSideEnum.None;
        }

        public bool CanFightAs(Battle battle, War war, Citizen citizen, WarSideEnum warSide)
        {
            if (citizen == null)
                return false;
            if (war == null)
                return false;

            if (war.IsTrainingWar)
                return true;

            var fightingSide = GetFightingSide(war, citizen);

            if (battle.AttackerInitiatedBattle == false)
            {
                if (warSide == WarSideEnum.Attacker)
                    warSide = WarSideEnum.Defender;
                else
                    warSide = WarSideEnum.Attacker;
            }

            if (fightingSide == warSide || fightingSide == WarSideEnum.Both)
                return true;

            return false;
        }

        public void ProcessDayChange(int newDay)
        {
            var wars = warRepository.GetAllActiveWars();
            using (NoSaveChanges)
            {
                foreach (var war in wars)
                {
                    var lastBattle = war.GetLastBattle();

                    if (lastBattle == null || lastBattle.Active == false)
                    {
                        int lastBattleTime = newDay - war.StartDay;
                        if (lastBattle != null)
                            lastBattleTime = newDay - lastBattle.StartDay + 1;

                        if (lastBattleTime >= 30)
                            CancelWar(war, "{0} between {1} and {2} has ended due to inactivity");
                        else if (war.IsRessistanceWar && lastBattleTime > 2)
                        {
                            CancelWar(war, "Ressistance {0} between {1} and {2} has ended due to no active ressistance battles");

                            var wonBattles = war.Battles.Count(b => b.WonByAttacker == true);

                            var goldForRessistanceHero = ConfigurationHelper.Configuration.RessistanceWarWinCost + wonBattles * 5;

                            citizenService.ReceiveRessistanceHeroMedal(war.Citizen, war, wonBattles, (double)goldForRessistanceHero);
                        }
                        else
                        {
                            int attackerRegions = war.Attacker.Regions.Count;
                            int defenderRegions = war.Defender.Regions.Count;

                            if (attackerRegions == 0 && defenderRegions > 0)
                            {
                                EndWar(war, informatAboutWarEnd: false);
                            }
                            else if (attackerRegions > 0 && defenderRegions == 0)
                                EndWar(war, informatAboutWarEnd: false);

                        }
                    }
                }
            }
            warRepository.SaveChanges();
        }

        public void CancelWar(War war, string warMessage = null)
        {
            using (NoSaveChanges)
                EndWar(war);

            var warLink = WarLinkCreator.Create(war);
            var attackerLink = EntityLinkCreator.Create(war.Attacker.Entity);
            var defenderLink = EntityLinkCreator.Create(war.Defender.Entity);

            if (warMessage == null)
                warMessage = string.Format("{0} between {1} and {2} has ended", warLink.ToHtmlString(), attackerLink.ToHtmlString(), defenderLink.ToHtmlString());
            else
                warMessage = string.Format(warMessage, warLink.ToHtmlString(), attackerLink.ToHtmlString(), defenderLink.ToHtmlString());

            SendMessageToEveryoneInWar(war, warMessage);

            warRepository.SaveChanges();
        }

        public Country GetControlledCountryInWar(Entity entity, War war)
        {
            if (war.IsTrainingWar)
                return null;

            switch (entity.GetEntityType())
            {
                case EntityTypeEnum.Citizen:
                    var operatedCountry = countryRepository
                        .Where(c => c.PresidentID == entity.EntityID
                        && (war.AttackerCountryID == c.ID || war.DefenderCountryID == c.ID)
                        )
                        .FirstOrDefault();

                    return operatedCountry;
                default:
                    return null;
            }
        }

        public bool AreAtWar(Country firstCountry, Country secondCountry)
        {
            if (warRepository.Any(w => w.Active &&
                     ((w.AttackerCountryID == firstCountry.ID && w.DefenderCountryID == secondCountry.ID)
                     ||
                     (w.AttackerCountryID == secondCountry.ID && w.AttackerCountryID == firstCountry.ID))))
                return true;

            return false;
        }
    }
}
