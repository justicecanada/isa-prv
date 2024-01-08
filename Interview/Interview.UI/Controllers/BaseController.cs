using GoC.WebTemplate.Components.Core.Services;
using GoC.WebTemplate.CoreMVC.Controllers;
using Interview.Entities;
using Interview.UI.Models.AppSettings;
using Interview.UI.Services.DAL;
using Interview.UI.Services.Mock.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Interview.UI.Controllers
{

    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class BaseController : WebTemplateBaseController
    {

        #region Declarations

        private string _assemblyVersion;
        private string _buildId;
        private readonly IOptions<JusticeOptions> _justiceOptions;
        protected readonly DalSql _dal;
        private MockUser _loggedInMockUser;

        #endregion

        #region Constructors

        public BaseController(IModelAccessor modelAccessor, IOptions<JusticeOptions> justiceOptions, DalSql dal) : base(modelAccessor)
        {

            _justiceOptions = justiceOptions;
            _dal = dal;
            
            //https://github.com/wet-boew/cdts-DotNetTemplates/blob/master/samples/dotnet-coremvc-sample/Controllers/GoCWebTemplateSamplesController.cs

            // css
            WebTemplateModel.HTMLHeaderElements.Add($"<link rel=\"stylesheet\" href=\"/css/site.css?v={BuildId}\" />");
            //WebTemplateModel.HTMLHeaderElements.Add("<link href=\"/lib/jquery-ui-1.13.2.custom/jquery-ui.min.css\" rel=\"stylesheet\" />");
            WebTemplateModel.HTMLHeaderElements.Add("<script src=\"/lib/jquery/dist/jquery.min.js\"></script>");

            // js
            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/site.js?v={BuildId} defer \"></script>");
            //WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/lib/jquery-ui-1.13.2.custom/jquery-ui.min.js\"></script>");

            // Identifier
            WebTemplateModel.VersionIdentifier = AssemblyVersion;

        }

        #endregion

        #region Properties

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

        #endregion

        #region Protected MockUser Properties and Methods

        protected MockUser LoggedInMockUser
        {
            get
            {
                if (_loggedInMockUser == null)
                    _loggedInMockUser = _dal.GetMockUserByName(_justiceOptions.Value.MockLoggedInUserName).GetAwaiter().GetResult();

                return _loggedInMockUser;
            }
        }

        protected bool IsLoggedInMockUserInRole(MockLoggedInUserRoles roleType)
        {
            return _justiceOptions.Value.MockLoggedInUserRole == roleType;
        }

        protected async Task<List<Process>> GetContestsForLoggedInUser()
        {

            List<Process> result = null;

            if (IsLoggedInMockUserInRole(MockLoggedInUserRoles.Admin) || IsLoggedInMockUserInRole(MockLoggedInUserRoles.System))
                result = await _dal.GetAllProcesses();
            else if (IsLoggedInMockUserInRole(MockLoggedInUserRoles.Owner))
                result = await _dal.GetProcessesForGroupOwner((Guid)LoggedInMockUser.Id);
            else
                result = await _dal.GetProcessesForRoleUser((Guid)LoggedInMockUser.Id);
            result.OrderByDescending(x => x.CreatedDate);

            return result;

        }

        #endregion

        #region Public Action Methods

        [HttpGet]
        public async Task<JsonResult> LookupInteralUser(string query)
        {

            List<MockUser> result = null;

            if (!string.IsNullOrEmpty(query))
                result = await _dal.LookupInteralMockUser(query);

            return new JsonResult(new { result = true, results = result })
            {
                StatusCode = 200
            };

        }

        #endregion

    }

}
