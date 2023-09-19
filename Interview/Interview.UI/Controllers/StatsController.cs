using AutoMapper;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;

namespace Interview.UI.Controllers
{
    
    public class StatsController : Controller
    {

        #region Declarations

        private readonly DalSql _dal;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public StatsController(DalSql dal, IMapper mapper)
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

    }

}
