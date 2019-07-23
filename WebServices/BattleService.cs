using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Repository;
using Entities.enums;
using Entities.Extensions;
using WebServices.Helpers;
using WebServices.PathFinding;
using WebServices.enums;
using WebServices.structs;
using Common;
using System.Transactions;
using WebServices.structs.Battles;
using Weber.Html;
using WebServices.Html;
using Common.Transactions;
using System.Diagnostics;

namespace WebServices
{
    public class BattleService : BaseService, IBattleService
    {
        private readonly IBattleRepository battleRepository;
        private readonly IEquipmentRepository equipmentRepository;
        private readonly IRegionRepository regionRepository;
        private readonly IWarService warService;
        private readonly IWarningService warningService;
        private readonly ICountryRepository countryRepository;
        private readonly ITransactionsService transactionService;
        private readonly IRegionService regionService;
        private readonly IWarRepository warRepository;
        private readonly ICitizenRepository citizenRepository;
        private readonly ICitizenService citizenService;
        private readonly IEntityRepository entityRepository;
        private readonly IBattleEventService battleEventService;

        public BattleService(IBattleRepository battleRepository, IRegionRepository regionRepository, IWarService warService, IEquipmentRepository equipmentRepository,
            IWarningService warningService, ICountryRepository countryRepository, ITransactionsService transactionService, IRegionService regionService,
            IWarRepository warRepository, ICitizenRepository citizenRepository, ICitizenService citizenService, IEntityRepository entityRepository,
            IBattleEventService battleEventService)
        {
            this.battleRepository = battleRepository;
            this.regionRepository = regionRepository;
            this.warService = Attach(warService);
            this.equipmentRepository = equipmentRepository;
            this.warningService = Attach(warningService);
            this.countryRepository = countryRepository;
            this.transactionService = Attach(transactionService);
            this.regionService = Attach(regionService);
            this.warRepository = warRepository;
            this.citizenRepository = citizenRepository;
            this.citizenService = citizenService;
            this.entityRepository = entityRepository;
            this.battleEventService = battleEventService;
        }


        public Battle CreateBattle(War war, int regionID, WarSideEnum attackingSide)
        {
            using (var transaction = transactionScopeProvider.CreateTransactionScope())
            {
                var region = regionRepository.GetById(regionID);
                double startingWallHealth = CalculateWallHealth(region);

                var battle = new Battle()
                {
                    RegionID = regionID,
                    WallHealth = (decimal)startingWallHealth,
                    War = war,
                    Active = true,
                    StartTime = DateTime.Now,
                    StartDay = GameHelper.CurrentDay,
                    AttackerInitiatedBattle = attackingSide == WarSideEnum.Attacker
                };

                var gold = warService.GetGoldNeededToStartBattle(war, region);
                MakeBattleStartTransaction(war, war.GetMainCountry(attackingSide), gold);

                SendMessageAboutAttack(war, attackingSide, region);

                war.AttackerOfferedSurrender = null;

                battleRepository.Add(battle);
                ConditionalSaveChanges(battleRepository);
                battleEventService.AddEvent(battle, BattleStatusEnum.Started);
                transaction?.Complete();
                return battle;
            }
        }

        public virtual void SendMessageAboutAttack(War war, WarSideEnum attackingSide, Region region)
        {
            var attacker = attackingSide == WarSideEnum.Attacker ? war.Attacker.Entity : war.Defender.Entity;
            var attackerLink = EntityLinkCreator.Create(attacker).ToHtmlString();
            var regionLink = RegionLinkCreator.Create(region).ToHtmlString();
            var warLink = WarLinkCreator.Create(war).ToHtmlString();

            string message = $"{attackerLink} attacked {regionLink} in {warLink}!";
            warService.SendMessageToEveryoneInWar(war, message, attacker.EntityID);
        }

        public virtual double CalculateWallHealth(Region region)
        {
            var development = (double)region.Development;

            double devModifier = Math.Sqrt(Math.Tan(development / 5.0)) / Math.Sqrt(Math.Tan(1));

            return 1000 * devModifier * ((double)region.DefenseSystemQuality + 1.0);
        }

