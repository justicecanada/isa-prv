using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Graph;
using Interview.UI.Models.Groups;
using Interview.UI.Services.DAL;
using Interview.UI.Services.Graph;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Interview.UI.Controllers
{
    
    public class GroupsController : BaseController
    {

        #region Declarations

        private readonly IToken _tokenManager;
        private readonly IUsers _usersManager;
        private readonly IMapper _mapper;
        private readonly IState _state;
        private readonly IStringLocalizer<GroupsController> _localizer;

        private const string _showAddGroupPartial = "SHOW_ADD_GROUP_PARTIAL";
        private const string _groupIdToEdit = "GROUP_ID_TO_EDIT";

        #endregion

        #region Constructors

        public GroupsController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IState state, IToken tokenManager, 
            IUsers userManager, IStringLocalizer<BaseController> baseLocalizer, IStringLocalizer<GroupsController> localizer) 
            : base(modelAccessor, dal, baseLocalizer)
        {
            _mapper = mapper;
            _state = state;
            _tokenManager = tokenManager;
            _usersManager = userManager;
            _localizer = localizer;
        }

        #endregion

        #region Public Index Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            VmIndex vmIndex = new VmIndex();
            List<Group> groups = null;

            // Hanld groups
            if (User.IsInRole(RoleTypes.Admin.ToString()) || User.IsInRole(RoleTypes.System.ToString()))
                groups = await _dal.GetGroups(null);
            else
                groups = await _dal.GetGroups(EntraId);
            vmIndex.Groups = _mapper.Map<List<VmGroup>>(groups);
            await PopulateGroupOwnersWithGraphUser(vmIndex.Groups);

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

        private async Task PopulateGroupOwnersWithGraphUser(List<VmGroup> vmGroups)
        {

            TokenResponse tokenResponse = await _tokenManager.GetToken();

            foreach (VmGroup vmGroup in vmGroups)
                foreach (VmGroupOwner vmGroupOwner in vmGroup.GroupOwners)
                    vmGroupOwner.GraphUser = await _usersManager.GetUserInfoAsync(vmGroupOwner.UserId.ToString(), tokenResponse.access_token);

        }

        private async Task IndexSetViewBag()
        {

            List<Process> processes = await _dal.GetAllProcesses();
            ViewBag.Processes = processes;

            if (TempData[_showAddGroupPartial] != null && (bool)TempData[_showAddGroupPartial])
                ViewBag.VmAddGroup = new VmAddGroup();

        }

        private void IndexRegisterClientResources()
        {

            // css
            WebTemplateModel.HTMLHeaderElements.Add($"<link rel='stylesheet' href='/lib/jquery-ui-1.13.2.custom/jquery-ui.min.css'>");
            WebTemplateModel.HTMLHeaderElements.Add("<link rel=\"stylesheet\" href=\"/lib/Magnific-Popup-master/Magnific-Popup-master/dist/magnific-popup.css\" />");

            // js
            WebTemplateModel.HTMLBodyElements.Add("<script src=\"/lib/Magnific-Popup-master/Magnific-Popup-master/dist/jquery.magnific-popup.min.js\"></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/lib/jquery-ui-1.13.2.custom/jquery-ui.min.js'></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/js/Groups/Index.js?v={BuildId}'></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/js/Groups/ConfirmDeletesModal.js?v={BuildId}'></script>");

        }

        #endregion

        #region Public Group Methods

        [HttpGet]
        public PartialViewResult DeleteGroupModal(Guid id)
        {

            return ConfirmDeleteModal(id, _localizer["DeleteGroupConfirmationString"].Value);

        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGroupModal(Guid id, bool hardDelete = false)
        {

            Group group = await _dal.GetEntity<Group>(id) as Group;

            group.IsDeleted = true;
            await _dal.UpdateEntity(group);

            return new JsonResult(new { result = true, id = id })
            {
                StatusCode = 200
            };

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

        #endregion

        #region Public Employee Methods

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
        public PartialViewResult RemoveEmployeeModal(Guid id)
        {

            return ConfirmDeleteModal(id, _localizer["RemoveEmployeeConfirmationString"].Value);

        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveEmployeeModal(Guid id, bool hardDelete = false)
        {

            await _dal.DeleteEntity<GroupOwner>(id);

            return new JsonResult(new { result = true, id = id })
            {
                StatusCode = 200
            };

        }

        #endregion

        #region Public Process Methods

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProcess(VmGroup vmGroup)
        {

            if (vmGroup.ProcessIdToAdd != null)
            {
                // Check to see if Process has been added to the group
                List<ProcessGroup> processGroups = await _dal.GetProcessGroupByGroupIdAndProcessId((Guid)vmGroup.Id, (Guid)vmGroup.ProcessIdToAdd);
                if (!processGroups.Any())
                {
                    ProcessGroup processGroup = new ProcessGroup()
                    {
                        ProcessId = (Guid)vmGroup.ProcessIdToAdd,
                        GroupId = (Guid)vmGroup.Id
                    };
                    await _dal.AddEntity<ProcessGroup>(processGroup);
                }
            }

            return RedirectToAction("Index");

        }

        [HttpGet]
        public PartialViewResult RemoveProcessModal(Guid id)
        {

            return ConfirmDeleteModal(id, _localizer["RemoveProcessConfirmationString"].Value);

        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveProcessModal(Guid id, bool hardDelete = false)
        {

            await _dal.DeleteEntity<ProcessGroup>(id);

            return new JsonResult(new { result = true, id = id })
            {
                StatusCode = 200
            };

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
                    NameEn = vmAddGroup.NameEn,
                    NameFr = vmAddGroup.NameFr,
                    ProcessId = (Guid)_state.ProcessId
                };
                group.GroupOwners.Add(new GroupOwner()
                {
                    UserId = EntraId
                });
                groupId = await _dal.AddEntity<Group>(group);

                AddRole();

                return RedirectToAction("Index");

            }
            else
            {
                ViewBag.ShowAddGroupPartial = true;

                return View("Index", vmAddGroup);
            }

        }

        private void AddRole()
        {

            // Not sure what this does. Look at:
            // Entrevue.Groups.AddRole()

        }

        #endregion

    }

}
