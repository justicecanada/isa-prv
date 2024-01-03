if (wb.isReady)
    interview.Init();
else
    $(document).on("wb-ready.wb", function (event) {
        interview.Init();
    });

var interview = {

    OpenLink: "#interviewModal",
    EditClass: "cal-evt-lnk",
    ModalSelector: "#modalContainer",
    Uri: "/Default/InterviewModal",
    Form: null,
    SubmitButtonSelector: "#btnInterviewSave",
    CancelButtonSelector: "#btnInterviewClose",

    Init: function () {


        this.ManageCalendarAnchors();
        this.HookupMagnificPopup();
        $(document).off("wb-updated.wb-calevt", ".wb-calevt");
        $(document).on("wb-updated.wb-calevt", ".wb-calevt", interview.HandleCalendarUpdate);

    },

    HookupModalHandlers: function () {

        this.Form = $("#interviewForm")[0];

        $(this.SubmitButtonSelector).on("click", interview.Post);

        $(this.CancelButtonSelector).on("click", function (e) {
            e.preventDefault();
            $.magnificPopup.instance.close();
        });

    },

    ManageCalendarAnchors: function () {

        $("." + this.EditClass).each(function () {
            var source = this.hash;
            var id = $(source).find(".interviewId").val();
            $(this).attr("href", interview.ModalSelector);
            $(this).attr("data-id", id);
        });

    },

    HandleCalendarUpdate: function (e) {

        interview.Init();

    },

    HookupMagnificPopup: function () {

        $(this.OpenLink + ", ." + this.EditClass).magnificPopup({
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

        if ($(item.el[0]).hasClass(interview.EditClass)) {

            var id = $(item.el[0]).data().id;

            $.get(interview.Uri + "?id=" + id)
                .done(function (data, textStatus, jqXHR) {
                    $(interview.ModalSelector).html($(data));
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
            interview.PosbBack();
            //table.UpdateTable(data.item);
            //interview.HookupMagnificPopup();
        }
        else {
            $(interview.ModalSelector).html($(data));
            interview.HookupModalHandlers();
        }

    },

    PostFail: function (e) {
        $.magnificPopup.close();
    },

    PosbBack: function () {

        var href = window.location.origin;
        window.location.href = href;

    }

}