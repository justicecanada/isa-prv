var priv = {

    Uri: "/Default/PrivacyStatementModal",
    ModalSelector: "#modalContainer",
    SubmitButtonSelector: "#btnPrivacySave",
    CancelButtonSelector: "#btnPrivacyCancel",
    Form: null,

    Init: function () {

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

    HookupModalHandlers: function () {

        this.Form = $("#privForm")[0];

        $(this.SubmitButtonSelector).on("click", priv.Post);

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
                elementParse: priv.ElementParse,
                close: priv.Close,
            }
        });

    },

    ElementParse: function (item) {

        $.get(priv.Uri)
            .done(function (data, textStatus, jqXHR) {
                $(priv.ModalSelector).html($(data));
                priv.HookupModalHandlers();
            })
            .fail(function (data, textStatus, jqXHR) {
                $.magnificPopup.close();
            });

    },

    Close: function () {
        $(priv.ModalSelector).empty();
    },

    Post: function (e) {

        e.preventDefault();
        var formData = $(priv.Form).serialize();

        $.ajax({
            type: "POST",
            url: priv.Uri,
            data: formData,
            success: priv.PostSuccess,
            fail: priv.PostFail
        });

    },

    PostSuccess: function (data) {

        if (data.result) {
            $.magnificPopup.close();
            //table.UpdateTable(data.item);
            //interview.HookupMagnificPopup();
        }
        else {
            $(priv.ModalSelector).html($(data));
            priv.HookupModalHandlers();
        }

    },

    PostFail: function (e) {
        $.magnificPopup.close();
    },

}

if (wb.isReady)
    priv.Init();
else
    $(document).on("wb-ready.wb", function (event) {
        priv.Init();
    });