$(document).ready(function () {

    $("#Contests").on("change", function () {

        val = $(this).val();

        if (val != '');
        window.location.href = "Default/SwitchContest?contestId=" + val;

    });

})