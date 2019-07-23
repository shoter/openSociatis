window.Sociatis = window.Sociatis || {};

window.Sociatis.Issues = (function () {

    $(function () {

        $("#unresolvedCheckbox").change(() => {
            $('table#isueesTable').DataTable()

                .draw();
        });

        $('table#isueesTable').dataTable({
            serverSide: true,
            searching: false,
            ordering: true,
            "ajax": {
                "url": "/DevIssue/IssuesAjax",
                "type": "POST",
                data: function (data) {
                    data.onlyUnresolved = $("#unresolvedCheckbox").is(":checked");
                }
            },
            columnDefs: [{
                targets: 0,
                render: function (data, type, row) {
                    var name = data;
                    var id = row.data.creatorID;

                    var href = "/Entity/" + id;
                    var $html = $("<a>", { href: href });
                    $html.text(name);
                    return $html[0].outerHTML;
                },
            }, {
                targets: 1,
                render: function (data, type, row) {
                    var name = data;
                    var id = row.data.id;

                    var href = "/DevIssue/" + id;
                    var $html = $("<h6>");
                    var $a = $("<a>", { href: href });
                    $a.text(name);
                    $html.append($a);
                    for (var label of row.labels) {
                        var labelName = label.name;
                        var labelClass = "sociatisLabel " + label.classname;

                        var $label = $("<span>", { class: labelClass });
                        $label.text(labelName);
                        $html.append($label);
                    }

                    return $html[0].outerHTML;
                }
            }, {
                targets: 3,
                render: function (data, type, row) {
                    return "day " + data;
                }
            }, {
                targets: 4,
                render: function (data, type, row) {
                    return "day " + data;
                }
            }],
            columns: [
                {
                    name: 'creatorName',
                    data: "data.creatorName",
                    title: "Author",
                    sortable: false,
                    searchable: false
                },
                {
                    name: 'name',
                    data: 'data.name',
                    title: "Name",
                    sortable: false,
                    searchable: false
                },
                {
                    name: 'votes',
                    data: "data.votes",
                    title: "Votes",
                    sortable: true,
                    searchable: false
                },
                {
                    name: 'day',
                    data: "data.day",
                    title: "Creation Day",
                    sortable: true,
                    searchable: false
                }
                ,
                {
                    name: 'lastActivity',
                    data: "data.lastActivity",
                    title: "Last Activity",
                    sortable: true,
                    searchable: false
                }
            ]
        });
    });

})();