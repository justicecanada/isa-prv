var interview = {

    OpenLink: "#interviewModal",
    ModalSelector: "#modalContainer",
    Uri: "/Default/InterviewModal",
    Form: null,
    SubmitButtonSelector: "#btnInterviewSave",
    CancelButtonSelector: "#btnInterviewClose",

    Init: function () {

        this.HookupMagnificPopup();

    },

    HookupModalHandlers: function () {

        this.Form = $("#interviewForm")[0];

        $(this.SubmitButtonSelector).on("click", interview.Post);

        $(this.CancelButtonSelector).on("click", function (e) {
            e.preventDefault();
            $.magnificPopup.instance.close();
        });

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
        }
        else {
            $(interview.ModalSelector).html($(data));
            interview.HookupModalHandlers();
        }

    },

    PostFail: function (e) {
        $.magnificPopup.close();
    },

}

interview.Init();