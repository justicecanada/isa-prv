
var slider = {

    Txt: $('#InterviewDuration')[0],
    Slider: null,
    Schedules: null,

    Init: function (schedules) {

        this.Schedules = schedules;

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
            handles: [{
                value: slider.Schedules[0].StartValue,
                type: "candidat"
            }, {
                value: slider.Schedules[1].StartValue,
                type: "members"
            }, {
                value: slider.Schedules[2].StartValue,
                type: "marking"
            }],
            // display type names
            showTypeNames: true,
            typeNames: {
                'candidat': Resources.Contest.ArriveCandidat,
                'members': Resources.Contest.ArriveComite,
                'marking': Resources.Contest.EvaluationComite
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