        public void ParticipateInBattle(Citizen citizen, Battle battle, bool isAttacker, int weaponQuality)
        {
            var damage = CalculateDamage(citizen, battle, weaponQuality, isAttacker);

            if (isAttacker)
                battle.WallHealth -= (decimal)damage;
            else
                battle.WallHealth += (decimal)damage;

            var currentRank = MilitaryRankEnumExtensions.GetRankForMilitaryRank((double)citizen.MilitaryRank);
            citizen.MilitaryRank += (decimal)damage;
            citizen.HitPoints -= 10;
            var newRank = MilitaryRankEnumExtensions.GetRankForMilitaryRank((double)citizen.MilitaryRank);

            if(currentRank != newRank)
            {
                var msg = string.Format("You were promoted to {0}.", newRank.ToHumanReadable());
                warningService.AddWarning(citizen.ID, msg);
            }

            if (weaponQuality > 0)
             equipmentRepository.RemoveEquipmentItem(citizen.Entity.EquipmentID.Value, (int)ProductTypeEnum.Weapon, weaponQuality);

            var participant = new BattleParticipant()
            {
                BattleID = battle.ID,
                CitizenID = citizen.ID,
                DamageDealt = (decimal)damage,
                IsAttacker = isAttacker,
                WeaponQualityUsed = weaponQuality,
                Day = GameHelper.CurrentDay,
                DateTime = DateTime.Now
            };

            battleRepository.AddParticipant(participant);
            ConditionalSaveChanges(battleRepository);

        }

        public void ProcessDayChange(int newDay)
        {
            var activeBattles = battleRepository.Where(b => b.Active && b.War.IsTrainingWar == false).ToList();

            foreach(var battle in activeBattles)
            {
               
                if(CanEndBattle(battle, newDay))
                using (NoSaveChanges)
                {
                        EndBattle(battle);
                }
            }
            ConditionalSaveChanges(battleRepository);
        }

        public bool CanEndBattle(Battle battle, int day)
        {
            var timeLeft = battle.GetTimeLeft(day);

            return timeLeft.TotalSeconds <= 0;
        }

        public BattleHero GetBattleHero(Battle battle, bool isAttacker)
        {
            bool participantAttacker = isAttacker;

            var hero = battle.BattleParticipants
                    .Where(p => p.IsAttacker == participantAttacker)
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

        public void EndBattle(Battle battle)
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                battle.Active = false;
                var defender = battle.GetDefender();
                var attacker = battle.GetAttacker();

                double defenseSTR = (double)battle.BattleParticipants
                        .Where(p => p.IsAttacker == false)
                        .Select(p => p.DamageDealt).Sum();

                double attackerSTR = (double)battle.BattleParticipants
                        .Where(p => p.IsAttacker == true)
                        .Select(p => p.DamageDealt).Sum();

                var bestAttacker = battle.BattleParticipants
                    .Where(p => p.IsAttacker == true)
                    .GroupBy(p => p.CitizenID)
                    .Select(group => new
                    {
                        CitizenID = group.Key,
                        Damage = group.Sum(p => p.DamageDealt)
                    })
                    .OrderByDescending(p => p.Damage)
                    .FirstOrDefault();

                var bestDefender = battle.BattleParticipants
                    .Where(p => p.IsAttacker == false)
                    .GroupBy(p => p.CitizenID)
                    .Select(group => new
                    {
                        CitizenID = group.Key,
                        Damage = group.Sum(p => p.DamageDealt)
                    })
                    .OrderByDescending(p => p.Damage)
                    .FirstOrDefault();

                if (bestAttacker != null)
                {
                    var bestAttackerCitizen = citizenRepository.GetById(bestAttacker.CitizenID);
                    citizenService.ReceiveBattleHeroMedal(bestAttackerCitizen);
                }
                if (bestDefender != null)
                {
                    var bestDefenderCitizen = citizenRepository.GetById(bestDefender.CitizenID);
                    citizenService.ReceiveBattleHeroMedal(bestDefenderCitizen);
                }

                double destroyedDevelopement;
                if (battle.WallHealth < 0)
                {
                    takeRegionByAttacker(battle);
                    destroyedDevelopement = calculateDestroyedSupplyValueOnAttackerWin((double)battle.WallHealth, defenseSTR, attackerSTR);
                    battle.Region.SupplyProgramType = (int)SupplyProgramTypeEnum.None;
                    battle.Region.DefenseSystemQuality -= Math.Max(0, battle.Region.DefenseSystemQuality - 1);
                    battle.WonByAttacker = true;

                    var destroyedDevelopementRatio = ((double)battle.Region.Development * destroyedDevelopement);
                    var countryDevelopementValue = countryRepository.GetCountryDevelopementValue(defender.ID);

                    double distanceFromCapitalRatio = 0.0; //if no capital then it's 0.0
                   
                    if (defender.CapitalID != null)
                    {
                        var path = regionService.GetPathBetweenRegions(battle.Region, defender.Capital);
                        distanceFromCapitalRatio = 3.0 - 9.0 / (path.Distance / 700 + 3) + Math.Log(path.Distance + 500, 500);
                    }

                    double goldTakenRatio = Math.Sqrt(destroyedDevelopementRatio / (countryDevelopementValue + destroyedDevelopementRatio + distanceFromCapitalRatio) + 1.0) - 1.0;
                    double goldTaken = goldTakenRatio * (double)defender.Entity.Wallet.GetMoney(CurrencyTypeEnum.Gold, Persistent.Currencies.GetAll()).Amount;

                    transactionService.PayForBattleLoss(attacker, defender, goldTaken);
                    battle.GoldTaken = (decimal)goldTaken;

                    battleEventService.AddEvent(battle, BattleStatusEnum.AttackerWin);
                }
                else
                {
                    destroyedDevelopement = calculateDestroyedSupplyValueOnDefenderWin((double)battle.WallHealth, defenseSTR, attackerSTR);
                    battle.WonByAttacker = false;
                    battleEventService.AddEvent(battle, BattleStatusEnum.DefenderWin);
                }

                destroyedDevelopement = Math.Max(destroyedDevelopement, 0.0);
                destroyedDevelopement = Math.Min(destroyedDevelopement, 1.0);

                battle.Region.Development -= (decimal)((double)battle.Region.Development * destroyedDevelopement);
                battle.DestroyedDevelopment = (decimal)destroyedDevelopement;


                ConditionalSaveChanges(battleRepository);
                trs?.Complete();
            }
        }

