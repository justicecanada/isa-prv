var jusRichTextBoxFor = {

    Init: function () {

        $(".richTextBox").each(function () {

            ClassicEditor.create(document.querySelector("#" + this.id))
                .catch(error => {
                    console.error(error);
                });

        });

    }

}

if (wb.isReady)
    jusRichTextBoxFor.Init();
else
    $(document).on("wb-ready.wb", function (event) {
        jusRichTextBoxFor.Init();
    });