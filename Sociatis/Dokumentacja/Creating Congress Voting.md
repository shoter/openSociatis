# Creating new congress voting

## Step 1
### Add new voting type.

~~Firstly add it to dbo.**VotingTypes**~~. <- it's now created automatically. no need for that

Complete **VotingTypeEnum** with new enum.

## Step 2
### Add new model which will handle this voting in services.

namespace : **WebServices.structs.Votings**

Example :

	public class ChangeVatCongressVotingParameters : StartCongressVotingParameters
    {
        public double NewVat { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeVat;

        public  ChangeVatCongressVotingParameters(double newVat)
        {
            this.NewVat = newVat;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = NewVat.ToString();
        }
    }

## Step 3
### Create ViewModel which represents the start voting sequence.

ViewModel need to be in **Sociatis.Models.Congress** namespace.

Example :

	public class ChangeVatViewModel : CongressVotingViewModel
    {
        [DisplayName("New VAT")]
        [Range(0, 1000)]
        public double NewVat { get; set; }



        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeVat;

        public ChangeVatViewModel() { }
        public ChangeVatViewModel(int countryID) :base(countryID)
        {
        }
        public ChangeVatViewModel(CongressVoting voting)
            :base(voting)
        {
        }

        public ChangeVatViewModel(FormCollection values)
            :base(values)
        {
        }


        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeVatCongressVotingParameters(NewVat);
        }
    }	


## Step 4
### Add this view model to CongressVotingViewModelChooser

	Add<ChangeVatViewModel>();

## Step 5
### Add new editor template for start voting. It's name must be the same as view model created in step 3

example :

	@model Sociatis.Models.Congress.ChangeVatViewModel

	@{
	    ViewData.TemplateInfo.HtmlFieldPrefix = "";
	}
	
	@Html.LabelFor(m => m.NewVat)
	@Html.TextBoxFor(m => m.NewVat)
	@Html.ValidationMessageFor(m => m.NewVat)

## Step 6
### Create viewmodel for displaying current voting

It need to be created in **Sociatis.Models.Congress.Votings** namespace.

example 

	public class ViewChangeMinimalWageViewModel : ViewVotingBaseViewModel
    {
        public double NewMinimalWage { get; set; }

        public override VotingTypeEnum VotingType => VotingTypeEnum.ChangeMinimalWage;

        public ViewChangeMinimalWageViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
        : base(voting, isPlayerCongressman, canVote)
        {
            NewMinimalWage = double.Parse(voting.Argument1);
        }


    }

## Step 7
### Add ViewModel to  CongressViewVotingViewModelChooser

Example :

	add<ViewChangeMinimalWageViewModel>();

## Step 8
### Create view for displaying this view model in Display Templates with the same name as VM created in step 6

Example 

	@model Sociatis.Models.Congress.Votings.ViewChangeVatViewModel

	<h4>Proposition #@Model.VotingID : Change vat to <b>@Model.NewVat</b>%</h4>
	by @Html.ActionLink(Model.CreatorName, "View", "Citizen", new { citizenID = Model.CreatorID })

## Step 9
### Create operation done after completing voting in CongressVotingService

method **FinishVoting**



example :

	case VotingTypeEnum.ChangeVat:
                    {
                        voting.Country.CountryPolicy.VAT = (decimal)(double.Parse(voting.Argument1) / 100.0);
                        break;
                    }
## Step 10
### Update ActualLawViewModel