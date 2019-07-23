#define AllowAll
using Common;
using Common.Operations;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using Entities.structs;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.Helpers;
using WebServices.structs;
using WebUtils.Attributes;

namespace Sociatis.Controllers
{
    public class GameController : ControllerBase
    {
        IWorldService worldService;
        IWalletService walletService;
        ICurrencyRepository currencyRepository;
        IDebugDayChangeRepository debugDayChangeRepository;
        ICitizenRepository citizenRepository;
        IWarService warService;
        IEntityRepository entityRepository;
        IBattleRepository battleRepository;
        IBattleService battleService;


        public GameController(IWorldService worldService, IWalletService walletService, ICurrencyRepository currencyRepository, IDebugDayChangeRepository debugDayChangeRepository
            , IPopupService popupService, ICitizenRepository citizenRepository, IWarService warService, IEntityRepository entityRepository, 
            IBattleService battleService, IBattleRepository battleRepository) : base(popupService)
        {
            this.worldService = worldService;
            this.walletService = walletService;
            this.currencyRepository = currencyRepository;
            this.debugDayChangeRepository = debugDayChangeRepository;
            this.citizenRepository = citizenRepository;
            this.warService = warService;
            this.entityRepository = entityRepository;
            this.battleRepository = battleRepository;
            this.battleService = battleService;
        }
        // GET: Game
#if DEBUG
        [SociatisAuthorize(PlayerTypeEnum.Player)]
#else
        [SociatisAuthorize(PlayerTypeEnum.Admin)]
#endif
        public ActionResult DayChange()
        {
            worldService.ProcessDayChange();
            return View();
            
        }

        [SociatisAuthorize(PlayerTypeEnum.Admin)]
        public ActionResult EndBattle(int battleID)
        {
            var battle = battleRepository.GetById(battleID);
            battleService.EndBattle(battle);

            return RedirectBack();
        }

        [SociatisAuthorize(PlayerTypeEnum.Admin)]
        public ActionResult StartTraining()
        {
            warService.TryToCreateTrainingWar();
            return RedirectToAction("Index", "Home");
        }

        [IsLocal]
        public ActionResult DayChangeServer()
        {
            debugDayChangeRepository.Add(new Entities.DebugDayChanx()
            {
                DayChangeTime = DateTime.Now
            });
            debugDayChangeRepository.SaveChanges();

            if (worldService.CanChangeDay())
                worldService.ProcessDayChange();
            return Content("");
        }

#if DEBUG || AllowAll
        [SociatisAuthorize(PlayerTypeEnum.Player)]
#else
        [SociatisAuthorize(PlayerTypeEnum.Admin)]
#endif
        public ContentResult Test()
        {
            ICountryRepository countryRepository = DependencyResolver.Current.GetService<ICountryRepository>();
            var enemyCountries = countryRepository.GetEnemyCountries(1);

            return Content("ASD");
        }
#if DEBUG || AllowAll
        [SociatisAuthorize(PlayerTypeEnum.Player)]
#else
        [SociatisAuthorize(PlayerTypeEnum.Admin)]
#endif

        public ActionResult GiveMeVoting()
        {
            var citizen = citizenRepository.GetById(SessionHelper.LoggedCitizen.ID);

            foreach(var cong in citizen.Congressmen.ToList())
                cong.LastVotingDay = GameHelper.CurrentDay - 1;

            citizenRepository.SaveChanges();
            return RedirectToHome();
        }

#if DEBUG || AllowAll
        [SociatisAuthorize(PlayerTypeEnum.Player)]
#else
        [SociatisAuthorize(PlayerTypeEnum.Admin)]
#endif
        public ActionResult IWantWorkAgain()
        {
            var citizen = citizenRepository.GetById(SessionHelper.LoggedCitizen.ID);

            citizen.Worked = false;

            citizenRepository.SaveChanges();
            return RedirectToHome();
        }

#if DEBUG || AllowAll
        [SociatisAuthorize(PlayerTypeEnum.Player)]
#else
        [SociatisAuthorize(PlayerTypeEnum.Admin)]
#endif
        public ActionResult IWantTrainAgain()
        {
            var citizen = citizenRepository.GetById(SessionHelper.LoggedCitizen.ID);

            citizen.Trained = false;

            citizenRepository.SaveChanges();
            return RedirectToHome();
        }

