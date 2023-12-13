var interview = {

    OpenLink: "#interviewModal",
    ModalSelector: "#modalContainer",
    Uri: "/Default/InterviewModal",
    Form: null,
    SubmitButtonSelector: "#btnInterviewSave",
    CancelButtonSelector: "#btnInterviewClose",

    Init: function () {

        this.HookupMagnificPopup();
        this.Users.Init();

    },

    HookupModalHandlers: function () {

        this.Form = $("#interviewForm")[0];

        $(this.SubmitButtonSelector).on("click", interview.Post);

        $(this.CancelButtonSelector).on("click", function (e) {
            e.preventDefault();
            $.magnificPopup.instance.close();
        });

        this.Users.HookupModalHandlers();

    },

    HookupMagnificPopup: function () {

        $(this.OpenLink + ", ." + calendar.EditClass).magnificPopup({
            type: 'inline',
            modal: true,
            callbacks: {
                elementParse: interview.ElementParse,
                open: interview.Open,
                beforeClose: interview.BeforeClose,
                close: interview.Close,
            }
        });

    },

    ElementParse: function (item) {

        if ($(item.el[0]).hasClass(calendar.EditClass)) {

            var id = $(item.el[0]).data().id;

            $.get(interview.Uri + "?id=" + id)
                .done(function (data, textStatus, jqXHR) {
                    $("#modalContainer").html($(data));
                    interview.HookupModalHandlers();
                })
                .fail(function (data, textStatus, jqXHR) {
                    $.magnificPopup.close();
                });

        }
        else {

            $.get(interview.Uri)
                .done(function (data, textStatus, jqXHR) {
                    $(interview.ModalSelector).html($(data));
                    interview.HookupModalHandlers();
                })
                .fail(function (data, textStatus, jqXHR) {
                    $.magnificPopup.close();
                });

        }

    },

    Open: function () {

    },

    BeforeClose: function () {

    },

    Close: function () {
        $(interview.ModalSelector).empty();
    },

    Post: function (e) {

        e.preventDefault();
        var formData = $(interview.Form).serialize();

        $.ajax({
            type: "POST",
            url: interview.Uri,
            data: formData,
            success: interview.PostSuccess,
            fail: interview.PostFail
        });

    },

    PostSuccess: function (data) {

        if (data.result) {
            $.magnificPopup.close();
            calendar.UpdateTable(data.item);
            interview.HookupMagnificPopup();
        }
        else {
            $(interview.ModalSelector).html($(data));
            interview.HookupModalHandlers();
        }

    },

    PostFail: function (e) {
        $.magnificPopup.close();
    },

    Users: {

        RblRolesSelector: "[name='RoleType']",
        BtnAddUserButoonSelector: "#btnAddMember",
        Form: null,
        Uri: "/Default/AddInterviewMember",

        Init: function () {
    

        },

        HookupModalHandlers: function () {

            this.Form = $("#userForm")[0];

            $(this.RblRolesSelector).change(function () {

                var val = $(this).val();

                $(".interviewerRole").hide();

                if (val === '5')
                    $("#candidateUsers").show();
                else if (val === '2')
                    $("#interviewerUsers").show();
                else if (val === '3')
                    $("#leadUsers").show();

            });

            $(this.BtnAddUserButoonSelector).on("click", interview.Users.Post);

        },

        Post: function (e) {

            e.preventDefault();
            var formData = $(interview.Users.Form).serialize();

            $.ajax({
                type: "POST",
                url: interview.Users.Uri,
                data: formData,
                success: interview.Users.PostSuccess,
                fail: interview.Users.PostFail
            });

        },

        PostSuccess: function (data) {
            $.magnificPopup.close();
        },

        PostFail: function (e) {
            $.magnificPopup.close();
        },

    }

}

interview.Init();