var lang = {

    Uri: "/Default/LanguageStatusModal",
    ModalSelector: "#modalContainer",
    SubmitButtonSelector: "#btnLanguageSave",
    CancelButtonSelector: "#btnLanguageCancel",
    Form: null,

    Init: function () {

        this.HookupMagnificPopup();

        if (showLanguageStatusModal) { 
            $.magnificPopup.open({

                items: {
                    src: lang.ModalSelector
                },
                type: 'inline',
                callbacks: {
                    elementParse: lang.ElementParse,
                    close: lang.Close,
                }
            });
        }

    },

    HookupModalHandlers: function () {

        this.Form = $("#langForm")[0];

        $(this.SubmitButtonSelector).on("click", lang.Post);

        $(this.CancelButtonSelector).on("click", function (e) {
            e.preventDefault();
            $.magnificPopup.instance.close();
        });

    },

    HookupMagnificPopup: function () {

        $().magnificPopup({
            type: 'inline',
            modal: true,
            callbacks: {
                elementParse: lang.ElementParse,
                close: lang.Close,
            }
        });

    },

    ElementParse: function (item) {

        $.get(lang.Uri)
            .done(function (data, textStatus, jqXHR) {
                $(lang.ModalSelector).html($(data));
                lang.HookupModalHandlers();
            })
            .fail(function (data, textStatus, jqXHR) {
                $.magnificPopup.close();
            });

    },

    Close: function () {
        $(lang.ModalSelector).empty();
    },

    Post: function (e) {

        e.preventDefault();
        var formData = $(lang.Form).serialize();

        $.ajax({
            type: "POST",
            url: lang.Uri,
            data: formData,
            success: lang.PostSuccess,
            fail: lang.PostFail
        });

    },

    PostSuccess: function (data) {

        if (data.result) {
            $.magnificPopup.close();
            //table.UpdateTable(data.item);
            //interview.HookupMagnificPopup();
        }
        else {
            $(lang.ModalSelector).html($(data));
            lang.HookupModalHandlers();
        }

    },

    PostFail: function (e) {
        $.magnificPopup.close();
    },

}

if (wb.isReady)
    lang.Init();
else
    $(document).on("wb-ready.wb", function (event) {
        lang.Init();
    });