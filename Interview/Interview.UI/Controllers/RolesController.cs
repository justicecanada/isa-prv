using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;

namespace Interview.UI.Controllers
{
    
    public class RolesController : BaseController
    {

        #region Declarations

        private readonly DalSql _dal;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public RolesController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper) : base(modelAccessor)
        {
            _dal = dal;
            _mapper = mapper;
        }

        #endregion

        #region Index Methods

        public async Task<IActionResult> Index(Guid contestId)
        {

            var contest = await _dal.GetEntity<Contest>(contestId, true);
            var vmContest = _mapper.Map<VmContest>(contest);
            
            return View();

        }

        #endregion

    }

}
