var interviewModal = {

    OpenLink: "#interviewTableModal",
    EditClass: "editInterview",
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

        $(this.SubmitButtonSelector).on("click", interviewModal.Post);

        $(this.CancelButtonSelector).on("click", function (e) {
            e.preventDefault();
            $.magnificPopup.instance.close();
        });

    },

    HandleCalendarUpdate: function (e) {

        interviewModal.Init();

    },

    HookupMagnificPopup: function () {

        $(this.OpenLink + ", ." + this.EditClass).magnificPopup({
            type: 'inline',
            modal: true,
            callbacks: {
                elementParse: interviewModal.ElementParse,
                close: interviewModal.Close,
            }
        });

    },

    ElementParse: function (item, e) {

        if ($(item.el[0]).hasClass(interviewModal.EditClass)) {

            var id = $(item.el[0]).data().id;

            $.get(interviewModal.Uri + "?id=" + id)
            //$.get(item.el[0].href)
                .done(function (data, textStatus, jqXHR) {
                    $(interviewModal.ModalSelector).html($(data));
                    interviewModal.HookupModalHandlers();
                })
                .fail(function (data, textStatus, jqXHR) {
                    $.magnificPopup.close();
                });

        }
        else {

            $.get(interviewModal.Uri)
                .done(function (data, textStatus, jqXHR) {
                    $(interviewModal.ModalSelector).html($(data));
                    interviewModal.HookupModalHandlers();
                })
                .fail(function (data, textStatus, jqXHR) {
                    $.magnificPopup.close();
                });

        }

    },

    Close: function () {
        $(interviewModal.ModalSelector).empty();
    },

    Post: function (e) {

        e.preventDefault();
        var formData = $(interviewModal.Form).serialize();

        $.ajax({
            type: "POST",
            url: interviewModal.Uri,
            data: formData,
            success: interviewModal.PostSuccess,
            fail: interviewModal.PostFail
        });

    },

    PostSuccess: function (data) {

        if (data.result) {
            $.magnificPopup.close();
            interviewModal.PosbBack();
            //table.UpdateTable(data.item);
            //table.HookupMagnificPopup();
        }
        else {
            $(interviewModal.ModalSelector).html($(data));
            interviewModal.HookupModalHandlers();
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

if (wb.isReady)
    interviewModal.Init();
else
    $(document).on("wb-ready.wb", function (event) {
        interviewModal.Init();
    });