        private static double calculateDestroyedSupplyValueOnDefenderWin(double wallHealth, double defenseSTR, double attackerSTR)
        {
            var val = attackerSTR / (0.5 * defenseSTR + 3 * wallHealth + 1.5 * attackerSTR);
            if (val > 0.9) val = 0.9;
            
            val = 1.0 - val;

            val = Math.Pow(val, -1);

            return Math.Min(1, Math.Max(0, -0.05 + Math.Log10(val)));
        }

        private static double calculateDestroyedSupplyValueOnAttackerWin(double wallHealth, double defenseSTR, double attackerSTR)
        {
            var val = attackerSTR / (3 * Math.Abs(wallHealth) + 1.5 * attackerSTR);
            if (val > 0.9) val = 0.9;

            val = 1.0 - val;

            val = Math.Pow(val, -1);

            return Math.Min(1, Math.Max(0, Math.Log10(val)));
        }

        private void takeRegionByAttacker(Battle battle)
        {
            EnableSpawnInWholeCountryIfLastSpawnableRegion(battle);

            var attacker = battle.GetAttacker();

            var attackerCountryID = attacker.ID;

            battle.Region.CountryID = attackerCountryID;
            battle.Region.Country = attacker;

            if (battle.War.Defender.CapitalID == battle.RegionID)
            {
                battle.War.Defender.CapitalID = null;
            }
        }

        private void EnableSpawnInWholeCountryIfLastSpawnableRegion(Battle battle)
        {
            if (battle.Region.CanSpawn)
            {
                if (battle.War.Defender.Regions.Count(r => r.CanSpawn) == 1)
                {
                    foreach (var reg in battle.War.Defender.Regions.ToList())
                    {
                        reg.CanSpawn = true;
                    }
                    var msg = "Spawn was automatically enabled in whole country due to losing last region with enabled spawn!";
                    warningService.AddWarning(battle.War.DefenderCountryID, msg);
                }
            }
        }

        public double CalculateDamage(Citizen citizen, Battle battle, int weaponQuality, bool isAttacker)
        {
            double overallModifier = CalculateOverallModifier(citizen, battle, weaponQuality, isAttacker);
            double weaponBonus = CalculateWeaponBonus(weaponQuality);

            var dmg = Math.Round(35 * overallModifier + weaponBonus);

            return Math.Max(dmg, 1);
        }

