$(function () {
    var $salary = $(".salary-slider");
    var minSalary = parseInt($salary.attr("data-min"));
    var maxSalary = parseInt($salary.attr("data-max"));
    

    $salary.slider({
        range: true,
        min: minSalary,
        max: maxSalary,
        values: [minSalary, maxSalary],
        slide: function (event, ui) {
            setSalary(ui.values[0], ui.values[1]);
        }
    });
    setSalary(minSalary, maxSalary);

    var $skill = $(".skill-slider");
    var minSkill = parseInt($skill.attr("data-min"));
    var maxSkill = parseInt($skill.attr("data-max"));


    $skill.slider({
        range: true,
        min: minSkill,
        max: maxSkill,
        step: 0.01,
        values: [minSkill, maxSkill],
        slide: function (event, ui) {
            setSkill(ui.values[0], ui.values[1]);
        }
    });
    setSkill(minSkill, maxSkill);

    $("#CountryID").change(function () {
        updateFilter();
    });
    $("#WorkTypeID").change(function () {
        updateFilter();
    });
});

function updateFilter()
{
    var countryID = $("#CountryID").val();
    var workTypeID = $("#WorkTypeID").val();

    var url = "/JobOffer/JobMarketMinMax";

    Sociatis.AjaxBegin();
    $.post(url, { countryID: countryID, workType: workTypeID }, function (data) {
        Sociatis.HandleJson(data, onFilterUpdate);
        Sociatis.AjaxEnd();
    });
}

function onFilterUpdate(model)
{
    var data = model.Data;

    $(".salary-slider").attr("data-symbol", data.CurrencySymbol);

    var minSalary = parseInt(data.MinimumSalary);
    var maxSalary = parseInt(data.MaximumSalary);

    $('.salary-slider').slider("option", "min", minSalary);
    $('.salary-slider').slider("option", "max", maxSalary);

    var curMin = parseInt($("#MinSalary").val());
    var curMax = parseInt($("#MaxSalary").val());

    if (curMin < minSalary)
        curMin = minSalary;
    else if (curMin > maxSalary)
        curMin = maxSalary;
    if (curMax > maxSalary)
        curMax = maxSalary;
    else if(curMax < minSalary)
        curMax = minSalary;

    $(".salary-slider").slider('values', [curMin, curMax]).change();
    setSalary(curMin, curMax); 

    var minSkill = parseFloat(data.MinimumSkill);
    var maxSkill = parseFloat(data.MaximumSkill);

    $('.skill-slider').slider("option", "min", minSkill);
    $('.skill-slider').slider("option", "max", maxSkill);
    
    curMin = parseFloat($("#MinSkill").val());
    curMax = parseFloat($("#MaxSkill").val());

    if (curMin < minSkill)
        curMin = minSkill;
    else if(curMin > maxSkill)
        curMin = maxSkill;
    if (curMax > maxSkill)
        curMax = maxSkill;
    else if (curMax < minSkill)
        curMax = minSKill;

    $(".skill-slider").slider('values', [curMin, curMax]).change();
    setSkill(curMin, curMax);
}

function setSalary(min, max)
{
    $("#MinSalary").val(min);
    $("#MaxSalary").val(max);

    var symbol = $(".salary-slider").attr("data-symbol");
    $(".salary-slider-text").text(min + " " + symbol + " - " + max + " " + symbol);
}

function setSkill(min, max)
{
    {
        $("#MinSkill").val(min);
        $("#MaxSkill").val(max);

        $(".skill-slider-text").text(min + " - " + max);
    }
}

function loadJobs(model)
{
    var data = model.Data.Offers;


    var table = $('#searchResults').dataTable()

    table.fnClearTable(false /*no redraw*/);
    table.fnAddData(data, true /*redraw enabled*/);
}

function createTable()
{
    return $('#searchResults').DataTable({
        autoWidth: false,
        columns: [
            { "data": "OfferType" },
            { "data": "Salary" },
            { "data": "Skill" },
            { "data": "MinimumHP" },
            { "data": "Length" },
        ],
        columnDefs: [
            { type: "natural", targets: 1 },
            { type: "natural", targets: 4 },
            {
                render: function (data, type, row) {
                    var url = "";
                    if (row[0].toLowerCase == "normal") {
                        url = Sociatis.GetAdress("AcceptOffer", "JobOffer", [{ Name: "offerID", Value: data }]);
                        return '<a href="'+url+'" class="button green">Accept offer</button>'
                    }
                    return '<button class="button green">Details</button>'
                }, targets: 5
            }
        ]
    });
} 