var interview = {

    OpenLink: $("#interviewModal")[0],
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

        $(this.OpenLink).magnificPopup({
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

        $.get(interview.Uri)
            .done(function (data, textStatus, jqXHR) {
                $(interview.ModalSelector).html($(data));
                interview.HookupModalHandlers();
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