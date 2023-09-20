
var slider = {

    Txt: $('#InterviewDuration')[0],
    Slider: null,

    Init: function () {

        this.Slider = $('#slider').slider({
            // set min and maximum values
            min: 0,
            //max: <%=txtInterviewDuration.Text %>,
            max: 1440,
            // step
            step: 5,
            // animate
            animate: true,
            // range
            range: false,
            // show tooltips
            tooltips: true,
            // ticks
            ticks: {
                // main tick
                tickMain: 10,
                // side tick
                tickSide: 10,
                // show main label
                tickShowLabelMain: true,
                // don't show side label
                tickShowLabelSide: false
            },
            // current data
            //handles: [{
            //    value: <%=Schedules.ElementAt(0).startValue %>,
            //    type: "candidat"
            //}, {
            //    value: <%=Schedules.ElementAt(1).startValue %>,
            //    type: "members"
            //}, {
            //    value: <%=Schedules.ElementAt(2).startValue %>,
            //    type: "marking"
            //}],
            // display type names
            showTypeNames: true,
            typeNames: {
                //'candidat': '<%=GetLocalResourceObject("ArriveCandidat").ToString()%>',
                //'members': '<%=GetLocalResourceObject("ArriveComite").ToString()%>',
                //'marking': '<%=GetLocalResourceObject("EvaluationComite").ToString()%>'
            },
            // slide callback
            slide: function (e, ui) {
                console.log(ui.values);
                //$.ajax({
                //    url: 'Contest.aspx/saveSchedule',
                //    type: 'POST',
                //    dataType: 'json',
                //    data: JSON.stringify({ values: ui.values }),
                //    contentType: 'application/json'
                //});
            }
        });

    },

}