        public double CalculateWeaponBonus(int weaponQuality)
        {
            return 10 * weaponQuality;
        }

        public  double CalculateOverallModifier(Citizen citizen, Battle battle, int weaponQuality, bool isAttacker)
        {
            var str = (double)citizen.Strength;

            var healthModifier = CalculateHealthModifier(citizen.HitPoints);

            double battleRankModifier = CalculateMilitaryRankModifier((double)citizen.MilitaryRank);

            double strengthModifier = CalculateStrengthModifier(str, weaponQuality);

            double distanceModifier = CalculateDistanceModifier(citizen, battle, isAttacker);

            double supplyModifier = CalculateDevelopmentModifier(battle, isAttacker);

            return distanceModifier * strengthModifier * battleRankModifier * healthModifier * supplyModifier;
        }

        public double CalculateDevelopmentModifier(Battle battle, bool isAttacker)
        {
            double supplyModifier = 1.0;

            if (isAttacker == false && battle.War.IsTrainingWar == false)
            {
                var sup = (double)battle.Region.Development;
                supplyModifier = (sup > 1 ? Math.Pow(sup, 0.25) : Math.Pow(sup, 2)) / 2 + 0.5;
            }

            return supplyModifier;
        }

        public double CalculateDistanceModifier(Citizen citizen, Battle battle, bool isAttacker)
        {
            if (citizen.RegionID == battle.RegionID)
                return 1f;

            Country associatedCountry = null;

            if (isAttacker)
                associatedCountry = battle.GetAttacker();
            else
                associatedCountry = battle.GetDefender();


            var path = regionService.GetPathBetweenRegions(citizen.Entity.GetCurrentRegion(), battle.Region, new AvoidEnemiesRegionSelector(associatedCountry, warRepository, battle.RegionID), new DefaultPassageCostCalculator(regionService));

            if (battle.War.IsTrainingWar)
                return 1f;

            if (path == null)
                return 0.5f;

            return .7f + .3f / Math.Sqrt(path.Distance / 1000f + 1f);
        }

        public double CalculateStrengthModifier(double str, int weaponQuality)
        {
            double weaponModifier = calculateWeaponModifier(weaponQuality);
            var modifier =  Math.Sqrt(Math.Log(str * weaponModifier + 2, 2));
            if (weaponQuality == 0)
                modifier /= 2;
            return modifier;
        }

        public double CalculateHealthModifier(double hitPoints)
        {
            return hitPoints / 100.0;
        }

        public List<int> GetUsableQualitiesOfWeapons(Citizen citizen)
        {
            return entityRepository.Where(e => e.EntityID == citizen.ID)
                .Select(e => e.Equipment)
                .SelectMany(e => e.EquipmentItems)
                .Where(e => e.ProductID == (int)ProductTypeEnum.Weapon)
                .Where(i => i.Amount > 0)
                .Select(i => i.Quality)
                .ToList();
        }

        private double calculateWeaponModifier(int weaponQuality)
        {
            switch(weaponQuality)
            {
                case 0:
                    return 0.1;
                case 1:
                    return 1.0;
                case 2:
                    return 2.5;
                case 3:
                    return 4.5;
                case 4:
                    return 7;
                case 5:
                    return 10;
            }

            throw new ArgumentException("Quality may be in range {0,1,2,3,4,5}");
        }

        public double CalculateMilitaryRankModifier(double militaryRank)
        {
            MilitaryRankEnum rank = MilitaryRankEnumExtensions.GetRankForMilitaryRank(militaryRank);

            return 1.0 + (double)rank * 0.02;

        }

        public virtual TransactionResult MakeBattleStartTransaction(War war, Country attacker, double goldAmount)
        {
            var attEntity = attacker.Entity;
            var money = new Money()
            {
                Amount = (decimal)goldAmount,
                Currency = Persistent.Currencies.GetById((int)CurrencyTypeEnum.Gold)
            };

            var transaction = new structs.Transaction()
            {
                Arg1 = "Start Battle - attacker cost",
                Arg2 = string.Format("{0}({1}) paid for creating batle in war #{2}", attEntity.Name, attEntity.EntityID, war.ID),
                Money = money,
                SourceEntityID = attacker.ID,
                TransactionType = TransactionTypeEnum.StartBattle
            };

            return transactionService.MakeTransaction(transaction, useSqlTransaction: false);
        }
    }
}
