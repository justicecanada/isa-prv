
var removeEmployee = {

    Uri: "/Groups/RemoveEmployeeModal",
    DeleteClass: "removeEmployee",
    ModalSelector: "modalContainer",
    SubmitButtonSelector: "btnYesDeleteRecord",
    CancelButtonSelector: "btnNoDeleteRecord",

    Init: function (uri) {

        this.HookupMagnificPopup();

    },

    HookupModalHandlers: function () {

        this.Form = $("#deleteConfirmationModal")[0];

        $("#" + this.SubmitButtonSelector).on("click", removeEmployee.Delete);

        $("#" + this.CancelButtonSelector).on("click", function (e) {
            e.preventDefault();
            $.magnificPopup.instance.close();
        });

    },

    HookupMagnificPopup: function () {

        $("." + this.DeleteClass).magnificPopup({
            type: 'inline',
            modal: true,
            callbacks: {
                elementParse: removeEmployee.ElementParse,
                open: removeEmployee.Open,
                beforeClose: removeEmployee.BeforeClose,
                close: removeEmployee.Close,
            }
        });

    },

    ElementParse: function (item) {

        var id = $(item.el).data().id;

        $.get(removeEmployee.Uri + "?id=" + id)
            .done(function (data, textStatus, jqXHR) {
                $("#" + removeEmployee.ModalSelector).html($(data));
                removeEmployee.HookupModalHandlers();
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
        $(removeEmployee.ModalSelector).empty();
    },

    Delete: function (e) {

        e.preventDefault();
        var formData = $(removeEmployee.Form).serialize();

        $.ajax({
            type: "DELETE",
            url: removeEmployee.Uri,
            data: formData,
            success: removeEmployee.DeleteSuccess,
            fail: removeEmployee.DeleteFail
        });

    },

    DeleteSuccess: function (data) {

        if (data.result) {

            window.location = window.location.href;

        }
        else {
            $(removeEmployee.ModalSelector).html($(data));
            removeEmployee.HookupModalHandlers();
        }

    },

    DeleteFail: function (e) {
        $.magnificPopup.close();
    },

}

var removeProcess = {

    Uri: "/Groups/RemoveProcessModal",
    DeleteClass: "removeProcess",
    ModalSelector: "modalContainer",
    SubmitButtonSelector: "btnYesDeleteRecord",
    CancelButtonSelector: "btnNoDeleteRecord",

    Init: function (uri) {

        this.HookupMagnificPopup();

    },

    HookupModalHandlers: function () {

        this.Form = $("#deleteConfirmationModal")[0];

        $("#" + this.SubmitButtonSelector).on("click", removeProcess.Delete);

        $("#" + this.CancelButtonSelector).on("click", function (e) {
            e.preventDefault();
            $.magnificPopup.instance.close();
        });

    },

    HookupMagnificPopup: function () {

        $("." + this.DeleteClass).magnificPopup({
            type: 'inline',
            modal: true,
            callbacks: {
                elementParse: removeProcess.ElementParse,
                open: removeProcess.Open,
                beforeClose: removeProcess.BeforeClose,
                close: removeProcess.Close,
            }
        });

    },

    ElementParse: function (item) {

        var id = $(item.el).data().id;

        $.get(removeProcess.Uri + "?id=" + id)
            .done(function (data, textStatus, jqXHR) {
                $("#" + removeProcess.ModalSelector).html($(data));
                removeProcess.HookupModalHandlers();
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
        $(removeProcess.ModalSelector).empty();
    },

    Delete: function (e) {

        e.preventDefault();
        var formData = $(removeProcess.Form).serialize();

        $.ajax({
            type: "DELETE",
            url: removeProcess.Uri,
            data: formData,
            success: removeProcess.DeleteSuccess,
            fail: removeProcess.DeleteFail
        });

    },

    DeleteSuccess: function (data) {

        if (data.result) {

            window.location = window.location.href;

        }
        else {
            $(removeProcess.ModalSelector).html($(data));
            removeProcess.HookupModalHandlers();
        }

    },

    DeleteFail: function (e) {
        $.magnificPopup.close();
    },

}

if (wb.isReady) {
    removeEmployee.Init();
    removeProcess.Init();
}
else
    $(document).on("wb-ready.wb", function (event) {
        removeEmployee.Init();
        removeProcess.Init();
    });