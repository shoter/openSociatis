$(function () {
    $("#selectAll").change(function () {
        var $this = $(this);
        var value = $this.is(':checked');
        var $checkboxes = $("#warningsTBody input[type='checkbox']");
        $checkboxes.prop({ checked: value });
    });

    $('input[name$="WarningID"]').change(function () {
        var $this = $(this);
        var value = $this.val();

        if ($this.is(':checked') == false) {
            $("#selectAll").prop({ checked: false });
        }
    });

});