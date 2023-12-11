
var calendar = {

    Data: [],
    Table: null,
    TableSelector: "#tblItems",
    EditClass: "aEdit",

    Init: function () {

        this.Data = $(this.TableSelector).data().tabledata;
        this.InitTable();

    }, 

    InitTable: function () {

        this.Table = $(this.TableSelector).DataTable();
        this.Table.destroy();

        this.Table = $(this.TableSelector).DataTable({
            paging: false,
            ordering: false,
            retrieve: true,
            searching: false,
            info: false,
            columns: [
                {
                    render: function (data, type, full, meta) {

                        var result;

                        result = "<span class='mr-2'><a data-id=" + data.Id + " href='#modalContainer' class='" + calendar.EditClass + "'>Edit</a></span>";
                        //result = "<span><a data-id=" + data.id + " href='#' class='" + ref.Table.DeleteClass + "'>Delete</a></span>";

                        return result;

                    },
                    data: function (row, type, val, meta) {
                        return row;
                    }
                },
                {
                    render: function (data, type, full, meta) {
                        return data.NoProcessus;
                    },
                    data: function (row, type, val, meta) {
                        return row;
                    }
                },
                {
                    render: function (data, type, full, meta) {
                        return data.GroupNiv;
                    },
                    data: function (row, type, val, meta) {
                        return row;
                    }
                },
                {
                    render: function (data, type, full, meta) {
                        return data.ContactName;
                    },
                    data: function (row, type, val, meta) {
                        return row;
                    }
                },
                {
                    render: function (data, type, full, meta) {
                        return data.ContactNumber;
                    },
                    data: function (row, type, val, meta) {
                        return row;
                    }
                },
            ],
            data: calendar.Data,
            //language: Resources.DataTablesLanguage,
        });

    },

}

calendar.Init();