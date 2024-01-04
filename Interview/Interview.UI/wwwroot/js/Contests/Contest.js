

var contest = {

    Init: function () {

        ClassicEditor.create(document.querySelector('.richTextBox'))
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