using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.AppSettings;
using Interview.UI.Services.DAL;
using Interview.UI.Services.Mock.Identity;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
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

            List<Contest> contests = null;
            Guid? contestId = null;

            if (IsLoggedInMockUserInRole(MockLoggedInUserRoles.Admin) || IsLoggedInMockUserInRole(MockLoggedInUserRoles.System))
                contests = await _dal.GetAllContests();
            else if (IsLoggedInMockUserInRole(MockLoggedInUserRoles.Owner))
                contests = await _dal.GetContestsForGroupOwner((Guid)LoggedInMockUser.Id);
            else
                contests = await _dal.GetContestsForUserSettingsUser((Guid)LoggedInMockUser.Id);
            contests.OrderByDescending(x => x.CreatedDate);

            if (contests.Any())
                _state.ContestId = contests.First().Id;

            // Look to Session for ContestId
            if (_state.ContestId != null)
                contestId = _state.ContestId;
            // Look to first item in list if _state.ContestId isn't set by user
            else if (contests.Any())
            {
                contestId = contests.First().Id;
                _state.ContestId = contestId;
            }

            ViewBag.Contests = contests;
            ViewBag.ContestId = contestId;
            if (contestId != null)
                await SetIndexViewBag(contests.Where(x => x.Id == contestId).First());

            return View();

        }

        [HttpGet]
        public async Task<IActionResult> SwitchContest(Guid contestId)
        {

            _state.ContestId = contestId;

            return RedirectToAction("Index");

        }

        private async Task SetIndexViewBag(Contest contest)
        {

            // This method mimics the Entrevue.Default.SetCalendar() method. That method has a few concerns:
            // 1. Populating dropdowns
            // 2. Setting the visibility of various form elements
            // 3. Setting the wording of various form elements
            // Within an MVC framework, these concerns are handled at the View layer (not the Controller layer).
            // However, the Controller here will set the items for dropdowns in viewbag.

            UserSetting userSetting = await _dal.GetUserSettingByContestIdAndUserId(contest.Id, (Guid)LoggedInMockUser.Id);
            List<MockUser> mockUsers = new List<MockUser>();

            if (IsLoggedInMockUserInRole(MockLoggedInUserRoles.Admin) || IsLoggedInMockUserInRole(MockLoggedInUserRoles.Owner) || IsLoggedInMockUserInRole(MockLoggedInUserRoles.System))
            {
                bool hasAccess = true;
                if (IsLoggedInMockUserInRole(MockLoggedInUserRoles.Owner))
                {
                    //hasAccess = await _dal.GetGroupOwnersByContextIdAndUserId(contest.Id, (Guid)LoggedInMockUser.Id).Any();
                    // Despit the above line's dal call returning a list, it treats the returned type as a single entity, so need to 
                    // get the list as a variable first. Moving on...
                    var groupOwners = await _dal.GetGroupOwnersByContextIdAndUserId(contest.Id, (Guid)LoggedInMockUser.Id);
                    hasAccess = groupOwners.Any();
                }

                if (hasAccess)
                {
                    userSetting = new UserSetting()
                    {
                        ContestId = contest.Id,
                        RoleType = RoleTypes.Admin,
                        UserId = (Guid)LoggedInMockUser.Id,
                        LanguageType = LanguageTypes.Bilingual,
                        HasAcceptedPrivacyStatement = true
                    };
                    await _dal.AddEntity<UserSetting>(userSetting);
                }
            }

            // Handle Users by RoleType
            foreach (UserSetting contestUserSetting in contest.UserSettings)
                mockUsers.Add(await _dal.GetMockUserById(contestUserSetting.UserId));

            ViewBag.CandidateUsers = mockUsers.Where(x => x.RoleType == RoleTypes.Candidate).ToList();
            ViewBag.InterviewerUsers = mockUsers.Where(x => x.RoleType == RoleTypes.Interviewer).ToList();
            ViewBag.LeadUsers = mockUsers.Where(x => x.RoleType == RoleTypes.Lead).ToList();

            // Handle Users as Members?
            if (userSetting.RoleType == RoleTypes.Assistant)
            {
                mockUsers.Clear();

                // Look at Entrevue.SDefault.SetCalendar lines 287 - 319

            }

        }

        #endregion

        #region Interview Modal

        [HttpGet]
        public PartialViewResult InterviewModal()
        {

            VmInterview result = new VmInterview();

            return PartialView(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> InterviewModal(VmInterview vmInterview)
        {

            if (ModelState.IsValid)
            {

                return new JsonResult(new { result = true, item = vmInterview })
                {
                    StatusCode = 200
                };

            }
            else
            {
                return null;
            }

        }

        #endregion

        #region Add User Modal

        [HttpGet]
        public async Task<PartialViewResult> AddUserModal()
        {

            return null;

        }

        #endregion

        #region User Language Modal

        [HttpGet]
        public async Task<PartialViewResult> UserLanguageModal()
        {

            return null;

        }

        #endregion

        #region Privacy Statement Modal

        [HttpGet]
        public async Task<PartialViewResult> PrivacyStatementModal()
        {

            return null;

        }

        #endregion

    }
}
