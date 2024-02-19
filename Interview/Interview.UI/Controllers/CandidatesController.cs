using GoC.WebTemplate.Components.Core.Services;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Interview.UI.Controllers
{
    
    public class CandidatesController : BaseController
    {

        #region Declarations



        #endregion

        #region Constructors

        public CandidatesController(IModelAccessor modelAccessor, DalSql dal, IStringLocalizer<BaseController> baseLocalizer)
            : base(modelAccessor, dal, baseLocalizer)
        {

        }

        #endregion

        #region Public Schedule Methods

        public IActionResult Schedule(Guid processId)
        {
            return View();
        }

        public IActionResult Schedule(Guid processId, Guid externalCandidateId)
        {
            return View();
        }

        #endregion

    }

}
