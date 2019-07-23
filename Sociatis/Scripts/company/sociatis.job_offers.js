$(function () {



});

function showContract()
{
    $("#contractDialog").dialog(
        {
            title: "Contract details",
            resizable: true,
            modal: true,
            fluid: true,

            dialogClass: "contractDialog"
        }
        );

    $("#contractDialog").dialog("open");
}

function closeContractWindow()
{
    $("#contractDialog").dialog("close");
}