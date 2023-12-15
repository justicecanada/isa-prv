var users = {

    ModalSelector: "#modalContainer",
    Form: null,
    Uri: "/Default/InterviewUsersModal",
    SubmitButtonSelector: "#btnInterviewUsersSave",
    CancelButtonSelector: "#btnInterviewUsersClose",

    Init: function () {

        this.HookupMagnificPopup();

    },

    HookupModalHandlers: function () {

        this.Form = $("#userForm")[0];

        $(this.SubmitButtonSelector).on("click", users.Post);

        $(this.CancelButtonSelector).on("click", function (e) {
            e.preventDefault();
            $.magnificPopup.instance.close();
        });

    },

    HookupMagnificPopup: function () {

        $("." + table.UsersClass).magnificPopup({
            type: 'inline',
            modal: true,
            callbacks: {
                elementParse: users.ElementParse,
                open: users.Open,
                beforeClose: users.BeforeClose,
                close: users.Close,
            }
        });

    },

    ElementParse: function (item) {

        var id = $(item.el[0]).data().id;

        $.get(users.Uri + "?interviewId=" + id)
            .done(function (data, textStatus, jqXHR) {
                $("#modalContainer").html($(data));
                users.HookupModalHandlers();
            })
            .fail(function (data, textStatus, jqXHR) {
                $.magnificPopup.close();
            });

    },

    Open: function () {

    },

    BeforeClose: function () {

    },

    Close: function () {
        $(users.ModalSelector).empty();
    },

    Post: function (e) {

        e.preventDefault();
        var formData = $(users.Form).serialize();

        $.ajax({
            type: "POST",
            url: users.Uri,
            data: formData,
            success: users.PostSuccess,
            fail: users.PostFail
        });

    },

    PostSuccess: function (data) {
        $.magnificPopup.close();
    },

    PostFail: function (e) {
        $.magnificPopup.close();
    },

}

users.Init();