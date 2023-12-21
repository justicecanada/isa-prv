//https://wet-boew.github.io/wet-boew/demos/cal-events/cal-events-en.html#calendar4

$(document).on("wb-ready.wb-calevt", ".wb-calevt", function (event) {
    calendar.Init();
});

var calendar = {

    Control: $("#interviewCalendar")[0],
    CurrentMonthSelector: ".current-month",

    Init: function () {

        $(document).on("wb-updated.wb-calevt", ".wb-calevt", calendar.ChangeMonth);

    }, 

    ChangeMonth: function () {

        debugger;

        var selectedMonth = $(calendar.CurrentMonthSelector)[0].innerHTML;

    }

}
