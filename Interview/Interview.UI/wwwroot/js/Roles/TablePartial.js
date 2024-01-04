var table = {

    Table: $("#roleUsers")[0],
    EditForm: $("#editForm")[0],
    ASubmit: $("#aSubmit")[0],

    Init: function () {

        this.InitTable();

        $(this.ASubmit).on('click', table.PostEditedRow);

    },

    InitTable: function () {

        $(this.Table).DataTable();

    },

    PostEditedRow: function (e) {

        e.preventDefault();
        $(table.EditForm).submit();

    }

}

if (wb.isReady) {
    table.Init();
}
else
    $(document).on("wb-ready.wb", function (event) {
        table.Init();
    });