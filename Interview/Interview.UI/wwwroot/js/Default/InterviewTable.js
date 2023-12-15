
var table = {

    Data: [],
    Table: null,
    TableSelector: "#tblItems",
    EditClass: "aEdit",
    UsersClass: "aUser",

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

                        result = "<span class='mrgn-rght-sm'><a data-id=" + data.Id + " href='#modalContainer' class='" + table.EditClass + "'>Edit</a></span>";
                        result = result + "<span><a data-id=" + data.Id + " href='#modalContainer' class='" + table.UsersClass + "'>Users</a></span>";

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
            data: table.Data,
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
        users.Init();

    },

}

table.Init();