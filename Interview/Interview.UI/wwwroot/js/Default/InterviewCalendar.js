
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
                        return data.Room;
                    },
                    data: function (row, type, val, meta) {
                        return row;
                    }
                },
                {
                    render: function (data, type, full, meta) {
                        return data.Location;
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

    UpdateTable: function (row) {

        var index = -1;

        $(this.Data).each(function (i) {
            if (this.Id == row.Id)
                index = i;
        });

        if (index === -1)
            this.Data.push(row);
        else
            this.Data[index] = row;

        this.InitTable();

    },

}

calendar.Init();