using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;

namespace Interview.UI.Controllers
{
    
    public class DashboardController : BaseController
    {

        #region Declarations

        private readonly DalSql _dal;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public DashboardController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper) : base(modelAccessor)
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
