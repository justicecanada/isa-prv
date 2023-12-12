
//$(function () {
//    loadCalendar();
//    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(loadCalendar);
//});

var timeout;

function loadCalendar() {

    // nécessaire pour la validention client
    $('form').wrap('<div class="wb-frmvld"></div>');

    // documentation: https://fullcalendar.io/docs#toc
    // configuration du calendrier
    var calendar = $('#calendar').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay,listMonth'
        },
        buttonText: {
            today: Resources.Calendar.Today,
            month: Resources.Calendar.Month,
            week: Resources.Calendar.Week,
            day: Resources.Calendar.Day,
            list: Resources.Calendar.List
        },
        defaultView: 'agendaWeek',
        defaultTimedEventDuration: '<%=Contest.interviewDuration%>',
        minTime: '<%=Contest.minTime%>',
        maxTime: '<%=Contest.maxTime%>',
        selectable: true,
        allDaySlot: false,
        slotEventOverlap: false,
        locale: '<%=this.lang%>-ca',
        weekends: false,
        weekNumbers: false,
        timezone: 'local',
        lazyFetching: true,
        startParam: 'start',
        endParam: 'end',
        contentHeight: 'auto',
        views: {
            week: {
                columnFormat: 'dddd DD'
            }
        },
        events: {
            url: 'FullCalender.asmx/GetCalendarEvents',
            data: {
                concoursId: '<%=Contest.concoursId%>',
                userId: '<%=(RoleUser.roleId == (int)Entrevue.Models.RoleEnum.ASSISTANT && usager.AdjointeDe.Count > 0) ? usager.AdjointeDe.Keys.ToListeSepareeParDelimiteur(x => x.ToString(), ";").TrimStart('; '): usager.usagerGUID.ToString() %>',
                roleId: '<%=RoleUser.roleId%>'
            }
        },
        loading: function (isLoading, view) {
            // loading si plus long que 1 seconde
            if (isLoading) {
                timeout = setTimeout(function () {
                    $('.spinner').show();
                }, 1000);
            }
            else {
                clearTimeout(timeout);
                $('.spinner').hide();
            }
        },
        eventClick: function (calEvent, jsEvent, view) {
            if (RoleId == RoleEnum.CANDIDATE && calEvent.users.toString().indexOf("<%=usager.NomPrenomAvecDistinctionTelephonique%>") == -1) {
                var dataRow = {
                    'UserId': '<%=usager.usagerGUID.ToString()%>',
                    'EventId': calEvent.id,
                    'ConcoursId': '<%=Contest.concoursId%>',
                    'UserAddedByAdmin': 'False'
                }
                addUserMember(dataRow, RoleId);
            }
            else if (RoleId != RoleEnum.CANDIDATE && RoleId == RoleEnum.ASSISTANT) {

                var dataRow = {
                    'UserId': $('#ddlAdjointDe').val(),
                    'EventId': calEvent.id,
                    'ConcoursId': '<%=Contest.concoursId%>',
                    'UserAddedByAdmin': 'False'
                }
                addUserAssistant(dataRow, calEvent);
                wb.doc.trigger("open.wb-lbx", [[{ src: "#popupAddUser", type: "inline" }], true]);

            }
            else if (RoleId != RoleEnum.CANDIDATE && (RoleId == RoleEnum.INTERVIEWER || RoleId == RoleEnum.LEAD) && calEvent.users.toString().indexOf("<%=usager.NomPrenomAvecDistinctionTelephonique%>") == -1) {

                if (confirm('<%=GetLocalResourceObject("AddAvailability")%>')) {
                    var dataRow = {
                        'UserId': '<%=usager.usagerGUID.ToString()%>',
                        'EventId': calEvent.id,
                        'ConcoursId': '<%=Contest.concoursId%>',
                        'UserAddedByAdmin': 'False'
                    }
                    addUserInterviewers(dataRow);
                }
            }
            else if (RoleId != RoleEnum.CANDIDATE && (RoleId == RoleEnum.ADMIN || RoleId == RoleEnum.ASSISTANT || RoleId == RoleEnum.HR)) {
                // calcul des minutes
                var diffMs = (calEvent.end - calEvent.start); // milliseconds between
                var diffMins = Math.floor((diffMs / 1000) / 60); // minutes

                // paneau de droite
                $('#containerUsers').show();
                $('#containerPreview').show();
                $('#candidatesArrival').val(moment(calEvent.start).add(calEvent.minutesFromStartForCandidate, 'minutes').format('HH:mm'));
                $('#membersArrival').val(moment(calEvent.start).add(calEvent.minutesFromStartForMembers, 'minutes').format('HH:mm'));
                $('#boardMarking').val(moment(calEvent.start).add(calEvent.minutesFromStartForMarking, 'minutes').format('HH:mm'));

                // boutons HR seulement
                if (RoleId == RoleEnum.HR) {
                    if (calEvent.users.toString().indexOf("<%=usager.NomPrenomAvecDistinctionTelephonique%>") == -1) {
                        $('#btnAddHRMember').show();
                        $('#btnRemoveHRMember').hide();
                    }
                    else {
                        $('#btnAddHRMember').hide();
                        $('#btnRemoveHRMember').show();
                    }
                }
                else {
                    $('#btnAddHRMember').hide();
                    $('#btnRemoveHRMember').hide();
                }

                $('#eventID').val(calEvent.id);
                $('#timezone').val(calEvent.start.format('Z'));
                $('#eventSalle').val(calEvent.salle);
                $('#eventLocation').val(calEvent.location);
                $('#eventContactName').val(calEvent.contactName);
                $('#eventContactNumber').val(calEvent.contactNumber);
                $('#eventDate').val(moment(calEvent.start).format('DD/MM/YYYY'));
                $('#eventTime').val(moment(calEvent.start).format('HH:mm'));
                $('#eventDuration').val(diffMins);

                $("#btnPopupDelete").show();
                ShowEventPopup(calEvent.date);
            }
        },
        dayClick: function (date, allDay, jsEvent, view) {
            if (RoleId != RoleEnum.CANDIDATE && (RoleId == RoleEnum.ADMIN || RoleId == RoleEnum.HR)) {
                ClearPopupFormValues();
                // default values
                $('#containerPreview').hide();
                $('#containerUsers').hide();
                $('#timezone').val(moment(date).format('Z'));
                $('#eventDate').val(moment(date).format('DD/MM/YYYY'));
                $('#eventTime').val(moment(date).format('HH:mm'));
                $('#eventDuration').val(<%=Contest.interviewDuration.Value.TotalMinutes %>);

                $("#btnPopupDelete").hide();
                ShowEventPopup(date);
            }
        },
        eventRender: function (event, element, view) {
            var usersStr = event.users.toString();
            // modifier l'affichage
            if (view.name == "listMonth") {

                // skip certains événements pour les interviewers
                if ((RoleId == RoleEnum.INTERVIEWER || RoleId == RoleEnum.LEAD) && usersStr.indexOf("<%=usager.NomPrenomAvecDistinctionTelephonique%>") == -1) {
                    return false;
                } else if (RoleId == RoleEnum.ASSISTANT && <%= usager.AdjointeDe.Count %> > 0) {
                    var list = "<%=usager.AdjointeDe.Values.ToListeSepareeParDelimiteur(x => x.NomPrenomAvecDistinctionTelephonique, "; ").TrimStart(';')%>";
    if (list.indexOf(';') == -1 && usersStr.indexOf(list) == -1) {
        return false;
    } else {
        var skipEvent = true;
        $.each(list.split(';'), function (i, val) {
            if (usersStr.indexOf(val) >= 0) {
                skipEvent = false;
                return false;
            }
        });
        if (skipEvent) {
            return false;
        }
    }
} else if (RoleId == RoleEnum.CANDIDATE && usersStr.indexOf("<%=usager.NomPrenomAvecDistinctionTelephonique%>") == -1) {
    return false;
}

//element.removeClass("Reserve").removeClass("Waiting").removeClass("Available");
//$info = $('<div class="row"><div class="col-md-7 infoUsers"></div><div class="col-md-3 infoLocation"></div></div>')
//element.find('.fc-list-item-title').append($info);

//// afficher la location
//var infoLocation = '<span class="h6"><%=GetLocalResourceObject("eventLocation")%></span><br /><span class="small">' + event.location + '</span><br /><span class="h6"><%=GetLocalResourceObject("eventSalle")%></span><br /><span class="small">' + event.salle + '</span><br /><span class="h6"><%=GetLocalResourceObject("eventLanguage")%></span><br /><span class="small">' + event.language + '</span>';
//// équité
//if (event.equities) {
//    infoLocation += ' <span class="small">' + event.equities + '</span>';
//}
//element.find('.infoLocation').append($(infoLocation));
//                        }
//                        else {
//    element.append('<span class="truncated">' + event.location + '<br />' + event.salle + '</span>');
//    if (event.equities) {
//        element.append('<span class="eventLang">' + event.language + ' ' + event.equities + '</span>');
//    } else {
//        element.append('<span class="eventLang">' + event.language + '</span>');
//    }
//}

//if (RoleId == RoleEnum.CANDIDATE && usersStr.indexOf("<%=usager.NomPrenomAvecDistinctionTelephonique%>") >= 0) {
//    // afficher le candidat dans l'événement
//    if (view.name == 'listMonth') {
//        element.find('.infoUsers').append('<span class="label label-warning"><%=GetLocalResourceObject("VousEtesInscrit").ToString()%></span>');
//    } else {
//        element.append('<span style="position:absolute;top:2px;right:2px;" class="label label-warning"><%=GetLocalResourceObject("VousEtesInscrit").ToString()%></span>');
//    }
//}
//else if (event.users && RoleId != RoleEnum.CANDIDATE && (RoleId == RoleEnum.ADMIN || RoleId == RoleEnum.ASSISTANT || RoleId == RoleEnum.HR || RoleId == RoleEnum.INTERVIEWER || RoleId == RoleEnum.LEAD)) {
//    if (view.name == "listMonth") {
//        element.find('.infoUsers').append(event.users);
//    }
//    else {
//        // aficher la liste des usagers sous forme de d'info-bulle
//        element.qtip({
//            content: event.users,
//            position: {
//                my: 'top left',
//                at: 'bottom left'
//            },
//            style: {
//                classes: 'qtip-bootstrap'
//            },
//            show: {
//                solo: true
//            },
//            hide: {
//                fixed: true,
//                delay: 90
//            },
//            events: {
//                render: function () {
//                    $(this).find('.remove-user').on('click', function () {

//                        var dataRow = {
//                            'PlageHoraireId': $(this).attr("data-phId"),
//                            'UserId': $(this).attr("data-guid")
//                        }

//                        removeUser(dataRow, element);
//                    });
//                }
//            }
//        });
//    }
//}
//                    }
//                });

//// afficher à la date du concour si ultérieur àa la date du jour
//if (moment('<%=Contest.startDate.ToDateTime().ToString("yyyy-MM-dd")%>') > moment().toDate()) {
//    var m = $.fullCalendar.moment('<%=Contest.startDate.ToDateTime().ToString("yyyy-MM-dd")%>');
//    calendar.fullCalendar('gotoDate', m);
//}

//$('#btnPopupDelete').click(function (e) {

//    var dataRow = {
//        'EventId': $('#eventID').val()
//    }

//    $.ajax({
//        type: 'POST',
//        url: "FullCalender.asmx/DeleteEvent",
//        data: dataRow,
//        beforeSend: function () {
//            $('.spinner').show();
//        },
//        success: function (response) {
//            if (response == 'true') {
//                ClearPopupFormValues();
//                $.notify({
//                    // options
//                    message: '<%=GetGlobalResourceObject("Notify", "NotifyDeleteEventSuccess").ToString()%>'
//                },
//                    {
//                        // settings
//                        element: "body",
//                        type: "success",
//                        allow_dismiss: true,
//                        placement: {
//                            from: "top",
//                            align: "center",
//                        },
//                        animate: {
//                            enter: "animated fadeInDown",
//                            exit: "animated fadeOutUp",
//                        },
//                        icon_type: "class"
//                    });
//            }
//            else {
//                $.notify({
//                    // options
//                    message: '<%=GetGlobalResourceObject("Notify", "NotifyDeleteEventFail").ToString()%>'
//                },
//                    {
//                        // settings
//                        element: "body",
//                        type: "danger",
//                        allow_dismiss: true,
//                        placement: {
//                            from: "top",
//                            align: "center",
//                        },
//                        animate: {
//                            enter: "animated fadeInDown",
//                            exit: "animated fadeOutUp",
//                        },
//                        icon_type: "class"
//                    });
//            }
//        },
//        error: function (XMLHttpRequest, textStatus, errorThrown) {
//            $.notify({
//                // options
//                message: '<%=GetGlobalResourceObject("Notify", "NotifyDeleteEventFail").ToString()%>'
//            },
//                {
//                    // settings
//                    element: "body",
//                    type: "danger",
//                    allow_dismiss: true,
//                    placement: {
//                        from: "top",
//                        align: "center",
//                    },
//                    animate: {
//                        enter: "animated fadeInDown",
//                        exit: "animated fadeOutUp",
//                    },
//                    icon_type: "class"
//                });
//        },
//        complete: function (data) {
//            $('#calendar').fullCalendar('refetchEvents');
//            $('.spinner').hide();
//        }
//    });
//});

//$('#btnPopupSave').click(function (e) {

//    var dataRow = {
//        'UserId': '<%=usager.usagerGUID.ToString()%>',
//        'RoleId': RoleId,
//        'Location': $('#eventLocation').val(),
//        'Salle': $('#eventSalle').val(),
//        'EventId': $('#eventID').val(),
//        'EventDate': $('#eventDate').val(),
//        'EventTime': $('#eventTime').val(),
//        'EventDuration': $('#eventDuration').val(),
//        'Timezone': $('#timezone').val(),
//        'ContactName': $('#eventContactName').val(),
//        'ContactNumber': $('#eventContactNumber').val(),
//        'ConcoursId': '<%=Contest.concoursId%>'
//    }

//    $.ajax({
//        type: 'POST',
//        url: "FullCalender.asmx/CreateAndUpdateEvent",
//        data: dataRow,
//        beforeSend: function () {
//            $('.spinner').show();
//        },
//        success: function (response) {
//            if (response == 'true') {
//                ClearPopupFormValues();
//                $.notify({
//                    // options
//                    message: '<%=GetGlobalResourceObject("Notify", "NotifyCreateAndUpdateEventSuccess").ToString()%>'
//                },
//                    {
//                        // settings
//                        element: "body",
//                        type: "success",
//                        allow_dismiss: true,
//                        placement: {
//                            from: "top",
//                            align: "center",
//                        },
//                        animate: {
//                            enter: "animated fadeInDown",
//                            exit: "animated fadeOutUp",
//                        },
//                        icon_type: "class"
//                    });
//            }
//            else {
//                $.notify({
//                    // options
//                    message: '<%=GetGlobalResourceObject("Notify", "NotifyCreateAndUpdateEventFail").ToString()%>'
//                },
//                    {
//                        // settings
//                        element: "body",
//                        type: "danger",
//                        allow_dismiss: true,
//                        placement: {
//                            from: "top",
//                            align: "center",
//                        },
//                        animate: {
//                            enter: "animated fadeInDown",
//                            exit: "animated fadeOutUp",
//                        },
//                        icon_type: "class"
//                    });
//            }
//        },
//        error: function (XMLHttpRequest, textStatus, errorThrown) {
//            $.notify({
//                // options
//                message: '<%=GetGlobalResourceObject("Notify", "NotifyCreateAndUpdateEventFail").ToString()%>'
//            },
//                {
//                    // settings
//                    element: "body",
//                    type: "danger",
//                    allow_dismiss: true,
//                    placement: {
//                        from: "top",
//                        align: "center",
//                    },
//                    animate: {
//                        enter: "animated fadeInDown",
//                        exit: "animated fadeOutUp",
//                    },
//                    icon_type: "class"
//                });
//        },
//        complete: function (data) {
//            $('#calendar').fullCalendar('refetchEvents');
//            $('.spinner').hide();
//        }
//    });
//});

//$('#btnAddUserAndRole').click(function () {

//    var userId;
//    var roleId;
//    var role = $("input:radio[name='roles']:checked").val();

//    if (role == '<%=GetGlobalResourceObject("global", "candidate")%>') {
//        userId = $("#ddlCandidates option:selected").val();
//        roleId = RoleEnum.CANDIDATE;
//    }
//    else if (role == '<%=GetGlobalResourceObject("global", "interviewer")%>') {
//        userId = $("#ddlInterviewers option:selected").val();
//        roleId = RoleEnum.INTERVIEWER;
//    }
//    else if (role == '<%=GetGlobalResourceObject("global", "lead")%>') {
//        userId = $("#ddlLeads option:selected").val();
//        roleId = RoleEnum.LEAD;
//    }

//    var dataRow = {
//        'UserId': userId,
//        'ConcoursId': '<%=Contest.concoursId%>',
//        'EventId': $('#eventID').val(),
//        'UserAddedByAdmin': 'True'
//    }

//    $.magnificPopup.close();
//    addUserMember(dataRow, roleId);

//    return false;
//});

//$('#btnAddHRMember').click(function () {
//    var dataRow = {
//        'UserId': '<%=usager.usagerGUID.ToString()%>',
//        'EventId': $('#eventID').val(),
//        'ConcoursId': '<%=Contest.concoursId%>',
//        'UserAddedByAdmin': 'True'
//    }
//    addUserMember(dataRow, RoleEnum.HR);
//    $('#btnAddHRMember').hide();
//    $('#btnRemoveHRMember').show();

//    return false;
//});

//$('#btnRemoveHRMember').click(function () {
//    var dataRow = {
//        'PlageHoraireId': $('#eventID').val(),
//        'UserId': '<%=usager.usagerGUID.ToString()%>'
//    }
//    removeUser(dataRow, null);
//    $('#btnAddHRMember').show();
//    $('#btnRemoveHRMember').hide();

//    return false;
//});

//$('#btnAddUserLanguage').click(function (e) {

//    var dataRow = {
//        'RoleUserId': '<%=RoleUser.userSettingId.ToString()%>',
//        'UserLanguageId': $('#ddlUserLanguage').val()
//    }

//    $.ajax({
//        type: 'POST',
//        url: "FullCalender.asmx/AddUserLanguage",
//        data: dataRow,
//        beforeSend: function () {
//            $('.spinner').show();
//        },
//        success: function (response) {
//            if (response == 'true') {
//                ClearPopupFormValues();
//                $.notify({
//                    // options
//                    message: '<%=GetGlobalResourceObject("Notify", "NotifyAddUserLanguageSuccess").ToString()%>'
//                },
//                    {
//                        // settings
//                        element: "body",
//                        type: "success",
//                        allow_dismiss: true,
//                        placement: {
//                            from: "top",
//                            align: "center",
//                        },
//                        animate: {
//                            enter: "animated fadeInDown",
//                            exit: "animated fadeOutUp",
//                        },
//                        icon_type: "class"
//                    });
//            }
//            else {
//                $.notify({
//                    // options
//                    message: '<%=GetGlobalResourceObject("Notify", "NotifyAddUserLanguageFail").ToString()%>'
//                },
//                    {
//                        // settings
//                        element: "body",
//                        type: "danger",
//                        allow_dismiss: true,
//                        placement: {
//                            from: "top",
//                            align: "center",
//                        },
//                        animate: {
//                            enter: "animated fadeInDown",
//                            exit: "animated fadeOutUp",
//                        },
//                        icon_type: "class"
//                    });
//            }
//        },
//        error: function (XMLHttpRequest, textStatus, errorThrown) {
//            $.notify({
//                // options
//                message: '<%=GetGlobalResourceObject("Notify", "NotifyAddUserLanguageFail").ToString()%>'
//            },
//                {
//                    // settings
//                    element: "body",
//                    type: "danger",
//                    allow_dismiss: true,
//                    placement: {
//                        from: "top",
//                        align: "center",
//                    },
//                    animate: {
//                        enter: "animated fadeInDown",
//                        exit: "animated fadeOutUp",
//                    },
//                    icon_type: "class"
//                });
//        },
//        complete: function (data) {
//            $('.spinner').hide();
//        }
//    });
//});

//$('#btnAcceptPrivacyStatement').click(function (e) {

//    var dataRow = {
//        'RoleUserId': '<%=RoleUser.userSettingId.ToString()%>',
//    }

//    $.ajax({
//        type: 'POST',
//        url: "FullCalender.asmx/SetPrivacyStatement",
//        data: dataRow,
//        beforeSend: function () {
//            $('.spinner').show();
//        },
//        success: function (response) {
//            if (response == 'true') {
//                wb.doc.trigger("open.wb-lbx", [[{ src: "#popupUserLanguage", type: "inline" }], true]);
//            }
//        },
//        complete: function (data) {
//            $('.spinner').hide();
//        }
//    });
//});
//            }

//function ShowEventPopup(date) {
//    wb.doc.trigger("open.wb-lbx", [[{ src: "#popupEventForm", type: "inline" }], true]);

//    $('#ddlCandidates, #ddlInterviewers, #ddlLeads').find('option:selected').removeAttr('selected');
//    $('input:radio[name=roles]').removeAttr('checked'); // unselect all checked
//    $('input:radio[name=roles]:nth(0)').prop('checked', true); // select first child

//    var ddlCandidates = $('#ddlCandidates').show();
//    var ddlInterviewers = $('#ddlInterviewers').hide();
//    var ddlLeads = $('#ddlLeads').hide();

//    $("input:radio[name='roles']").click(function () {

//        var role = $("input:radio[name='roles']:checked").val();

//        if (role == '<%=GetGlobalResourceObject("global", "candidate")%>') {
//            ddlCandidates.show();
//            ddlInterviewers.hide();
//            ddlLeads.hide();
//        }
//        else if (role == '<%=GetGlobalResourceObject("global", "interviewer")%>') {
//            ddlCandidates.hide();
//            ddlInterviewers.show();
//            ddlLeads.hide();
//        }
//        else if (role == '<%=GetGlobalResourceObject("global", "lead")%>') {
//            ddlCandidates.hide();
//            ddlInterviewers.hide();
//            ddlLeads.show();
//        }
//    });

//    $('#eventLocation').focus();
//}

//function ClearPopupFormValues() {
//    $('#eventID').val("");
//    $('#timezone').val("");
//    $('#eventLocation').val("");
//    $('#eventSalle').val("");
//    $('#eventContactName').val("");
//    $('#eventContactNumber').val("");
//    $('#eventDateTime').val("");
//    $('#eventDuration').val("");
//}

//function HidePupupAddUserLabels() {
//    $('#AddMemberAvailability').hide();
//    $('#AddCandidateAvailability').hide();
//    $('#ddlAdjointDe').hide();
//}

//function addUserMember(dataRow, roleId) {

//    // vérifier si l'usager est un candidat
//    if (roleId == RoleEnum.CANDIDATE) {
//        $.ajax({
//            type: 'POST',
//            url: "FullCalender.asmx/IsCandidateIsAlreadyBooked",
//            data: dataRow,
//            success: function (response) {
//                if (response == 'true') // candidat déjà inscrit dans une plage horaire
//                {
//                    // candidat déjà inscrit
//                    $.notify({
//                        // options
//                        message: '<%=GetGlobalResourceObject("Notify", "NotifyCandidateAlreadyBooked").ToString()%>'
//                    },
//                        {
//                            // settings
//                            element: "body",
//                            type: "danger",
//                            allow_dismiss: true,
//                            placement: {
//                                from: "top",
//                                align: "center",
//                            },
//                            animate: {
//                                enter: "animated fadeInDown",
//                                exit: "animated fadeOutUp",
//                            },
//                            icon_type: "class"
//                        });
//                }
//                else {
//                    $.ajax({
//                        type: 'POST',
//                        url: "FullCalender.asmx/IsAlreadyHaveUser",
//                        data: dataRow,
//                        success: function (response) {
//                            if (response == 'false') // usager non présent dans la plage horaire
//                            {
//                                if (confirm('<%=GetLocalResourceObject("AddCandidate")%>')) {
//                                    $.ajax({
//                                        type: 'POST',
//                                        url: "FullCalender.asmx/AddUser",
//                                        data: dataRow,
//                                        success: function (response) {
//                                            if (response == 'true') {
//                                                $.notify({
//                                                    // options
//                                                    message: '<%=GetGlobalResourceObject("Notify", "NotifyCandidateAddedSuccess").ToString()%>'
//                                                },
//                                                    {
//                                                        // settings
//                                                        element: "body",
//                                                        type: "success",
//                                                        allow_dismiss: true,
//                                                        placement: {
//                                                            from: "top",
//                                                            align: "center",
//                                                        },
//                                                        animate: {
//                                                            enter: "animated fadeInDown",
//                                                            exit: "animated fadeOutUp",
//                                                        },
//                                                        icon_type: "class"
//                                                    });
//                                            }
//                                            else {
//                                                $.notify({
//                                                    // options
//                                                    message: '<%=GetGlobalResourceObject("Notify", "NotifyCandidateAddedFailed").ToString()%>'
//                                                },
//                                                    {
//                                                        // settings
//                                                        element: "body",
//                                                        type: "danger",
//                                                        allow_dismiss: true,
//                                                        placement: {
//                                                            from: "top",
//                                                            align: "center",
//                                                        },
//                                                        animate: {
//                                                            enter: "animated fadeInDown",
//                                                            exit: "animated fadeOutUp",
//                                                        },
//                                                        icon_type: "class"
//                                                    });
//                                            }
//                                        },
//                                        error: function (XMLHttpRequest, textStatus, errorThrown) {
//                                            $.notify({
//                                                // options
//                                                message: '<%=GetGlobalResourceObject("Notify", "NotifyCandidateAddedFailed").ToString()%>'
//                                            },
//                                                {
//                                                    // settings
//                                                    element: "body",
//                                                    type: "danger",
//                                                    allow_dismiss: true,
//                                                    placement: {
//                                                        from: "top",
//                                                        align: "center",
//                                                    },
//                                                    animate: {
//                                                        enter: "animated fadeInDown",
//                                                        exit: "animated fadeOutUp",
//                                                    },
//                                                    icon_type: "class"
//                                                });
//                                        },
//                                        complete: function (data) {
//                                            $('#calendar').fullCalendar('refetchEvents');
//                                        }
//                                    });
//                                }
//                            }
//                            else {
//                                // usager déjà présent dans la plage horaire
//                                $.notify({
//                                    // options
//                                    message: '<%=GetGlobalResourceObject("Notify", "NotifyCandidateAlreadyIn").ToString()%>'
//                                },
//                                    {
//                                        // settings
//                                        element: "body",
//                                        type: "danger",
//                                        allow_dismiss: true,
//                                        placement: {
//                                            from: "top",
//                                            align: "center",
//                                        },
//                                        animate: {
//                                            enter: "animated fadeInDown",
//                                            exit: "animated fadeOutUp",
//                                        },
//                                        icon_type: "class"
//                                    });
//                            }
//                        },
//                        error: function (XMLHttpRequest, textStatus, errorThrown) {
//                            $.notify({
//                                // options
//                                message: '<%=GetGlobalResourceObject("Notify", "NotifyCandidateAddedFailed").ToString()%>'
//                            },
//                                {
//                                    // settings
//                                    element: "body",
//                                    type: "danger",
//                                    allow_dismiss: true,
//                                    placement: {
//                                        from: "top",
//                                        align: "center",
//                                    },
//                                    animate: {
//                                        enter: "animated fadeInDown",
//                                        exit: "animated fadeOutUp",
//                                    },
//                                    icon_type: "class"
//                                });
//                        }
//                    });
//                }
//            }
//        });
//    }
//    else {
//        $.ajax({
//            type: 'POST',
//            url: "FullCalender.asmx/IsAlreadyHaveUser",
//            data: dataRow,
//            success: function (response) {
//                if (response == 'false') // usager non présent dans la plage horaire
//                {
//                    if (confirm('<%=GetLocalResourceObject("AddCandidate")%>')) {
//                                    <% --var dataRow = {
//                            'UserId': '<%=usager.usagerGUID.ToString()%>',
//                            'EventId': eventId,
//                            'UserAddedByAdmin': 'False'
//                        }-- %>

//                            $.ajax({
//                                type: 'POST',
//                                url: "FullCalender.asmx/AddUser",
//                                data: dataRow,
//                                success: function (response) {
//                                    if (response == 'true') {
//                                        $.notify({
//                                            // options
//                                            message: '<%=GetGlobalResourceObject("Notify", "NotifyCandidateAddedSuccess").ToString()%>'
//                                        },
//                                            {
//                                                // settings
//                                                element: "body",
//                                                type: "success",
//                                                allow_dismiss: true,
//                                                placement: {
//                                                    from: "top",
//                                                    align: "center",
//                                                },
//                                                animate: {
//                                                    enter: "animated fadeInDown",
//                                                    exit: "animated fadeOutUp",
//                                                },
//                                                icon_type: "class"
//                                            });
//                                    }
//                                    else {
//                                        $.notify({
//                                            // options
//                                            message: '<%=GetGlobalResourceObject("Notify", "NotifyCandidateAddedFailed").ToString()%>'
//                                        },
//                                            {
//                                                // settings
//                                                element: "body",
//                                                type: "danger",
//                                                allow_dismiss: true,
//                                                placement: {
//                                                    from: "top",
//                                                    align: "center",
//                                                },
//                                                animate: {
//                                                    enter: "animated fadeInDown",
//                                                    exit: "animated fadeOutUp",
//                                                },
//                                                icon_type: "class"
//                                            });
//                                    }
//                                },
//                                error: function (XMLHttpRequest, textStatus, errorThrown) {
//                                    $.notify({
//                                        // options
//                                        message: '<%=GetGlobalResourceObject("Notify", "NotifyCandidateAddedFailed").ToString()%>'
//                                    },
//                                        {
//                                            // settings
//                                            element: "body",
//                                            type: "danger",
//                                            allow_dismiss: true,
//                                            placement: {
//                                                from: "top",
//                                                align: "center",
//                                            },
//                                            animate: {
//                                                enter: "animated fadeInDown",
//                                                exit: "animated fadeOutUp",
//                                            },
//                                            icon_type: "class"
//                                        });
//                                },
//                                complete: function (data) {
//                                    $('#calendar').fullCalendar('refetchEvents');
//                                }
//                            });
//                    }
//                }
//                else {
//                    // usager déjà présent dans la plage horaire
//                    $.notify({
//                        // options
//                        message: '<%=GetGlobalResourceObject("Notify", "NotifyCandidateAlreadyIn").ToString()%>'
//                    },
//                        {
//                            // settings
//                            element: "body",
//                            type: "danger",
//                            allow_dismiss: true,
//                            placement: {
//                                from: "top",
//                                align: "center",
//                            },
//                            animate: {
//                                enter: "animated fadeInDown",
//                                exit: "animated fadeOutUp",
//                            },
//                            icon_type: "class"
//                        });
//                }
//            }
//        });
//    }
//}

//function addUserAssistant(dataRow, calEvent) {

//    HidePupupAddUserLabels();
//    $('#AddAssistantAvailability').show();
//    $('#ddlAdjointDe').show();

//    $('#ddlAdjointDe').find('option:eq(0)').prop('selected', true);
//    $('#btnApply').prop("disabled", true);

//    $('#ddlAdjointDe').change(function (e) {
//        if (this.selectedIndex == 0) {
//            $('#btnApply').prop("disabled", true);
//        }
//        else {
//            $('#btnApply').prop("disabled", false);
//        }
//    });

//    $('#btnApply').click(function (e) {

//        // exécuter l'événement seulement une fois
//        $(this).off(e);

//        dataRow.UserId = $('#ddlAdjointDe').val();

//        if (calEvent.users.length == 0 || (calEvent.users.length > 0 && calEvent.users.toString().indexOf($('#ddlAdjointDe option:selected').html()) == -1)) {
//            $.ajax({
//                type: 'POST',
//                url: "FullCalender.asmx/AddUser",
//                data: dataRow,
//                beforeSend: function () {
//                    $('.spinner').show();
//                },
//                success: function (response) {
//                    if (response == 'true') {
//                        $.notify({
//                            // options
//                            message: '<%=GetGlobalResourceObject("Notify", "NotifyCommiteeMemberAddedSuccess").ToString()%>'
//                        },
//                            {
//                                // settings
//                                element: "body",
//                                type: "success",
//                                allow_dismiss: true,
//                                placement: {
//                                    from: "top",
//                                    align: "center",
//                                },
//                                animate: {
//                                    enter: "animated fadeInDown",
//                                    exit: "animated fadeOutUp",
//                                },
//                                icon_type: "class"
//                            });
//                    }
//                    else {
//                        $.notify({
//                            // options
//                            message: '<%=GetGlobalResourceObject("Notify", "NotifyCommiteeMemberAddedFail").ToString()%>'
//                        },
//                            {
//                                // settings
//                                element: "body",
//                                type: "danger",
//                                allow_dismiss: true,
//                                placement: {
//                                    from: "top",
//                                    align: "center",
//                                },
//                                animate: {
//                                    enter: "animated fadeInDown",
//                                    exit: "animated fadeOutUp",
//                                },
//                                icon_type: "class"
//                            });
//                    }
//                },
//                error: function (XMLHttpRequest, textStatus, errorThrown) {
//                    $.notify({
//                        // options
//                        message: '<%=GetGlobalResourceObject("Notify", "NotifyCommiteeMemberAddedFail").ToString()%>'
//                    },
//                        {
//                            // settings
//                            element: "body",
//                            type: "danger",
//                            allow_dismiss: true,
//                            placement: {
//                                from: "top",
//                                align: "center",
//                            },
//                            animate: {
//                                enter: "animated fadeInDown",
//                                exit: "animated fadeOutUp",
//                            },
//                            icon_type: "class"
//                        });
//                },
//                complete: function (data) {
//                    $('#calendar').fullCalendar('refetchEvents');
//                    $('.spinner').hide();
//                }
//            });
//        }
//        else {
//            $.notify({
//                // options
//                message: '<%=GetGlobalResourceObject("Notify", "NotifyMemberAlreadyIn").ToString()%>'
//            },
//                {
//                    // settings
//                    element: "body",
//                    type: "danger",
//                    allow_dismiss: true,
//                    placement: {
//                        from: "top",
//                        align: "center",
//                    },
//                    animate: {
//                        enter: "animated fadeInDown",
//                        exit: "animated fadeOutUp",
//                    },
//                    icon_type: "class"
//                });
//        }
//    });
//}

//function addUserInterviewers(dataRow) {
//    $.ajax({
//        type: 'POST',
//        url: "FullCalender.asmx/AddUser",
//        data: dataRow,
//        beforeSend: function () {
//            $('.spinner').show();
//        },
//        success: function (response) {
//            if (response == 'true') {
//                $.notify({
//                    // options
//                    message: '<%=GetGlobalResourceObject("Notify", "NotifyCommiteeMemberAddedSuccess").ToString()%>'
//                },
//                    {
//                        // settings
//                        element: "body",
//                        type: "success",
//                        allow_dismiss: true,
//                        placement: {
//                            from: "top",
//                            align: "center",
//                        },
//                        animate: {
//                            enter: "animated fadeInDown",
//                            exit: "animated fadeOutUp",
//                        },
//                        icon_type: "class"
//                    });
//            }
//            else {
//                $.notify({
//                    // options
//                    message: '<%=GetGlobalResourceObject("Notify", "NotifyCommiteeMemberAddedFail").ToString()%>'
//                },
//                    {
//                        // settings
//                        element: "body",
//                        type: "danger",
//                        allow_dismiss: true,
//                        placement: {
//                            from: "top",
//                            align: "center",
//                        },
//                        animate: {
//                            enter: "animated fadeInDown",
//                            exit: "animated fadeOutUp",
//                        },
//                        icon_type: "class"
//                    });
//            }
//        },
//        error: function (XMLHttpRequest, textStatus, errorThrown) {
//            $.notify({
//                // options
//                message: '<%=GetGlobalResourceObject("Notify", "NotifyCommiteeMemberAddedFail").ToString()%>'
//            },
//                {
//                    // settings
//                    element: "body",
//                    type: "danger",
//                    allow_dismiss: true,
//                    placement: {
//                        from: "top",
//                        align: "center",
//                    },
//                    animate: {
//                        enter: "animated fadeInDown",
//                        exit: "animated fadeOutUp",
//                    },
//                    icon_type: "class"
//                });
//        },
//        complete: function (data) {
//            $('#calendar').fullCalendar('refetchEvents');
//            $('.spinner').hide();
//        }
//    });
//}

//function removeUser(dataRow, element) {

//    $.ajax({
//        type: 'POST',
//        url: "FullCalender.asmx/RemoveUser",
//        data: dataRow,
//        beforeSend: function () {
//            $('.spinner').show();
//        },
//        success: function (response) {
//            if (response == 'true') {
//                $.notify({
//                    // options
//                    message: '<%=GetGlobalResourceObject("Notify", "NotifyRemoveMemberSuccess").ToString()%>'
//                },
//                    {
//                        // settings
//                        element: "body",
//                        type: "success",
//                        allow_dismiss: true,
//                        placement: {
//                            from: "top",
//                            align: "center",
//                        },
//                        animate: {
//                            enter: "animated fadeInDown",
//                            exit: "animated fadeOutUp",
//                        },
//                        icon_type: "class"
//                    });
//            }
//            else {
//                $.notify({
//                    // options
//                    message: '<%=GetGlobalResourceObject("Notify", "NotifyRemoveMemberFail").ToString()%>'
//                },
//                    {
//                        // settings
//                        element: "body",
//                        type: "danger",
//                        allow_dismiss: true,
//                        placement: {
//                            from: "top",
//                            align: "center",
//                        },
//                        animate: {
//                            enter: "animated fadeInDown",
//                            exit: "animated fadeOutUp",
//                        },
//                        icon_type: "class"
//                    });
//            }
//        },
//        error: function (XMLHttpRequest, textStatus, errorThrown) {
//            $.notify({
//                // options
//                message: '<%=GetGlobalResourceObject("Notify", "NotifyRemoveMemberFail").ToString()%>'
//            },
//                {
//                    // settings
//                    element: "body",
//                    type: "danger",
//                    allow_dismiss: true,
//                    placement: {
//                        from: "top",
//                        align: "center",
//                    },
//                    animate: {
//                        enter: "animated fadeInDown",
//                        exit: "animated fadeOutUp",
//                    },
//                    icon_type: "class"
//                });
//        },
//        complete: function (data) {
//            if (element)
//                element.qtip('hide');
//            $('#calendar').fullCalendar('refetchEvents');
//            $('.spinner').hide();
//        }
//    });
//}