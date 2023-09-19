
$(document).ready(function () {
    datePickers.Init();
});

var datePickers = {

    MinVal: "0001-01-01",
    Format: "yy-mm-dd",

    Init: function () {

        var date = new Date();

        $(".date-picker.date").each(function () {

            $(this).datepicker({
                changeMonth: true,
                changeYear: true,
                dateFormat: datePickers.Format,
                language: "en",
                autoclose: true,
                //date: date
                //startView: "month"
                //defaultViewDate: date
                //todayHighlight: true,
                //assumeNearbyYear: true
            });

            //if ($(this).val() == datePickers.MinVal)
            //    $(this).val("");

        });

    },

}