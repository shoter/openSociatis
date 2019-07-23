using Common.EntityFramework;
using Entities;
using Entities.enums;
using Entities.enums.Attributes;
using Microsoft.Diagnostics.Runtime;
using Sociatis.Code;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using WebServices.enums;
using WebUtils.Forms.Select2;
using WebUtils.Scripts;

namespace Sociatis.App_Start
{
    public class Starter
    {
        public static void Start()
        {
            initializeSelect2();
            initializeEnumators();

            initializeCulture();

            ScriptInjector.SetJavascriptRenderer(new JqueryAfterInitScriptRenderer());
        }

        private static void initializeCulture()
        {
            var culture = CultureInfo.CurrentCulture.Clone() as CultureInfo;
            setDotOnCultureInfo(culture);
            var cultureUI = CultureInfo.CurrentUICulture.Clone() as CultureInfo;
            setDotOnCultureInfo(cultureUI);

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = cultureUI;

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = cultureUI;
        }

        private static CultureInfo setDotOnCultureInfo( CultureInfo customCulture)
        {
            if(customCulture?.NumberFormat?.NumberDecimalSeparator != null)
                customCulture.NumberFormat.NumberDecimalSeparator = ".";

            return customCulture;
        }

        private static void initializeEnumators()
        {
            using (var context = new SociatisEntities())
            {
                var genericRepository = new GenericRepository<SociatisEntities>(context);

                new Enumator<VotingType, VotingTypeEnum>(genericRepository)
                    .CreateNewIfAble(
                    setName: (voting, name) => voting.Name = name,
                    setValue: (voting, value) =>
                    {
                        voting.ID = value;
                        voting.AlwaysVotable = isVotingEnumAlawaysVotable((VotingTypeEnum)value);

                    },
                    getName: votingType => votingType.ToString(),
                    getValue: votingType => (int)votingType,

                    isTheSame: (voting, votingType) => voting.ID == (int)votingType);

                new Enumator<CongressVotingRejectionReason, CongressVotingRejectionReasonEnum>(genericRepository)
                    .CreateNewIfAble(
                    setName: (reason, name) => reason.Name = name,
                    setValue: (reason, value) => reason.ID = value,

                    getName: reasonEnum => reasonEnum.ToString(),
                    getValue: reasonEnum => (int)reasonEnum,

                    isTheSame: (reason, reasonEnum) => reason.ID == (int)reasonEnum);

                new Enumator<TransactionType, TransactionTypeEnum>(genericRepository)
                    .CreateNewIfAble(
                    setName: (transfer, name) => transfer.Name = name,
                    setValue: (transfer, value) => transfer.ID = value,

                    getName: transferEnum => transferEnum.ToString(),
                    getValue: transferEnum => (int)transferEnum,

                    isTheSame: (transfer, transferEnum) => transfer.ID == (int)transferEnum);

                new Enumator<TradeStatus, TradeStatusEnum>(genericRepository)
                    .CreateNewIfAble(
                    setName: (status, name) => status.Name = name,
                    setValue: (status, value) => status.ID = value,

                    getName: statusEnum => statusEnum.ToString(),
                    getValue: statusEnum => (int)statusEnum,

                    isTheSame: (status, statusEnum) => status.ID == (int)statusEnum);

                new Enumator<Product, ProductTypeEnum>(genericRepository)
                    .CreateNewIfAble(
                    setName: (product, name) => product.Name = name,
                    setValue: (product, value) => product.ID = value,

                    getName: productType => productType.ToString(),
                    getValue: productType => (int)productType,

                    isTheSame: (product, productType) => product.ID == (int)productType);

                new Enumator<CompanyType, CompanyTypeEnum>(genericRepository)
                   .CreateNewIfAble(
                   setName: (companyType, name) => companyType.Name = name,
                   setValue: (companyType, value) => companyType.ID = value,

                   getName: companyTypeEnum => companyTypeEnum.ToString(),
                   getValue: companyTypeEnum => (int)companyTypeEnum,

                   isTheSame: (companyType, companyTypeEnum) => companyType.ID == (int)companyTypeEnum);

                new Enumator<DevIssueLabelType, DevIssueLabelTypeEnum>(genericRepository)
                   .CreateNewIfAble(
                   setName: (labelType, name) => labelType.Name = name,
                   setValue: (labelType, value) => labelType.ID = value,

                   getName: labelTypeEnum => labelTypeEnum.ToString(),
                   getValue: labelTypeEnum => (int)labelTypeEnum,

                   isTheSame: (labelType, labelTypeEnum) => labelType.ID == (int)labelTypeEnum);

                new Enumator<VisibilityOption, VisibilityOptionEnum>(genericRepository)
                   .CreateNewIfAble(
                   setName: (visibilityOption, name) => visibilityOption.Name = name,
                   setValue: (visibilityOption, value) => visibilityOption.ID = value,

                   getName: visibilityOptionEnum => visibilityOptionEnum.ToString(),
                   getValue: visibilityOptionEnum => (int)visibilityOptionEnum,

                   isTheSame: (visibilityOption, visibilityOptionEnum) => visibilityOption.ID == (int)visibilityOptionEnum);

                new Enumator<UploadLocation, UploadLocationEnum>(genericRepository).CreateNewIfAble();
                new Enumator<LawAllowHolder, LawAllowHolderEnum>(genericRepository).CreateNewIfAble();

                new Enumator<VotingStatus, VotingStatusEnum>(genericRepository).CreateNewIfAble();
                new Enumator<EntityType, EntityTypeEnum>(genericRepository).CreateNewIfAble();
                new Enumator<PlayerType, PlayerTypeEnum>(genericRepository).CreateNewIfAble(
                   setName: (playerType, name) => playerType.name = name,
                   setValue: (playerType, value) => playerType.ID = value,

                   getName: playerTypeEnum => playerTypeEnum.ToString(),
                   getValue: playerTypeEnum => (int)playerTypeEnum,

                   isTheSame: (playerType, playerTypeEnum) => playerType.ID == (int)playerTypeEnum);

                new Enumator<CompanyType, CompanyTypeEnum>(genericRepository).CreateNewIfAble();
                new Enumator<JobType, JobTypeEnum>(genericRepository).CreateNewIfAble();
                new Enumator<PartyRole, PartyRoleEnum>(genericRepository).CreateNewIfAble();
                new Enumator<JoinMethod, JoinMethodEnum>(genericRepository).CreateNewIfAble();
                new Enumator<Product, ProductTypeEnum>(genericRepository).CreateNewIfAble();
                new Enumator<VotingType, VotingTypeEnum>(genericRepository).CreateNewIfAble();
                new Enumator<ResourceType, ResourceTypeEnum>(genericRepository).CreateNewIfAble(
                   setName: (playerType, name) => playerType.name = name,
                   setValue: (playerType, value) => playerType.ID = value,

                   getName: playerTypeEnum => playerTypeEnum.ToString(),
                   getValue: playerTypeEnum => (int)playerTypeEnum,

                   isTheSame: (playerType, playerTypeEnum) => playerType.ID == (int)playerTypeEnum);

                new Enumator<DevIssueLabelType, DevIssueLabelTypeEnum>(genericRepository).CreateNewIfAble();
                new Enumator<CommentRestriction, CommentRestrictionEnum>(genericRepository).CreateNewIfAble();


                new Enumator<PresidentCandidateStatus, PresidentCandidateStatusEnum>(genericRepository).CreateNewIfAble();

                new Enumator<CongressCandidateStatus, CongressCandidateStatusEnum>(genericRepository).CreateNewIfAble(
                   setName: (playerType, name) => playerType.name = name,
                   setValue: (playerType, value) => playerType.ID = value,

                   getName: playerTypeEnum => playerTypeEnum.ToString(),
                   getValue: playerTypeEnum => (int)playerTypeEnum,

                   isTheSame: (playerType, playerTypeEnum) => playerType.ID == (int)playerTypeEnum);

                new Enumator<VoteType, VoteTypeEnum>(genericRepository).CreateNewIfAble();
                new Enumator<MonetaryOfferType, MonetaryOfferTypeEnum>(genericRepository).CreateNewIfAble();
                new Enumator<ProductTaxType, ProductTaxTypeEnum>(genericRepository).CreateNewIfAble();

                new Enumator<FurnitureType, FurnitureTypeEnum>(genericRepository).CreateNewIfAble();

                new Enumator<MemberStatus, MemberStatusEnum>(genericRepository).CreateNewIfAble();
                new Enumator<MemberStatus, MemberStatusEnum>(genericRepository).CreateNewIfAble();
                new Enumator<WarStatus, WarStatusEnum>(genericRepository).CreateNewIfAble();
                new Enumator<BattleStatus, BattleStatusEnum>(genericRepository).CreateNewIfAble();
                new Enumator<EventType, EventTypeEnum>(genericRepository).CreateNewIfAble();
                new Enumator<CountryEventType, CountryEventTypeEnum>(genericRepository).CreateNewIfAble();
              //  new Enumator<PartyEvent, PartyEventEnum>(genericRepository).CreateNewIfAble();

            }
        }

        private static bool isVotingEnumAlawaysVotable(VotingTypeEnum value)
        {
            var type = typeof(VotingTypeEnum);
            var memInfo = type.GetMember(value.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(AlwaysVotableAttribute), false);
            return ((AlwaysVotableAttribute)attributes[0]).Value;
        }

        private static void initializeSelect2()
        {
            new Select2AjaxDefaultProvider()
            {
                JavascriptFile = "~/bundles/select2",
                StyleFile = "~/Content/select2",
#if DEBUG
                DefaultMinimumInputLength = 1,
#else
                DefaultMinimumInputLength = 2,
#endif
            };
        }
    }
}