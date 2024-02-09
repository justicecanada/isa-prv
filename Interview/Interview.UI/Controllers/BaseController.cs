﻿using GoC.WebTemplate.Components.Core.Services;
using GoC.WebTemplate.Components.Entities;
using GoC.WebTemplate.CoreMVC.Controllers;
using Interview.Entities;
using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Graph;
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

        #endregion

        #region Constructors

        public BaseController(IModelAccessor modelAccessor, DalSql dal, IStringLocalizer<BaseController> localizer) : base(modelAccessor)
        {

            _dal = dal;
            _localizer = localizer;

            //https://github.com/wet-boew/cdts-DotNetTemplates/blob/master/samples/dotnet-coremvc-sample/Controllers/GoCWebTemplateSamplesController.cs

            // Top menu
            WebTemplateModel.MenuLinks = new List<MenuLink>();
            WebTemplateModel.MenuLinks.Add(new MenuLink() { Text = _localizer["Home"].Value, Href = "/Default/Index" });
            WebTemplateModel.MenuLinks.Add(new MenuLink() { Text = _localizer["ProcessList"].Value, Href = "/Processes/Index" });
            WebTemplateModel.MenuLinks.Add(new MenuLink() { Text = _localizer["GroupList"].Value, Href = "/Groups/Index" });
            WebTemplateModel.MenuLinks.Add(new MenuLink() { Text = _localizer["Dashboard"].Value, Href = "/Dashboard/Index" });
            WebTemplateModel.MenuLinks.Add(new MenuLink() { Text = _localizer["Account"].Value, Href = "/Account/Details" });
            WebTemplateModel.MenuLinks.Add(new MenuLink() { Text = "Send Emails", Href = "/Emails/SendEmail" });

            // css
            WebTemplateModel.HTMLHeaderElements.Add($"<link rel=\"stylesheet\" href=\"/css/site.css?v={BuildId}\" />");
            WebTemplateModel.HTMLHeaderElements.Add("<script src=\"/lib/jquery/dist/jquery.min.js\"></script>");

            // js
            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/site.js?v={BuildId} defer \"></script>");

            // Identifier
            WebTemplateModel.VersionIdentifier = AssemblyVersion;

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

        #endregion

        #region Protected Methods

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
