using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Default;
//using Interview.UI.Models.Groups;
using Interview.UI.Services.DAL;
using Interview.UI.Services.Mock.Identity;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;

namespace Interview.UI.Controllers
{
    public class DefaultController : BaseController
    {

        #region Declarations

        private readonly IMapper _mapper;
        private readonly IState _state;

        #endregion

        #region Constructors

        public DefaultController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IState state, IOptions<JusticeOptions> justiceOptions) 
            : base(modelAccessor, justiceOptions, dal)
        {
            _mapper = mapper;
            _state = state;
        }

        #endregion

        #region Public Index Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            VmIndex result = new VmIndex();
            List<Contest> contests = await GetContestsForLoggedInUser();
            Guid? contestId = null;

            // Look to Session for ContestId
            if (_state.ContestId != null)
                contestId = _state.ContestId;
            // Look to first item in list if _state.ContestId isn't set by user
            else if (contests.Any())
                contestId = contests.First().Id;

            await SetIndexViewBag(contests, contestId);
            RegisterIndexClientResources();
            _state.ContestId = contestId;
            result.ContestId = contestId;
            
            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SwitchContest(Guid contestId)
        {

            _state.ContestId = contestId;

            return RedirectToAction("Index");

        }

		private async Task SetIndexViewBag(List<Contest> contests, Guid? contestId)
        {

            ViewBag.Contests = contests;
            ViewBag.ContestId = contestId;

            if (contestId != null)
            {

                Contest contest = contests.Where(x => x.Id == contestId).First();
				List<Interview.Entities.Interview> interviews = await _dal.GetInterViewsByContestId((Guid)contestId);
                List<VmInterview> vmInterviews = _mapper.Map<List<VmInterview>>(interviews);

				ViewBag.ContestStartDate = contest.StartDate;
				ViewBag.ContestEndDate = contest.EndDate;
				ViewBag.VmInterviews = vmInterviews;

            }

        }

