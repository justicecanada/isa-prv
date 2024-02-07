var searchInternalUser = {

    Uri: "/Account/SearchInteralUsers",
    InputSelector: ".internalName",
    Hidden: null,

    Init: function () {

        $(this.InputSelector).autocomplete({
            minLength: 3,
            source: function (request, response) {

                searchInternalUser.Hidden = $(this.element[0].form).find('#InternalId')[0];

                $.ajax({
                    url: searchInternalUser.Uri,
                    dataType: "json",
                    data: { query: request.term },
                    success: function (data) {

                        response($.map(data.results, function (item) {
                            var object = new Object();
                            object.ID = item.id;
                            object.label = item.surname + ", " + item.givenName;
                            object.value = item.surname + ", " + item.givenName;
                            return object
                        }));

                    }
                });

            },
            select: function (event, ui) {
                $(searchInternalUser.Hidden).val(ui.item.ID);
            },
        });

    }

}

if (wb.isReady)
    searchInternalUser.Init();
else
    $(document).on("wb-ready.wb", function (event) {
        searchInternalUser.Init();
    });