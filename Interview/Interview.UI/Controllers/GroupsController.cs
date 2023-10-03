using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Groups;
using Interview.UI.Services.DAL;
using Interview.UI.Services.Mock.Identity;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Interview.UI.Controllers
{
    
    public class GroupsController : BaseController
    {

        #region Declarations

        private readonly IMapper _mapper;
        private readonly IState _state;

        #endregion

        #region Constructors

        public GroupsController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IOptions<JusticeOptions> justiceOptions, IState state) 
            : base(modelAccessor, justiceOptions, dal)
        {
            _mapper = mapper;
            _state = state;
        }

        #endregion

        #region Public Index Methods

        [HttpGet]
        public IActionResult Index()
        {
            
            return View();

        }

        #endregion

        #region Add Group

        [HttpGet]
        public IActionResult AddGroup()
        {

            VmAddGroup result = new VmAddGroup();

            ViewBag.ShowAddGroupPartial = true;

            return View("Index", result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGroup(VmAddGroup vmAddGroup)
        {

            if (ModelState.IsValid)
            {

                Guid groupId;
                Group group = new Group()
                {
                    ContestId = (Guid)_state.ContestId,
                    NameEn = vmAddGroup.NameEn,
                    NameFr = vmAddGroup.NameFr,
                };
                group.GroupOwners.Add(new GroupOwner()
                {
                    UserId = (Guid)LoggedInMockUser.Id
                });
                groupId = await _dal.AddEntity(group);

                AddRole(MockLoggedInUserRoles.Owner, (Guid)LoggedInMockUser.Id);

                return RedirectToAction("Index");

            }
            else
            {
                ViewBag.ShowAddGroupPartial = true;

                return View("Index", vmAddGroup);
            }

        }

        private void AddRole(MockLoggedInUserRoles mockLoggedInUserRole, Guid loggedInUserId)
        {



        }

        #endregion

    }

}
