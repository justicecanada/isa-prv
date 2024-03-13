using GoC.WebTemplate.Components.Core.Services;
using GoC.WebTemplate.Components.Entities;
using GoC.WebTemplate.CoreMVC.Controllers;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Graph;
using Interview.UI.Models.Roles;
using Interview.UI.Models.Shared;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Reflection;

namespace Interview.UI.Controllers
{

    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class BaseController : WebTemplateBaseController
    {

        #region Declarations

        private string _assemblyVersion;
        private string _buildId;
        private readonly IStringLocalizer<BaseController> _localizer;
        protected readonly DalSql _dal;
        IOptions<SessionTimeoutOptions> _sessionTimeoutOptions;

        #endregion

        #region Constructors

        public BaseController(IModelAccessor modelAccessor, DalSql dal, IStringLocalizer<BaseController> localizer, IOptions<SessionTimeoutOptions> sessionTimeoutOptions) : base(modelAccessor)
        {

            _dal = dal;
            _localizer = localizer;

            // css
            WebTemplateModel.HTMLHeaderElements.Add($"<link rel=\"stylesheet\" href=\"/css/site.css?v={BuildId}\" />");
            WebTemplateModel.HTMLHeaderElements.Add("<script src=\"/lib/jquery/dist/jquery.min.js\"></script>");

            // js
            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/site.js?v={BuildId} defer \"></script>");

            // Identifier
            WebTemplateModel.VersionIdentifier = AssemblyVersion;

            // Session Timeout
            _sessionTimeoutOptions = sessionTimeoutOptions;

        }

        #endregion

        #region Protected Properties

        protected string AssemblyVersion
        {
            get
            {

                if (string.IsNullOrEmpty(_assemblyVersion))
                    _assemblyVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

                return _assemblyVersion;

            }
        }

        protected string BuildId
        {
            get
            {

                if (string.IsNullOrEmpty(_buildId))
                    _buildId = Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToString();

                return _buildId;

            }
        }

        protected Guid EntraId
        {
            get
            {
                return new Guid(User.Claims.FirstOrDefault(x => x.Type == Constants.EntraIdClaimKey).Value);
            }
        }

        protected string HostName
        {
            get
            {
                string result = null;

                if (Request.Headers.ContainsKey("X-Forwarded-Host")) ;
                result = Request.Headers["X-Forwarded-Host"];

                return result;
            }
        }

        #endregion

        #region Protected Action Methods

        [HttpGet]
        public PartialViewResult ConfirmDeleteModal(Guid id, string message)
        {

            VmConfirmDeleteModal result = new VmConfirmDeleteModal();

            result.Id = id;
            ViewBag.Message = message;

            return PartialView("ConfirmDeleteModal", result);

        }

        [HttpGet]
        public IActionResult Logout()
        {

            IActionResult result = null;

            // Handle where to redirect
            if (User.Identity.IsAuthenticated)
                result = new RedirectToActionResult("LoggedOut", "Account", null);
            else
                result = new RedirectToActionResult("SessionEnded", "Account", null);

            // Handle Session
            HttpContext.Session.Clear();

            return result;

        }

        [HttpGet]
        public string SessionValidity()
        {
            return "true";
        }

        #endregion

        #region Protected Methods

        protected void HandleCommonPageMethods(bool addTopMenuItems = true)
        {

            // This cannot be handled in the BaseController constructor because User is null at that time.
            //https://github.com/wet-boew/cdts-DotNetTemplates/blob/master/samples/dotnet-coremvc-sample/Controllers/GoCWebTemplateSamplesController.cs

            // Top menu
            if (addTopMenuItems)
            {
                WebTemplateModel.MenuLinks = new List<MenuLink>();
                WebTemplateModel.MenuLinks.Add(new MenuLink() { Text = _localizer["Home"].Value, Href = "/Default/Index" });
                WebTemplateModel.MenuLinks.Add(new MenuLink() { Text = _localizer["ProcessList"].Value, Href = "/Processes/Index" });
                WebTemplateModel.MenuLinks.Add(new MenuLink() { Text = _localizer["GroupList"].Value, Href = "/Groups/Index" });
                WebTemplateModel.MenuLinks.Add(new MenuLink() { Text = _localizer["Dashboard"].Value, Href = "/Dashboard/Index" });
                if (User.IsInRole(Roles.Admin))
                {
                    WebTemplateModel.MenuLinks.Add(new MenuLink() { Text = _localizer["UserRoles"].Value, Href = "/Account/ManageUserRoles" });
                }
            }

            // Session Timeout
            WebTemplateModel.Settings.SessionTimeout.Enabled = _sessionTimeoutOptions.Value.Enabled;
            WebTemplateModel.Settings.SessionTimeout.Inactivity = _sessionTimeoutOptions.Value.InactivityInMilliseconds;
            WebTemplateModel.Settings.SessionTimeout.ReactionTime = _sessionTimeoutOptions.Value.ReactionTimeInMilliseconds;
            WebTemplateModel.Settings.SessionTimeout.SessionAlive = _sessionTimeoutOptions.Value.SessionAliveInMilliseconds;
            WebTemplateModel.Settings.SessionTimeout.LogoutUrl = "Logout";
            WebTemplateModel.Settings.SessionTimeout.RefreshCallBackUrl = "SessionValidity";
            WebTemplateModel.Settings.SessionTimeout.RefreshOnClick = _sessionTimeoutOptions.Value.RefreshOnClick;
            WebTemplateModel.Settings.SessionTimeout.RefreshLimit = _sessionTimeoutOptions.Value.RefreshLimitInMilliseconds;
            WebTemplateModel.Settings.SessionTimeout.Method = _sessionTimeoutOptions.Value.Method;
            WebTemplateModel.Settings.SessionTimeout.AdditionalData = "";

        }

        protected async Task<List<Process>> GetProcessesForLoggedInUser()
        {

            List<Process> result = null;

            if (User.IsInRole(RoleTypes.Admin.ToString()) || User.IsInRole(RoleTypes.System.ToString()))
                result = await _dal.GetAllProcesses();
            else if (User.IsInRole(RoleTypes.Owner.ToString()))
                result = await _dal.GetProcessesForGroupOwner(EntraId);
            else
                result = await _dal.GetProcessesForRoleUser(EntraId);
            result.OrderByDescending(x => x.CreatedDate);

            return result;

        }

        protected void Notify(string message, string alertClass)
        {

            VmNotificationPartial vmNotificationPartial = new VmNotificationPartial()
            {
                Message = message,
                AlertClass = alertClass,
            };

            TempData[Constants.NotificationPartialModel] = JsonConvert.SerializeObject(vmNotificationPartial);

        }

        #endregion

    }

}
