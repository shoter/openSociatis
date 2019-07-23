$(function () {

    var refreshTable = function () {
        $('#overviewTable').DataTable().draw();
    };

    $("#currencyID").change(refreshTable);

    var dt = $('#overviewTable').dataTable({
        serverSide: true,
        searching:false,
        "ajax": {
            "url": "OverviewAjax",
            "type": "POST",
            data: function (data) {
                var currencyID = $("#currencyID").val();
                if (currencyID)
                    data.currencyID = currencyID;
            }
        },
        columnDefs: [{
            targets: 3,
            render: function (data, type, row) {
                var day = data;
                var companyID = $("#companyID").val();

                var href = "/Company/" + companyID +  "/Finances/day-" + day + "/Summary";
                var $a = $("<a>", { href: href });
                var $i = $("<i>", { class: "fa fa-dot-circle-o" });
                $a.append($i);
                $a.append(" Details");
               
                return $a[0].outerHTML;
            },
        }],
        order: [[0, "desc"]],
        columns: [
            {
                name: 'day',
                data: "day",
                title: "Date",
                sortable: true,
                searchable: false
            },
            {
                name: 'currencySymbol',
                data: "currencySymbol",
                title: "Currency",
                sortable: false,
                searchable: false
            },
            {
                name: 'total',
                data: "total",
                title: "Balance",
                sortable: false,
                searchable: false
            },

            {
                name: 'details',
                title: "Details",
                data: "day",
                sortable: false,
                searchable: false
            }
        ]
    });

});