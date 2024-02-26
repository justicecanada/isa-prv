var participantsModal = {

    EditClass: "editParticipants",
    ModalSelector: "#modalContainer",
    Uri: "/Default/ParticipantsModal",
    Form: null,
    SubmitButtonSelector: "#btnParticipantSave",
    CancelButtonSelector: "#btnParticipantsClose",

    Init: function () {

        this.HookupMagnificPopup();

    },

    HookupModalHandlers: function () {

        this.Form = $("#participantsForm")[0];

        $(this.SubmitButtonSelector).on("click", participantsModal.Post);

        $(this.CancelButtonSelector).on("click", function (e) {
            e.preventDefault();
            $.magnificPopup.instance.close();
        });

    },

    HookupMagnificPopup: function () {

        $("." + this.EditClass).magnificPopup({
            type: 'inline',
            modal: true,
            callbacks: {
                elementParse: participantsModal.ElementParse,
                close: participantsModal.Close,
            }
        });

    },

    ElementParse: function (item, e) {

        var id = $(item.el[0]).data().id;

        $.get(participantsModal.Uri + "?id=" + id)
            //$.get(item.el[0].href)
            .done(function (data, textStatus, jqXHR) {
                $(participantsModal.ModalSelector).html($(data));
                participantsModal.HookupModalHandlers();
            })
            .fail(function (data, textStatus, jqXHR) {
                $.magnificPopup.close();
            });

    },

    Close: function () {
        $(participantsModal.ModalSelector).empty();
    },

    Post: function (e) {

        e.preventDefault();
        var formData = $(participantsModal.Form).serialize();

        $.ajax({
            type: "POST",
            url: participantsModal.Uri,
            data: formData,
            success: participantsModal.PostSuccess,
            fail: participantsModal.PostFail
        });

    },

    PostSuccess: function (data) {

        if (data.result) {
            $.magnificPopup.close();
            participantsModal.PosbBack();
            //table.UpdateTable(data.item);
            //table.HookupMagnificPopup();
        }
        else {
            $(participantsModal.ModalSelector).html($(data));
            participantsModal.HookupModalHandlers();
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
    participantsModal.Init();
else
    $(document).on("wb-ready.wb", function (event) {
        participantsModal.Init();
    });