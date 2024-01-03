

var contest = {

    Init: function () {

        InlineEditor.create(document.querySelector('#editor'))
            .catch(error => {
                console.error(error);
            });
    }

}

if (wb.isReady)
    contest.Init();
else
    $(document).on("wb-ready.wb", function (event) {
        contest.Init();
    });