var table = {

    Table: $("#roleInterviews")[0],

    Init: function () {

        this.InitTable();

    },

    InitTable: function () {

        $(this.Table).DataTable();

    },

}

if (wb.isReady) {
    table.Init();
}
else
    $(document).on("wb-ready.wb", function (event) {
        table.Init();
    });