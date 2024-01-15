var priv = {

    Uri: "/Default/PrivacyStatementModal",
    ModalSelector: "#modalContainer",
    SubmitButtonSelector: "#btnPrivacySave",
    CancelButtonSelector: "#btnPrivacyCancel",
    Form: null,

    Init: function () {

        debugger;
        this.HookupMagnificPopup();

        if (showPrivacyStatementModal) {
            $.magnificPopup.open({

                items: {
                    src: priv.ModalSelector
                },
                type: 'inline',
                callbacks: {
                    elementParse: priv.ElementParse,
                    close: priv.Close,
                }
            });
        }

    },

    HookupMagnificPopup: function () {

        $().magnificPopup({
            type: 'inline',
            modal: true,
            callbacks: {
                elementParse: priv.ElementParse,
                close: priv.Close,
            }
        });

    },

    ElementParse: function (item) {

        $.get(priv.Uri)
            .done(function (data, textStatus, jqXHR) {
                $(priv.ModalSelector).html($(data));
                //priv.HookupModalHandlers();
            })
            .fail(function (data, textStatus, jqXHR) {
                $.magnificPopup.close();
            });

    },

    Close: function () {
        $(priv.ModalSelector).empty();
    },

}

if (wb.isReady)
    priv.Init();
else
    $(document).on("wb-ready.wb", function (event) {
        priv.Init();
    });