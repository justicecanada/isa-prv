$(document).ready(function () {
    userType.Init();
    //searchInternalUser.Init();
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

    InputSelector: "#InternalName",

    Init: function () {

        //debugger;

        $(this.InputSelector).autocomplete({
            minLength: 3,
            source: function (request, response) {
                debugger;
            },
            select: function (event, ui) {

            },
        });

    }

}