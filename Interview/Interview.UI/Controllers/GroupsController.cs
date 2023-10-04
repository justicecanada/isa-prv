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

        private const string _showAddGroupPartial = "SHOW_ADD_GROUP_PARTIAL";
        private const string _groupIdToEdit = "GROUP_ID_TO_EDIT";

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
        public async Task<IActionResult> Index()
        {

            VmIndex vmIndex = new VmIndex();
            List<Group> groups = null;

            // Hanld groups
            if (IsLoggedInMockUserInRole(MockLoggedInUserRoles.Admin) || IsLoggedInMockUserInRole(MockLoggedInUserRoles.System))
                groups = await _dal.GetGroups(null);
            else
                groups = await _dal.GetGroups(LoggedInMockUser.Id);
            vmIndex.Groups = _mapper.Map<List<VmGroup>>(groups);
            await PopulateGroupOwnersWithMockUser(vmIndex.Groups);                    // Ugly

            // Handle Add group
            if (TempData[_showAddGroupPartial] != null && (bool)TempData[_showAddGroupPartial])
            {
                vmIndex.AddGroup = true;
                vmIndex.VmAddGroup = new VmAddGroup();
            }
            // Handle Edit group
            if (TempData[_groupIdToEdit] != null)
                vmIndex.Groups.Where(x => x.Id == (Guid)TempData[_groupIdToEdit]).First().EditThisGroup = true;
            
            await IndexSetViewBag();
            IndexRegisterClientResources();

            return View(vmIndex);

        }

        [HttpGet]
        public async Task<IActionResult> DeleteGroup(Guid id)
        {

            Group group = await _dal.GetEntity<Group>(id) as Group;

            group.IsDeleted = true;
            await _dal.UpdateEntity(group);

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> EditGroup(Guid id)
        {

            TempData[_groupIdToEdit] = id;

            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGroup(VmGroup vmGroup)
        {

            Group group = await _dal.GetEntity<Group>((Guid)vmGroup.Id) as Group;

            group.NameEn = vmGroup.NameEn;
            group.NameFr = vmGroup.NameFr;
            await _dal.UpdateEntity(group);

            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmployee(VmGroup vmGroup)
        {

            if (vmGroup.InternalId != null)
            {
                // Check to see if User has been added to the group
                List<GroupOwner> groupOwners = await _dal.GetGroupOwnersByGroupIdAndUserId((Guid)vmGroup.Id, (Guid)vmGroup.InternalId);
                if (!groupOwners.Any())
                {
                    GroupOwner groupOwner = new GroupOwner()
                    {
                        GroupId = (Guid)vmGroup.Id,
                        UserId = (Guid)vmGroup.InternalId,
                        HasAccessEE = vmGroup.HasAccessEE
                    };
                    await _dal.AddEntity<GroupOwner>(groupOwner);
                }
            }

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> RemoveEmployee(Guid groupOwnerId)
        {

            await _dal.DeleteEntity<GroupOwner>(groupOwnerId);

            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddContest(VmGroup vmGroup)
        {

            if (vmGroup.ContestIdToAdd != null)
            {
                // Check to see if Contest has been added to the group
                List<ContestGroup> contestGroups = await _dal.GetContestGroupByGroupIdAndContestId((Guid)vmGroup.Id, (Guid)vmGroup.ContestIdToAdd);
                if (!contestGroups.Any())
                {
                    ContestGroup contestGroup = new ContestGroup()
                    {
                        ContestId = (Guid)vmGroup.ContestIdToAdd,
                        GroupId = (Guid)vmGroup.Id
                    };
                    await _dal.AddEntity<ContestGroup>(contestGroup);
                }
            }

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> RemoveContest(Guid contestGroupId)
        {

            await _dal.DeleteEntity<ContestGroup>(contestGroupId);

            return RedirectToAction("Index");

        }

        private async Task PopulateGroupOwnersWithMockUser(List<VmGroup> vmGroups)
        {

            foreach (VmGroup vmGroup in vmGroups)
                foreach (VmGroupOwner vmGroupOwner in vmGroup.GroupOwners)
                    vmGroupOwner.MockUser = await _dal.GetMockUserByIdAndType(vmGroupOwner.UserId, UserTypes.Internal);

        }

        private async Task IndexSetViewBag()
        {

            List<Contest> contests = await _dal.GetAllContests();
            ViewBag.Contests = contests;

            if (TempData[_showAddGroupPartial] != null && (bool)TempData[_showAddGroupPartial])
                ViewBag.VmAddGroup = new VmAddGroup();

        }

        private void IndexRegisterClientResources()
        {

            // css
            WebTemplateModel.HTMLHeaderElements.Add($"<link rel='stylesheet' href='/lib/jquery-ui-1.13.2.custom/jquery-ui.min.css'>");

            // js
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/lib/jquery-ui-1.13.2.custom/jquery-ui.min.js'></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/js/Groups/Index.js?v={BuildId}'></script>");

        }

        #endregion

        #region Add Group

        [HttpGet]
        public IActionResult AddGroup()
        {

            TempData[_showAddGroupPartial] = true;

            return RedirectToAction("Index");

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
                groupId = await _dal.AddEntity<Group>(group);

                ContestGroup contestGroup = new ContestGroup()
                {
                    ContestId = (Guid)_state.ContestId,
                    GroupId = groupId,
                };
                await _dal.AddEntity<ContestGroup>(contestGroup);

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
