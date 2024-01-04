var searchInternalUser = {

    Uri: "/Groups/LookupInteralUser",
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
                            object.ID = item.Id;
                            object.label = item.LastName + ", " + item.FirstName;
                            object.value = item.Name;
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