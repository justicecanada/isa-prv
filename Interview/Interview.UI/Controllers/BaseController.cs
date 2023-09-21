using GoC.WebTemplate.Components.Core.Services;
using GoC.WebTemplate.CoreMVC.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Interview.UI.Controllers
{
    
    public class BaseController : WebTemplateBaseController
    {
        
        public BaseController(IModelAccessor modelAccessor) : base(modelAccessor)
        {

        }

    }

}
