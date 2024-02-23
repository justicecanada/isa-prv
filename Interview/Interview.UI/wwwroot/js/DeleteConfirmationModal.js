
var deleteModal = {

    Uri: null,
    DeleteClass: "deleteRecord",
    ModalSelector: "modalContainer",
    SubmitButtonSelector: "btnYesDeleteRecord",
    CancelButtonSelector: "btnNoDeleteRecord",

    Init: function (uri) {

        this.Uri = uri;

        this.HookupMagnificPopup();

    },

    HookupModalHandlers: function () {

        this.Form = $("#deleteConfirmationModal")[0];

        $("#" + this.SubmitButtonSelector).on("click", deleteModal.Delete);

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
                elementParse: deleteModal.ElementParse,
                open: deleteModal.Open,
                beforeClose: deleteModal.BeforeClose,
                close: deleteModal.Close,
            }
        });

    },

    ElementParse: function (item) {

        var id = $(item.el).data().id;

        $.get(deleteModal.Uri + "?id=" + id)
            .done(function (data, textStatus, jqXHR) {
                $("#" + deleteModal.ModalSelector).html($(data));
                deleteModal.HookupModalHandlers();
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
        $(deleteModal.ModalSelector).empty();
    },

    Delete: function (e) {

        e.preventDefault();
        var formData = $(deleteModal.Form).serialize();

        $.ajax({
            type: "DELETE",
            url: deleteModal.Uri,
            data: formData,
            success: deleteModal.DeleteSuccess,
            fail: deleteModal.DeleteFail
        });

    },

    DeleteSuccess: function (data) {

        if (data.result) {

            window.location = window.location.href;

        }
        else {
            $(deleteModal.ModalSelector).html($(data));
            deleteModal.HookupModalHandlers();
        }

    },

    DeleteFail: function (e) {
        $.magnificPopup.close();
    },

}

if (wb.isReady)
    deleteModal.Init(deleteUri);
else
    $(document).on("wb-ready.wb", function (event) {
        deleteModal.Init(deleteUri);
    });