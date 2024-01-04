var jusRichTextBoxFor = {

    Init: function () {

        $(".richTextBox").each(function () {

            var height = $(this).data().height;

            ClassicEditor.create(document.querySelector("#" + this.id))
                .then(editor => {
                    editor.ui.view.editable.element.style.height = height;
                })
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