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

        public AccountController(IModelAccessor modelAccessor, IOptions<JusticeOptions> justiceOptions, IOptions<SessionTimeout> sessionTimeoutOptions, DalSql dal) 
            : base(modelAccessor, justiceOptions, sessionTimeoutOptions, dal)
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

        #region SignOut

        [HttpGet]
        public RedirectToActionResult SignOut()
        {

            RedirectToActionResult result = null;

            // Handle where to redirect
            if (User.Identity.IsAuthenticated)
                result = new RedirectToActionResult("SignIn", "Account", null);
            else
                result = new RedirectToActionResult("SignedOut", "Account", null);

            // Handle Session
            HttpContext.Session.Clear();

            return result;

        }

        [HttpGet]
        public IActionResult SignedOut()
        {

            return View();

        }

        #endregion

    }

}
