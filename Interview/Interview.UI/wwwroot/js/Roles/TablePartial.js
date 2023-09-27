$(document).ready(function () {
    table.Init();
});

var table = {

    Table: $("#userSettings")[0],
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