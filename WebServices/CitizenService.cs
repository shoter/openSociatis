using Common;
using Common.EncoDeco;
using Common.Language;
using Common.Transactions;
using Entities;
using Entities.enums;
using Entities.Repository;
using HeyRed.MarkdownSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weber.Html;
using WebServices.BigParams.messages;
using WebServices.enums;
using WebServices.Helpers;
using WebServices.Html;
using WebServices.structs;

using IsolationLevel = System.Transactions.IsolationLevel;

namespace WebServices
{
    public class CitizenService : BaseService, ICitizenService
    {
        ICitizenRepository citizenRepository;
        ICountryRepository countriesRepository;
        Entities.Repository.IWalletRepository walletsRepository;
        IEntityService entitiesService;
        IConfigurationRepository configurationRepository;
        ITransactionsService transactionService;
        IWarningService warningService;
        IPopupService popupService;
        IWalletService walletService;
        IMessageService messageService;
        IEquipmentService equipmentService;

        public CitizenService(ICountryRepository countriesRepository, Entities.Repository.IWalletRepository walletsRepository, ICitizenRepository citizensRepository,
            IEntityService entitiesService, IConfigurationRepository configurationRepository, ITransactionsService transactionService, IWarningService warningService,
            IPopupService popupService, IWalletService walletService, IMessageService messageService, IEquipmentService equipmentService)
        {
            this.countriesRepository = countriesRepository;
            this.walletsRepository = walletsRepository;
            this.entitiesService = entitiesService;
            this.configurationRepository = configurationRepository;
            this.citizenRepository = citizensRepository;
            this.transactionService = Attach(transactionService);
            this.warningService = Attach(warningService);
            this.popupService = Attach(popupService);
            this.walletService = walletService;
            this.messageService = messageService;
            this.equipmentService = equipmentService;
        }

        /// <summary>
        /// Decides wheter to give medal to the citizen on sold article. +1 to sold articles for citizen
        /// </summary>
        /// <param name="citizen"></param>
        public void OnSoldArticle(Citizen citizen)
        {
            citizen.SoldArticles += 1;
            if(isEligibleForSuperJournalistMedal(citizen))
                ReceiveSuperJournalist(citizen);

        }

        private static bool isEligibleForSuperJournalistMedal(Citizen citizen)
        {
            for (int i = 2; ; ++i)
            {
                int fib = Utils.Fibbonaci(i) * 100;
                if (fib == citizen.SoldArticles)
                {
                    return true;
                }
                if (fib > citizen.SoldArticles)
                    return false;
            }
        }

        public int CalculateExperienceForLevel(int level)
        {
            int exp = 0;
            int inc = 2;
            int acc = 1;
            double accOfAcc = 0;

            for (int i = 1; i < level; ++i)
            {
                exp += inc;
                inc += acc;
                acc += (int)accOfAcc + 1;
                accOfAcc += 0.3;
            }

            return exp;
        }

        public int CalculateExperienceForNextLevel(int level)
        {
            return CalculateExperienceForLevel(level + 1);
        }

       

        public int CalculateExperienceForNextLevel(Citizen citizen)
        {
            return CalculateExperienceForLevel(citizen.ExperienceLevel + 1);
        }

        public double ReceiveHardWorker(Citizen citizen)
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                citizen.HardWorkerMedals += 1;

                double goldReceived = 5;

                if (citizen.DayWorkedRow > 365)
                    goldReceived += 10;

                ReceiveGoldForMedal(citizen, goldReceived, medalName: "Hard worker");

                using (NoSaveChanges)
                    warningService.AddWarning(citizen.ID, $"You received hard worker medal and {goldReceived} gold.");

