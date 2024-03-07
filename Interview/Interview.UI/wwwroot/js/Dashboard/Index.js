﻿var table = {

    Table: $("#tblResults")[0],
    Url: "/Dashboard/GetResults",
    ProcessId: $("#ProcessId")[0],
    PeriodOfTimeType: $("#PeriodOfTimeType")[0],
    StartDate: $("#startdate")[0],
    EndDate: $("#enddate")[0],

    Init: function () {

        this.InitTable();

    },

    InitTable: function () {

        $(this.Table).DataTable({
            // Design Assets
            stateSave: true,
            autoWidth: true,
            // ServerSide Setups
            processing: true,
            serverSide: true,
            // Paging Setups
            paging: true,
            lengthMenu: [[5, 10, 25, 50, 100, -1], [5, 10, 25, 50, 100, "All"]],
            pageLength: 10,
            pagingType: "full_numbers",
            // Custom Export Buttons
            dom: 'lBfrtip',
            searching: { regex: true },
            ajax: {
                url: table.Url,
                type: "POST",
                contentType: "application/json",
                dataType: "json",
                data: function (d) {

                    var formFilter = {
                        ProcessId: $(table.ProcessId).val(),
                        PeriodOfTimeType: $(table.PeriodOfTimeType).val(),
                        StartDate: $(table.StartDate).val(),
                        EndDate: $(table.EndDate).val(),
                    };
                    d.formfilter = JSON.stringify(formFilter);

                    return JSON.stringify(d);
                }
            },
            columns: [
                { data: "dates" },
                { data: "numberslots" },
                { data: "progress" },
                { data: "numberprogresscompleted" },
                { data: "numbercandidateinslots" },
                { data: "numbervirtuals" },
                { data: "nnumberdaysofinterview" },
                { data: "eecandidates" }
            ],
            //columnDefs: [
            //    { targets: "no-sort", orderable: false },
            //    { targets: "no-search", searchable: false },
            //    //{
            //    //    targets: "trim",
            //    //    render: function (data, type, full, meta) {
            //    //        if (type === "display") {
            //    //            data = strtrunc(data, 10);
            //    //        }

            //    //        return data;
            //    //    }
            //    //},
            //    //{ targets: "date-type", type: "date-eu" },
            //    //{
            //    //    targets: 10,
            //    //    data: null,
            //    //    defaultContent: "<a class='btn btn-link' role='button' href='#' onclick='edit(this)'>Edit</a>",
            //    //    orderable: false
            //    //},
            //]
        });

    },

}

if (wb.isReady) {
    table.Init();
}
else
    $(document).on("wb-ready.wb", function (event) {
        table.Init();
    });