$(document).ready(function () {

    $("#Contests").on("change", function () {

        val = $(this).val();

        if (val != '');
            window.location.href = "Contests/Contest?id=" + val;

    });

})