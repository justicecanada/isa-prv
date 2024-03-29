﻿var userType = {

    Rbl: $("[name='UserType']"),
    InternalDetails: $("#internalDetails"),
    ExistingExternalDetails: $("#existingExternalDetails"),
    NewExternalDetails: $("#newExternalDetails"),

    Init: function () {

        $(this.Rbl).change(function () {

            var val = $(this).val();

            $(".userTypeDetails").hide();

            if (val === '0') 
                $(userType.InternalDetails).show();
            else if (val === '1')
                $(userType.ExistingExternalDetails).show();
            else if (val === '2')
                $(userType.NewExternalDetails).show();

        });

    }

}

var searchInternalUser = {

    Uri: "/Account/SearchInteralUsers",
    InputSelector: "#InternalName",
    Hidden: $("#InternalId")[0],

    Init: function () {

        $(this.InputSelector).autocomplete({
            minLength: 3,
            source: function (request, response) {

                $.ajax({
                    url: searchInternalUser.Uri,
                    dataType: "json",
                    data: { query: request.term },
                    success: function (data) {

                        response($.map(data.results, function (item) {
                            var object = new Object();
                            object.ID = item.id;
                            object.label = item.surname + ", " + item.givenName + ", " + item.mail;
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

if (wb.isReady) { 
    userType.Init();
    searchInternalUser.Init();
}
else
    $(document).on("wb-ready.wb", function (event) {
        userType.Init();
        searchInternalUser.Init();
    });