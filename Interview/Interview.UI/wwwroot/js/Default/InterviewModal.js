var table = {

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

        $(this.SubmitButtonSelector).on("click", table.Post);

        $(this.CancelButtonSelector).on("click", function (e) {
            e.preventDefault();
            $.magnificPopup.instance.close();
        });

    },

    HandleCalendarUpdate: function (e) {

        table.Init();

    },

    HookupMagnificPopup: function () {

        $(this.OpenLink + ", ." + this.EditClass).magnificPopup({
            type: 'inline',
            modal: true,
            callbacks: {
                elementParse: table.ElementParse,
                close: table.Close,
            }
        });

    },

    ElementParse: function (item, e) {

        if ($(item.el[0]).hasClass(table.EditClass)) {

            var id = $(item.el[0]).data().id;

            $.get(table.Uri + "?id=" + id)
            //$.get(item.el[0].href)
                .done(function (data, textStatus, jqXHR) {
                    $(table.ModalSelector).html($(data));
                    table.HookupModalHandlers();
                })
                .fail(function (data, textStatus, jqXHR) {
                    $.magnificPopup.close();
                });

        }
        else {

            $.get(table.Uri)
                .done(function (data, textStatus, jqXHR) {
                    $(table.ModalSelector).html($(data));
                    table.HookupModalHandlers();
                })
                .fail(function (data, textStatus, jqXHR) {
                    $.magnificPopup.close();
                });

        }

    },

    Close: function () {
        $(table.ModalSelector).empty();
    },

    Post: function (e) {

        e.preventDefault();
        var formData = $(table.Form).serialize();

        $.ajax({
            type: "POST",
            url: table.Uri,
            data: formData,
            success: table.PostSuccess,
            fail: table.PostFail
        });

    },

    PostSuccess: function (data) {

        if (data.result) {
            $.magnificPopup.close();
            table.PosbBack();
            //table.UpdateTable(data.item);
            //table.HookupMagnificPopup();
        }
        else {
            $(table.ModalSelector).html($(data));
            table.HookupModalHandlers();
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
    table.Init();
else
    $(document).on("wb-ready.wb", function (event) {
        table.Init();
    });