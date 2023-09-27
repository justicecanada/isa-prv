$(document).ready(function () {
    table.Init();
});

var table = {

    Table: $("#userSettings")[0],

    Init: function () {

        this.InitTable();

    },

    InitTable: function () {

        $(this.Table).DataTable();

    }

}