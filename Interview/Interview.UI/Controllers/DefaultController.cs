using AutoMapper;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;

namespace Interview.UI.Controllers
{
    public class DefaultController : Controller
    {

        #region Declarations

        private readonly DalSql _dal;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public DefaultController(DalSql dal, IMapper mapper)
        {
            _dal = dal;
            _mapper = mapper;
        }

        #endregion

        #region Public Index Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var contests = await _dal.GetAllContestsWithUserSettingsAndRoles();
            var vmContests = contests == null ? new List<VmContest>() : _mapper.Map(contests, typeof(List<Contest>), typeof(List<VmContest>));

            ViewBag.VmContests = vmContests;

            return View();

        }

        #endregion

    }
}
