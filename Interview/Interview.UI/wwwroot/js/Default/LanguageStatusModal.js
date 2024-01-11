var lang = {

    Init: function () {

        debugger;

    }

}

if (wb.isReady)
    lang.Init();
else
    $(document).on("wb-ready.wb", function (event) {
        lang.Init();
    });