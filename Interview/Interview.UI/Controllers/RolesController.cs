using AutoMapper;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;

namespace Interview.UI.Controllers
{
    
    public class RolesController : GoC.WebTemplate.CoreMVC.Controllers.WebTemplateBaseController
    {

        #region Declarations

        private readonly DalSql _dal;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public RolesController(DalSql dal, IMapper mapper, GoC.WebTemplate.Components.Core.Services.IModelAccessor modelAccessor)
            : base(modelAccessor)
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
