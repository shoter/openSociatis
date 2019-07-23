$(function () {
    $("input#JobType").change(
    function(){
        loadHint();
    });

});

function loadHint()
{
    var val = $("input#JobType:checked").val();

    if(val == 1)
    {
        $("#normalDescription").show();
        $("#contractDescription").hide();
    }
    else if(val == 2)
    {
        $("#normalDescription").hide();
        $("#contractDescription").show();
    }
}