        private void RegisterIndexClientResources()
        {

            WebTemplateModel.HTMLHeaderElements.Add("<link rel=\"stylesheet\" href=\"/lib/Magnific-Popup-master/Magnific-Popup-master/dist/magnific-popup.css\" />");
            WebTemplateModel.HTMLBodyElements.Add("<script src=\"/lib/Magnific-Popup-master/Magnific-Popup-master/dist/jquery.magnific-popup.min.js\"></script>");

            // These datatable libraries will be removed once the WET Calendar is working.           
            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/Default/InterviewModal.js?v={BuildId}\"></script>");
            //WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/Default/InterviewUsersModal.js?v={BuildId}\"></script>");

        }

        #endregion

        #region Interview Modal

        [HttpGet]
        public async Task<PartialViewResult> InterViewModal(Guid? id)
        {

            VmInterview result = null;
            Interview.Entities.Interview interview = null;

            if (id == null)
                result = new VmInterview() { ContestId = (Guid)_state.ContestId };
            else
            {
                interview = await _dal.GetEntity<Interview.Entities.Interview>((Guid)id, true) as Interview.Entities.Interview;
                result = _mapper.Map<VmInterview>(interview);

                result.VmInterviewerUserIds.CandidateUserId = interview.InterviewUsers.Where(x => x.RoleType == RoleTypes.Candidate).FirstOrDefault()?.UserId;
                result.VmInterviewerUserIds.InterviewerUserId = interview.InterviewUsers.Where(x => x.RoleType == RoleTypes.Interviewer).FirstOrDefault()?.UserId;
                result.VmInterviewerUserIds.InterviewerLeadUserId = interview.InterviewUsers.Where(x => x.RoleType == RoleTypes.Lead).FirstOrDefault()?.UserId;
                result.VmInterviewerUserIds.InterviewId = id;
            }

			await SetInterviewModalViewBag(interview);

			return PartialView(result);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InterviewModal(VmInterview vmInterview)
        {

            if (ModelState.IsValid)
            {

                Interview.Entities.Interview interview = _mapper.Map<Interview.Entities.Interview>(vmInterview);

                // Handle Interview
                interview.ContestId = (Guid)_state.ContestId;
                if (vmInterview.Id == null)
                {
                    vmInterview.Id = await _dal.AddEntity<Interview.Entities.Interview>(interview);
                }
                else
                {
                    await _dal.UpdateEntity(interview);

                    // Handle Users
                    List<InterviewUser> dbInterviewUsers = await _dal.GetInterviewUsersByInterviewId((Guid)vmInterview.Id);
                    await ResolveInterviewUser(vmInterview.VmInterviewerUserIds.CandidateUserId, dbInterviewUsers, RoleTypes.Candidate, (Guid)vmInterview.Id);
                    await ResolveInterviewUser(vmInterview.VmInterviewerUserIds.InterviewerUserId, dbInterviewUsers, RoleTypes.Interviewer, (Guid)vmInterview.Id);
                    await ResolveInterviewUser(vmInterview.VmInterviewerUserIds.InterviewerLeadUserId, dbInterviewUsers, RoleTypes.Lead, (Guid)vmInterview.Id);

                }

                return new JsonResult(new { result = true, item = vmInterview })
                {
                    StatusCode = 200
                };

            }
            else
            {
                Interview.Entities.Interview interview = vmInterview.Id == null ? null : await _dal.GetEntity<Interview.Entities.Interview>((Guid)vmInterview.Id, true) as Interview.Entities.Interview;

                await SetInterviewModalViewBag(interview);

                return PartialView(vmInterview);
            }

        }

        [HttpGet]
        public async Task<ActionResult> InterviewDelete(Guid id)
        {

            await _dal.DeleteEntity<Interview.Entities.Interview>(id);

            return RedirectToAction("Index");

        }

        private async Task SetInterviewModalViewBag(Interview.Entities.Interview? interview)
        {

            // Handle Interview Schedule
            if (interview != null)
            {

                List<Schedule> schedules = await _dal.GetSchedulesByContestId(interview.ContestId);
                TimeSpan startTime = interview.StartDate.TimeOfDay;
                TimeSpan candidateArrival = new TimeSpan(0, (int)schedules.Where(x => x.ScheduleType == ScheduleTypes.Candidate).First().StartValue, 0);
                TimeSpan interviewerArrival = new TimeSpan(0, (int)schedules.Where(x => x.ScheduleType == ScheduleTypes.Members).First().StartValue, 0);
                TimeSpan marking = new TimeSpan(0, (int)schedules.Where(x => x.ScheduleType == ScheduleTypes.Marking).First().StartValue, 0);

                ViewBag.CandidateArrival = candidateArrival.Add(startTime).ToString(@"hh\:mm");
                ViewBag.InterviewerArrival = interviewerArrival.Add(startTime).ToString(@"hh\:mm");
                ViewBag.Marking = marking.Add(startTime).ToString(@"hh\:mm"); ;

            }
            else
            {
                ViewBag.CandidateArrival = string.Empty;
                ViewBag.InterviewerArrival = string.Empty;
                ViewBag.Marking = string.Empty;
            }

            // Handle Interview Start and End Dates
            Contest contest = await _dal.GetEntity<Contest>((Guid)_state.ContestId) as Contest;
			ViewBag.ContestStartDate = contest.StartDate;
			ViewBag.ContestEndDate = contest.EndDate;

			// Handle Interview Users
			List<RoleUser> roleUsers = await _dal.GetRoleUsersByContestId((Guid)_state.ContestId);
			if (IsLoggedInMockUserInRole(MockLoggedInUserRoles.Admin) || IsLoggedInMockUserInRole(MockLoggedInUserRoles.Owner) || IsLoggedInMockUserInRole(MockLoggedInUserRoles.System))
			{
				bool hasAccess = true;
				if (IsLoggedInMockUserInRole(MockLoggedInUserRoles.Owner))
				{
					// Despit the above line's dal call returning a list, it treats the returned type as a single entity, so need to 
					// get the list as a variable first. Moving on...
					var groupOwners = await _dal.GetGroupOwnersByContextIdAndUserId((Guid)_state.ContestId, (Guid)LoggedInMockUser.Id);
					hasAccess = groupOwners.Any();
				}
			}

			ViewBag.CandidateUsers = roleUsers.Where(x => x.RoleType == RoleTypes.Candidate).ToList();
			ViewBag.InterviewerUsers = roleUsers.Where(x => x.RoleType == RoleTypes.Interviewer).ToList();
			ViewBag.LeadUsers = roleUsers.Where(x => x.RoleType == RoleTypes.Lead).ToList();

		}

        private async Task ResolveInterviewUser(Guid? postedUserId, List<InterviewUser> dbInterviewUsers, RoleTypes roleType, Guid interviewId)
        {

            InterviewUser dbInterviewUser = dbInterviewUsers.Where(x => x.RoleType == roleType).FirstOrDefault();

            if (dbInterviewUser == null && postedUserId != null)
            {
                // Add
                InterviewUser newInterviewUser = new InterviewUser()
                {
                    UserId = (Guid)postedUserId,
                    RoleType = roleType,
                    InterviewId = interviewId
                };
                await _dal.AddEntity<InterviewUser>(newInterviewUser);
            }
            else if ((dbInterviewUser != null && postedUserId != null) && (dbInterviewUser.Id != postedUserId))
            {
                // Update
                InterviewUser newInterviewUser = new InterviewUser()
                {
                    UserId = (Guid)postedUserId,
                    RoleType = roleType,
                    InterviewId = interviewId
                };
                await _dal.DeleteEntity(dbInterviewUser);
                await _dal.AddEntity<InterviewUser>(newInterviewUser);
            }
            else if (dbInterviewUser != null && postedUserId == null)
            {
                // Delete
                await _dal.DeleteEntity(dbInterviewUser);
            }

        }

        #endregion

    }
}
