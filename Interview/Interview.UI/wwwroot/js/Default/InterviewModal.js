var interviewTable = {

    OpenLink: "#interviewTableModal",
    EditClass: "editTable",
    ModalSelector: "#modalContainer",
    Uri: "/Default/InterviewModal",
    Form: null,
    SubmitButtonSelector: "#btnInterviewSave",
    CancelButtonSelector: "#btnInterviewClose",

    Init: function () {

        this.HookupMagnificPopup();
        //$(document).off("wb-updated.wb-calevt", ".wb-calevt");
        //$(document).on("wb-updated.wb-calevt", ".wb-calevt", interview.HandleCalendarUpdate);

    },

    HookupModalHandlers: function () {

        this.Form = $("#interviewForm")[0];

        $(this.SubmitButtonSelector).on("click", interviewTable.Post);

        $(this.CancelButtonSelector).on("click", function (e) {
            e.preventDefault();
            $.magnificPopup.instance.close();
        });

    },

    HandleCalendarUpdate: function (e) {

        interviewTable.Init();

    },

    HookupMagnificPopup: function () {

        $(this.OpenLink + ", ." + this.EditClass).magnificPopup({
            type: 'inline',
            modal: true,
            callbacks: {
                elementParse: interviewTable.ElementParse,
                close: interviewTable.Close,
            }
        });

    },

    ElementParse: function (item, e) {

        if ($(item.el[0]).hasClass(interviewTable.EditClass)) {

            var id = $(item.el[0]).data().id;

            $.get(interviewTable.Uri + "?id=" + id)
            //$.get(item.el[0].href)
                .done(function (data, textStatus, jqXHR) {
                    $(interviewTable.ModalSelector).html($(data));
                    interviewTable.HookupModalHandlers();
                })
                .fail(function (data, textStatus, jqXHR) {
                    $.magnificPopup.close();
                });

        }
        else {

            $.get(interviewTable.Uri)
                .done(function (data, textStatus, jqXHR) {
                    $(interviewTable.ModalSelector).html($(data));
                    interviewTable.HookupModalHandlers();
                })
                .fail(function (data, textStatus, jqXHR) {
                    $.magnificPopup.close();
                });

        }

    },

    Close: function () {
        $(interviewTable.ModalSelector).empty();
    },

    Post: function (e) {

        e.preventDefault();
        var formData = $(interviewTable.Form).serialize();

        $.ajax({
            type: "POST",
            url: interviewTable.Uri,
            data: formData,
            success: interviewTable.PostSuccess,
            fail: interviewTable.PostFail
        });

    },

    PostSuccess: function (data) {

        if (data.result) {
            $.magnificPopup.close();
            interviewTable.PosbBack();
            //table.UpdateTable(data.item);
            //table.HookupMagnificPopup();
        }
        else {
            $(interviewTable.ModalSelector).html($(data));
            interviewTable.HookupModalHandlers();
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
    interviewTable.Init();
else
    $(document).on("wb-ready.wb", function (event) {
        interviewTable.Init();
    });