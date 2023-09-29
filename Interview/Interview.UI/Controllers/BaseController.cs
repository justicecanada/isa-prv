using GoC.WebTemplate.Components.Core.Services;
using GoC.WebTemplate.CoreMVC.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Interview.UI.Controllers
{

    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class BaseController : WebTemplateBaseController
    {

        #region Declarations

        private string _assemblyVersion;

        #endregion

        #region Constructors

        public BaseController(IModelAccessor modelAccessor) : base(modelAccessor)
        {

            //https://github.com/wet-boew/cdts-DotNetTemplates/blob/master/samples/dotnet-coremvc-sample/Controllers/GoCWebTemplateSamplesController.cs

            // css
            WebTemplateModel.HTMLHeaderElements.Add($"<link rel=\"stylesheet\" href=\"/css/site.css?v={AssemblyVersion}\" />");
            //WebTemplateModel.HTMLHeaderElements.Add("<link href=\"/lib/jquery-ui-1.13.2.custom/jquery-ui.min.css\" rel=\"stylesheet\" />");
            WebTemplateModel.HTMLHeaderElements.Add("<script src=\"/lib/jquery/dist/jquery.min.js\"></script>");

            // js
            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/site.js?v={AssemblyVersion} defer \"></script>");
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

        #endregion

    }

}
