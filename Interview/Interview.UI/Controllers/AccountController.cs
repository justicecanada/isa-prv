using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.UI.Models.AppSettings;
using Interview.UI.Services.DAL;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Interview.UI.Controllers
{
    
    public class AccountController : BaseController
    {

        #region Declarations



        #endregion

        #region Constructors

        public AccountController(IModelAccessor modelAccessor, IOptions<JusticeOptions> justiceOptions, DalSql dal) 
            : base(modelAccessor, justiceOptions, dal)
        {

        }

        #endregion

        #region SignIn

        [HttpGet]
        public async Task<IActionResult> SignIn()
        {

            return View();

        }

        #endregion

    }

}
