var searchInternalUser = {

    SearchUri: "/Account/SearchInteralUsers",
    GetDetailsUri: "/Account/GetUserDetails",
    InputSelector: "#InternalName",
    UserDetailsContainer: $("#userDetailsContainer")[0],
    UserDetails: $("#userDetails")[0],

    Init: function () {

        $(this.InputSelector).autocomplete({
            minLength: 3,
            source: searchInternalUser.Search,
            select: searchInternalUser.Select
        });

    },

    Search: function (request, response) {

        $.ajax({
            url: searchInternalUser.SearchUri,
            dataType: "json",
            data: { query: request.term },
            success: function (data) {
                response($.map(data.results, function (item) {
                    var object = new Object();
                    object.ID = item.userPrincipalName;
                    object.label = item.surname + ", " + item.givenName + ", " + item.mail;
                    object.value = item.surname + ", " + item.givenName;
                    return object
                }));
            }
        });

    },

    Select: function(event, ui) {

        var userPrincipalName = ui.item.ID;

        $.ajax({
            url: searchInternalUser.GetDetailsUri,
            dataType: "json",
            async: true,
            data: { userPrincipalName: userPrincipalName },
            success: function (event, ui) {
                $(searchInternalUser.UserDetailsContainer).show();
                $(searchInternalUser.UserDetails).html(event.results);
            }
        });

    }, 

}

if (wb.isReady)
    searchInternalUser.Init();
else
    $(document).on("wb-ready.wb", function (event) {
        searchInternalUser.Init();
    });