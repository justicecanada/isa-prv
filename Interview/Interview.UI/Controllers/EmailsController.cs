using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.Emails;
using Interview.UI.Models.AppSettings;
using Interview.UI.Services.DAL;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Interview.UI.Controllers
{
    public class EmailsController : BaseController
    {

        #region Declarations

        private readonly IMapper _mapper;
        private readonly IState _state;

        #endregion

        #region Constructors

        public EmailsController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IOptions<JusticeOptions> justiceOptions, IState state) 
            : base(modelAccessor, justiceOptions, dal)
        {
            _mapper = mapper;
            _state = state;
        }

        #endregion

        #region Public Index Methods

        [HttpGet]
        public async Task<IActionResult> Index(Guid id)
        {

            VmIndex result = new VmIndex();

            if (_state.ContestId != null)
            {
                
                var emailTemplates = await _dal.GetEmailTemplatesByContestId((Guid)_state.ContestId);
                result.EmailTemplates = _mapper.Map<List<VmEmailTemplate>>(emailTemplates);

                foreach (EmailTypes emailType in Enum.GetValues(typeof(EmailTypes)))
                    if (!result.EmailTemplates.Any(x => x.EmailType == emailType))
                        result.EmailTemplates.Add(new VmEmailTemplate() { EmailType = emailType });

            }

            RegisterIndexClientResources();

            return View(result);

        }

        [HttpPost]
        public async Task<IActionResult> Save(VmIndex vmIndex)
        {

            return View("Index");

        }

        private void RegisterIndexClientResources()
        {

            WebTemplateModel.HTMLBodyElements.Add("<script src=\"/assets/vendor/ckeditor5/build/ckeditor.js\"></script>");

            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/JusRichTextBoxFor.js?v={BuildId}\"></script>");

        }

        #endregion

    }
}
