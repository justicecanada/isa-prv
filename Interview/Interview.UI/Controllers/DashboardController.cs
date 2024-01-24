using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.UI.Models.AppSettings;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Interview.UI.Controllers
{
    
    public class DashboardController : BaseController
    {

        #region Declarations

        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public DashboardController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IOptions<JusticeOptions> justiceOptions
            , IStringLocalizer<BaseController> baseLocalizer) 
            : base(modelAccessor, justiceOptions, dal, baseLocalizer)
        {
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
