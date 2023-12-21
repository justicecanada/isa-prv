//https://wet-boew.github.io/wet-boew/demos/cal-events/cal-events-en.html#calendar4

//$(document).on("wb-ready.wb-calevt", ".wb-calevt", function (event) {
//    calendar.Init();
//});

var calendar = {

    Control: $("#interviewCalendar")[0],
    CurrentMonthSelector: ".current-month",
    GetMonthUri: "Default/MonthylyInterviewsPartial",
    InterviewList: $("#interviewList")[0],

    Init: function () {

        $(document).on("wb-updated.wb-calevt", ".wb-calevt", calendar.ChangeMonth);

    }, 

    ChangeMonth: function () {

        var currentMonth = $(calendar.CurrentMonthSelector)[0].innerHTML;

        if (currentMonth != "") {
            // For some reason, the wb-ready.wb-calevt event is not being raised. So calling calendar.Init() as script loads.
            // This check ensures the ajax call is not executed on page load. Boo.

            $.get(calendar.GetMonthUri + "?currentMonth=" + currentMonth)
                .done(function (data, textStatus, jqXHR) {
                    $(calendar.InterviewList).html(data);
                    // TODO: https://wet-boew.github.io/wet-boew/docs/ref/cal-events/cal-events-en.html
                    // 1. Look here to reinitialize the calendar. Which element is $elm? (calendar.InterviewList or calendar.Control)
                    // 2. Monthly Interviews partial reapply the hidden style to ul
                    $(calendar.InterviewList).trigger("wb-init.wb-calevt");
                })
                .fail(function (data, textStatus, jqXHR) {
                    debugger;
                });

        }

    }

}

calendar.Init();