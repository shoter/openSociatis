$(function () {
    var createTable = function () {
        dt = $('#battleStatistics table').dataTable({
            serverSide: true,
            "ajax": {
                "url": "/Battle/StatisticsAjax",
                "type": "POST",
                data: function (data) {
                    data.battleID = battleID;
                }
            },
            columnDefs:
            [{
                className: "col_avatar",
                render: function (data, type, row) {
                    return "<img class='smallEntityAvatar' src='" + data + "' />";
                },
                targets: 0
            },
            {
                className: "col_name",
                render: function (data, type, row) {
                    return "<a href='/Citizen/" + row.id + "'>" + data + "</a>";
                },
                targets: 1
            },
            {
                className: "col_side",
                render: function (data, type, row) {
                    return "<img class='tableSmallFlag' src='" + data + "' />";
                },
                targets: 2
            },

            {
                className: "col_dmg",
                render: function (data, type, row) {
                    return data + ' dmg';
                },
                targets: 3
            }
            ],
            order: [[1, "desc"]],
            columns: [
                {
                    name: 'imgUrl',
                    data: "imgUrl",
                    title: "Avatar",
                    sortable: true,
                    searchable: false
                },
                {
                    name: 'name',
                    data: 'name',
                    title: "Name",
                    sortable: true,
                    searchable: true
                },
                {
                    name: 'side',
                    data: "sideImgUrl",
                    title: "Side",
                    sortable: true,
                    searchable: false
                },
                {
                    name: 'dmg',
                    data: "damage",
                    title: "Damage",
                    sortable: true,
                    searchable: false
                }
            ]
        });
    };

    var dt;
    var battleID = $("#battleID").val();
    createTable();

});