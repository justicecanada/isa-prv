using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.UI.Models.Groups;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;

namespace Interview.UI.Controllers
{
    
    public class GroupsController : BaseController
    {

        #region Declarations

        private readonly DalSql _dal;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public GroupsController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper) : base(modelAccessor)
        {
            _dal = dal;
            _mapper = mapper;
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
        public IActionResult AddGroup(VmAddGroup vmAddGroup)
        {

            if (ModelState.IsValid)
            {

                return RedirectToAction("Index");

            }
            else
            {
                ViewBag.ShowAddGroupPartial = true;

                return View("Index", vmAddGroup);
            }

        }

        #endregion

    }

}
