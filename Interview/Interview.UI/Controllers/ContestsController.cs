using AutoMapper;
using Interview.Entities;
using Interview.UI.Data;
using Interview.UI.Models;
using Interview.UI.Services.Automapper;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;

namespace Interview.UI.Controllers
{
    public class ContestsController : Controller
    {

        #region Declarations

        private readonly DalSql _dal;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public ContestsController(DalSql dal, IMapper mapper)
        {
            _dal = dal;
            _mapper = mapper;
        }

        #endregion

        #region Index Methods

        public async Task<IActionResult> Index()
        {

            var contests = await _dal.GetAllContests();
            var vmContests = _mapper.Map(contests, typeof(List<Contest>), typeof(List<VmContest>));

            ViewBag.VmContests = vmContests;

            return View();

        }

        #endregion

        #region Details Methods



        #endregion

    }
}