                trs?.Complete();
                return goldReceived;
            }
        }

        public virtual void ReceiveSuperJournalist(Citizen citizen)
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                citizen.SuperJournalistMedals += 1;

                double goldReceived = 5;

                if (citizen.SoldArticles > 10_000)
                    goldReceived = 10;

                ReceiveGoldForMedal(citizen, goldReceived, medalName: "Super journalist");
                trs?.Complete();
            }
        }

        public void ReceiveCongressMedal(Citizen citizen, double gold)
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                citizen.CongressMedals += 1;

                using (NoSaveChanges)
                    warningService.AddWarning(citizen.ID, $"You received congress medal and {gold} gold.");

                ReceiveGoldForMedal(citizen, amount: gold, medalName : "congress");
                trs?.Complete();
            }
        }

        public void GrantExperience(Citizen citizen, int amount)
        {
            citizen.Experience += amount;
            var nextLevelExp = CalculateExperienceForNextLevel(citizen);

            if (citizen.Experience >= nextLevelExp)
            {
                citizen.Experience -= nextLevelExp;
                citizen.ExperienceLevel += 1;
                popupService.AddInfo($"Level up! Your actual level is {citizen.ExperienceLevel}");
            }

            ConditionalSaveChanges(citizenRepository);
        }

        public void TryToReceiveSuperSoldierMedal(Citizen citizen)
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                citizen.SuperSoldierMedals += 1;
                double goldReceived = 5;

                if (citizen.Strength >= 8)
                    goldReceived = 10;

                using (NoSaveChanges)
                    warningService.AddWarning(citizen.ID, $"You received super soldier medal and {goldReceived} gold.");

                ReceiveGoldForMedal(citizen, goldReceived, medalName: "Super Soldier");
                trs?.Complete();
            }
        }

        public void ReceiveBattleHeroMedal(Citizen citizen)
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                citizen.BattleHeroMedals += 1;

                ReceiveGoldForMedal(citizen, amount: 5, medalName: "Battle Hero");
                using (NoSaveChanges)
                    warningService.AddWarning(citizen.ID, $"You received battle hero medal and 5 gold.");

                trs?.Complete();
            }
        }

        public void ReceiveWarHeroMedal(Citizen citizen)
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                citizen.WarHeroMedals += 1;

                ReceiveGoldForMedal(citizen, amount: 10, medalName: "War Hero");

                using (NoSaveChanges)
                    warningService.AddWarning(citizen.ID, $"You received war hero medal and 10 gold.");

                trs?.Complete();
            }
        }

        public void ReceiveRessistanceHeroMedal(Citizen citizen, War war, int battleWonCount, double goldAmount)
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {

                citizen.RessistanceHeroMedals += 1;
                

                var warLink = WarLinkCreator.Create(war);
                using (NoSaveChanges)
                    warningService.AddWarning(citizen.ID, $"{warLink} has been won and you received {goldAmount} gold for winning {battleWonCount} battle{PluralHelper.S(battleWonCount)}.");

                ReceiveGoldForMedal(citizen, amount: goldAmount, medalName: "Battle hero");

                trs?.Complete();
            }
        }

        public void ReceivePresidentMedal(Citizen citizen, double gold)
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                citizen.PresidentMedals += 1;

                using (NoSaveChanges)
                    warningService.AddWarning(citizen.ID, $"You received president medal and {gold} gold.");

                ReceiveGoldForMedal(citizen, amount: gold, medalName: "President");
                trs?.Complete();
            }
        }

        public void SetPassword(Citizen citizen, string newPassword)
        {
            citizen.Password = SHA256.Encode(newPassword);
            ConditionalSaveChanges(citizenRepository);
        }


        public Citizen CreateCitizen(RegisterInfo info)
        {
            var entity = entitiesService.CreateEntity(info.Name, EntityTypeEnum.Citizen);
            entity.Equipment.ItemCapacity = 25;
            var country = countriesRepository.GetById(info.CountryID);

            Citizen citizen = new Citizen()
            {
                BirthDay = configurationRepository.GetCurrentDay(),
                CreationDate = DateTime.Now,
                CitizenshipID = info.CountryID,
                Email = info.Email,
                RegionID = info.RegionID,
                Verified = true,
                PlayerTypeID = (int)info.PlayerType,
                ID = entity.EntityID,
                HitPoints = 100
            };

            using (NoSaveChanges)
                SetPassword(citizen, info.Password);

            var currency = Persistent.Countries.GetCountryCurrency(country);

            walletService.AddMoney(entity.WalletID, new Money(currency, 50));
            walletService.AddMoney(entity.WalletID, new Money(GameHelper.Gold, 5));
            equipmentService.GiveItem(ProductTypeEnum.Bread, 5, 1, entity.Equipment);


            citizenRepository.Add(citizen);
            citizenRepository.SaveChanges();

           
            Money citizenFee = new Money(Persistent.Currencies.GetById(country.CurrencyID), country.CountryPolicy.CitizenFee);

            if (walletService.HaveMoney(country.Entity.WalletID, citizenFee))
            {
                Transaction feeTransaction = new Transaction()
                {
                    Arg1 = "Citizen Fee",
                    Arg2 = info.Name,
                    DestinationEntityID = citizen.ID,
                    Money = citizenFee,
                    SourceEntityID = country.ID,
                    TransactionType = TransactionTypeEnum.CitizenFee
                };
                transactionService.MakeTransaction(feeTransaction);
            }
            else
            {
                string citMessage = "Your country did not have enough money to give you birth starting money.";
                warningService.AddWarning(citizen.ID, citMessage);

                var citLink = EntityLinkCreator.Create(citizen.Entity);
                string countryMessage = $"You did not have enough money to give birth starting money to {citLink}.";
                warningService.AddWarning(country.ID, countryMessage);
            }
            string welcomeMessage = country.GreetingMessage;



            var thread = messageService.CreateNewThread(new List<int> { citizen.ID, country.ID }, "Welcome message");
            var smp = new SendMessageParams()
            {
                AuthorID = country.ID,
                Content = welcomeMessage,
                Date = DateTime.Now,
                Day = GameHelper.CurrentDay,
                ThreadID = thread.ID
            };
            messageService.SendMessage(smp);

            return citizen;
        }

        public void IncreaseSkill(int citizenID, WorkTypeEnum workType, double amount)
        {
            var citizen = citizenRepository.GetById(citizenID);
            switch(workType)
            {
                case WorkTypeEnum.Construction:
                    {
                        citizen.Construction += (decimal)amount;
                        break;
                    }
                case WorkTypeEnum.Manufacturing:
                    {
                        citizen.Manufacturing += (decimal)amount;
                        break;
                    }
                case WorkTypeEnum.Raw:
                    {
                        citizen.Raw += (decimal)amount;
                        break;
                    }
                case WorkTypeEnum.Selling:
                    {
                        citizen.Selling += (decimal)amount;
                        break;
                    }
            }
            citizenRepository.SaveChanges();
        }

        public double GetSkillIncreaseForWork(int citizenID, WorkTypeEnum workType)
        {
            var citizen = citizenRepository.GetById(citizenID);

            double currentSkill = GetCurrentSkill(citizen, workType);

            double skillIncrease = calculateSkillIncrease(currentSkill);

            return Math.Round(skillIncrease, 4);
        }

        private static double GetCurrentSkill(Citizen citizen, WorkTypeEnum workType)
        {
            double currentSkill = 0;

            switch (workType)
            {
                case WorkTypeEnum.Construction:
                    {
                        currentSkill = (double)citizen.Construction;
                        break;
                    }
                case WorkTypeEnum.Manufacturing:
                    {
                        currentSkill = (double)citizen.Manufacturing;
                        break;
                    }
                case WorkTypeEnum.Raw:
                    {
                        currentSkill = (double)citizen.Raw;
                        break;
                    }
                case WorkTypeEnum.Selling:
                    {
                        currentSkill = (double)citizen.Selling;
                        break;
                    }
            }

            return currentSkill;
        }

        public double Train(Citizen citizen)
        {
            int oldStrength = (int)citizen.Strength;
            double strengthGain = calculateStrengthIncrease((double)citizen.Strength);
            citizen.Strength += (decimal)strengthGain;
            int newStrength = (int)citizen.Strength;

            if(oldStrength != newStrength)
            {
                TryToReceiveSuperSoldierMedal(citizen);
            }

            citizen.Trained = true;
            citizen.HitPoints -= 1;
            GrantExperience(citizen, 1);
            citizen.LastActivityDay = GameHelper.CurrentDay;
            ConditionalSaveChanges(citizenRepository);
            return strengthGain;
        }

        public static double calculateStrengthIncrease(double currentStrength)
        {
            double A = 5.117435297;
            double B = 1.49897E-05;
            double C = 19.41608989;
            double D = 108098.9343;
            double E = 4.68E+00;
            double F = 1.070000161;
            double a = 1.12451E-05;
            double b = 0.000250554;
            double c = 9.34872E-06;

            double gain = D / Math.Pow((Math.Pow(currentStrength, F) + a * Math.Pow(currentStrength, F) + b * Math.Pow(currentStrength, A) + c * Math.Pow(currentStrength, B) + C), E);

            return Math.Max(Math.Round(gain, 4), 0.0001);
        }

        public static double calculateSkillIncrease(double currentSkill)
        {
            double A = 5.117435297;
            double B = 1.49897E-05;
            double C = 19.41608989;
            double D = 108098.9343;
            double E = 4.68E+00;
            double F = 1.070000161;
            double a = 1.12451E-05;
            double b = 0.000250554;
            double c = 9.34872E-06;

            double gain = D / Math.Pow((Math.Pow(currentSkill, F) + a * Math.Pow(currentSkill, F) + b * Math.Pow(currentSkill, A) + c * Math.Pow(currentSkill, B) + C), E);

            return Math.Max(Math.Round(gain, 4), 0.0001);

        }


        public TransactionResult ReceiveGoldForMedal(Citizen citizen, double amount, string medalName)
        {
            var adminCurrency = Persistent.Currencies.Gold;
            var entity = citizen.Entity;

            var adminMoney = new Money()
            {
                Amount = (decimal)amount,
                Currency = adminCurrency
            };


            var adminTransaction = new Transaction()
            {
                Arg1 = "Medal reward",
                Arg2 = string.Format("{0}({1}) received gold for {2}", entity.Name, entity.EntityID, medalName),
                DestinationEntityID = entity.EntityID,
                Money = adminMoney,
                TransactionType = TransactionTypeEnum.MedalReward
            };

            return transactionService.MakeTransaction(adminTransaction, useSqlTransaction: false);
        }

        public int GetActivePlayerCount()
        {
            return citizenRepository.GetActivePlayerCount(GameHelper.CurrentDay);
        }

        
    }
}