        [SociatisAuthorize(PlayerTypeEnum.Admin)]
        public ActionResult GiveMeMoney()
        {
            var entity = SessionHelper.CurrentEntity;
            var wallet = entity.Wallet;
            var gold = currencyRepository.Gold;

            foreach (var currency in Persistent.Currencies.GetAll())
            {
                var countryMoney = new Money()
                {
                    Amount = 100,
                    Currency = currency
                };
                walletService.AddMoney(wallet.ID, countryMoney);
            }
            var adminMoney = new Money()
            {
                Amount = 100,
                Currency = gold
            };


           

            
            walletService.AddMoney(wallet.ID, adminMoney);

            return RedirectToHome();
        }
#if DEBUG || AllowAll
        [SociatisAuthorize(PlayerTypeEnum.Player)]
#else
        [SociatisAuthorize(PlayerTypeEnum.Admin)]
#endif
        public ActionResult GiveProduct(int productTypeID, int quality, int amount)
        {
            var entity = SessionHelper.CurrentEntity;
            var equipmentService = DependencyResolver.Current.GetService<IEquipmentService>();
            var productType = (ProductTypeEnum)productTypeID;
            var equipment = entity.Equipment;

            MethodResult result;
            if ((result = equipmentService.CanGiveItem(equipment, productType, amount, quality)).IsError)
                return RedirectToHomeWithError(result);

            equipmentService.GiveItem(productType, amount, quality, equipment);


            return RedirectToHome();
        }
#if DEBUG || AllowAll
        [SociatisAuthorize(PlayerTypeEnum.Player)]
#else
        [SociatisAuthorize(PlayerTypeEnum.Admin)]
#endif
        public ActionResult GiveMeHealth()
        {
            ICitizenRepository citizenRepository = DependencyResolver.Current.GetService<ICitizenRepository>();

            var citizen = citizenRepository.GetById(SessionHelper.LoggedCitizen.ID);

            citizen.HitPoints = 100;
            citizen.Worked = false;

            citizenRepository.SaveChanges();

            return RedirectToHome();
        }
#if DEBUG || AllowAll
        [SociatisAuthorize(PlayerTypeEnum.Player)]
#else
        [SociatisAuthorize(PlayerTypeEnum.Admin)]
#endif
        public ActionResult Info()
        {
            var entity = SessionHelper.CurrentEntity;

            var vm = new EntityViewModel(entity);

            return View(vm);
        }
        [SociatisAuthorize(PlayerTypeEnum.Admin)]
        public ActionResult SetMyHealth(int hp)
        {
            var citizen = citizenRepository.GetById(SessionHelper.LoggedCitizen.ID);
            citizen.HitPoints = hp;
            citizen.UsedHospital = false;
            citizenRepository.SaveChanges();
            return RedirectToHome();
        }

        public ActionResult SwitchStack()
        {
            var stack = SessionHelper.SwitchStack;

            List<EntityDom> domek = new List<EntityDom>();
            for(int i = 0;i < stack.Count;++i)
            {
                domek.Add(stack.ElementAt(i));
            }

            return View(domek);
        }

        [SociatisAuthorize(PlayerTypeEnum.Admin)]
        public ActionResult AddMeMoneySuka(int moneySukaID, int amount)
        {
            var entity = SessionHelper.CurrentEntity;


            var countryMoney = new Money()
            {
                Amount = amount,
                Currency = Persistent.Currencies.GetById(moneySukaID)
            };



            var wallet = entity.Wallet;

            walletService.AddMoney(wallet.ID, countryMoney);
            return RedirectBack();
        }

        [SociatisAuthorize(PlayerTypeEnum.Admin)]
        public ActionResult ForceSwitch(int entityID)
        {
            EntityController.SwitchInto(entityRepository.GetById(entityID));
            return RedirectBack();
        }

        /* Not working. It would be nice if it worked.
                private string getMyInfo(object what, string prefix, List<object> visited)
                {
                    string info = "";
                    Type type = what.GetType();

                    foreach(PropertyInfo pi in  what.GetType().GetProperties())
                    {
                        if (IsEnumerable(pi.PropertyType))
                            continue;



                        if (Utils.IsSimple(type) || pi.GetValue(what) == null)
                            info += prefix + pi.GetValue(what).ToString() + "<br/>";
                        else
                        {
                            var value = pi.GetValue(what);
                            if (visited.Contains(value))
                                continue;
                            visited.Add(value);
                            info += getMyInfo(value, prefix + "  ", visited);
                        }
                    }

                    return info;
                }

                static bool IsEnumerable(Type type)
                {
                    foreach (Type intType in type.GetInterfaces())
                    {
                        if (intType.IsGenericType
                            && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        {
                            return true;
                        }

                    }
                    return false;
                }*/
    }
}