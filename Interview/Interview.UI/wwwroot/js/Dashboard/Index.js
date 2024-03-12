var table = {

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
                {
                    data: function (row, type, val, meta) {
                        return row;
                    },
                    render: function (data, type, full, meta) {
                        return "<div>" + Resources.Dashboard.Completed + ": " + data.numberprogresscompleted + "</div>" +
                            "<div>" + Resources.Dashboard.Remaining + ": " + data.numberprogressremaining + "</div>";
                    },
                },
                {
                    data: function (row, type, val, meta) {
                        return row;
                    },
                    render: function (data, type, full, meta) {
                        return "<div>" + Resources.Dashboard.InTimeSlot + ": " + data.numbercandidateinslots + "</div>" +
                            "<div>" + Resources.Dashboard.NotInTimeSlot + ": " + data.numbercandidatenotinslots + "</div>";
                    },
                },
                {
                    data: function (row, type, val, meta) {
                        return row;
                    },
                    render: function (data, type, full, meta) {
                        return "<div>" + Resources.Dashboard.Virtual + ": " + data.numbervirtuals + "</div>" +
                            "<div>" + Resources.Dashboard.InPerson + ": " + data.numberinpersons + "</div>";
                    },
                },
                { data: "numberdaysofinterview" },
                {
                    data: function (row, type, val, meta) {
                        return row;
                    },
                    render: function (data, type, full, meta) {

                        var result = "";

                        $.each(data.eecandidates, function (key, value) {
                            result = result + "<div>" + value.name + ": " + value.count + "</div>";
                        });

                        return result;

                    },
                },
                {
                    data: function (row, type, val, meta) {
                        return row;
                    },
                    render: function (data, type, full, meta) {

                        var result = "";

                        $.each(data.eeboardmembers, function (key, value) {
                            result = result + "<div>" + value.name + ": " + value.count + "</div>";
                        });

                        return result;

                    },
                },
            ],
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