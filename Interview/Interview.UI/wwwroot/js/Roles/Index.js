$(document).ready(function () {
    userType.Init();
    searchInternalUser.Init();
});

var userType = {

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

    Uri: "/Roles/LookupInteralUser",
    InputSelector: "#InternalName",

    Init: function () {

        //$(this.InputSelector).off("autocomplete");
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
                            object.label = item.lastName + ", " + item.firstName;
                            object.value = item.name;
                            
                            //object.email = item.email;
                            //object.website = item.website;
                            return object
                        }));

                    }
                });

            },
            select: function (event, ui) {
                //debugger;
            },
        });
        //    .autocomplete("instance")._renderItem = function (ul, item) {
        //    debugger;
        //};

    }

}