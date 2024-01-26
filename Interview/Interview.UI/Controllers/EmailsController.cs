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
using Microsoft.Extensions.Localization;
using Interview.UI.Models.Graph;
using Newtonsoft.Json;
using System.Net.Mail;
using Interview.UI.Services.Graph;

namespace Interview.UI.Controllers
{
    public class EmailsController : BaseController
    {

        #region Declarations

        private readonly IMapper _mapper;
        private readonly IState _state;
        private readonly IToken _tokenManager;
        private readonly IEmails _emailsManager;

        #endregion

        #region Constructors

        public EmailsController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IOptions<JusticeOptions> justiceOptions, IState state, 
            IStringLocalizer<BaseController> baseLocalizer, IToken tokenManager, IEmails emailsManager) 
            : base(modelAccessor, justiceOptions, dal, baseLocalizer)
        {
            _mapper = mapper;
            _state = state;
            _tokenManager = tokenManager;
            _emailsManager = emailsManager;
        }

        #endregion

        #region Public Index Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            VmIndex result = new VmIndex();

            if (_state.ProcessId != null)
            {
                
                var emailTemplates = await _dal.GetEmailTemplatesByProcessId((Guid)_state.ProcessId);
                result.EmailTemplates = _mapper.Map<List<VmEmailTemplate>>(emailTemplates);

                foreach (EmailTypes emailType in Enum.GetValues(typeof(EmailTypes)))
                    if (!result.EmailTemplates.Any(x => x.EmailType == emailType))
                        result.EmailTemplates.Add(new VmEmailTemplate() { EmailType = emailType });

            }

            RegisterIndexClientResources();

            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(VmIndex vmIndex)
        {

            Guid processId = (Guid)_state.ProcessId;
            List<EmailTemplate> emailTemplates = _mapper.Map<List<EmailTemplate>>(vmIndex.EmailTemplates);

            foreach (EmailTemplate emailTemplate in emailTemplates)
            {
                emailTemplate.ProcessId = processId;
                if (emailTemplate.Id == Guid.Empty)
                    await _dal.AddEntity<EmailTemplate>(emailTemplate);
                else
                    await _dal.UpdateEntity(emailTemplate);
            }

            return RedirectToAction("Index");

        }

        private void RegisterIndexClientResources()
        {

            WebTemplateModel.HTMLBodyElements.Add("<script src=\"/assets/vendor/ckeditor5/build/ckeditor.js\"></script>");

            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/JusRichTextBoxFor.js?v={BuildId}\"></script>");

        }

        #endregion

        #region Public Send Email Methods

        [HttpGet]
        public IActionResult SendEmail()
        {

            VmSendEmail result = new VmSendEmail();

            return View(result); 

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmail(VmSendEmail vmSendEmail)
        {

            if (ModelState.IsValid)
            {

                EmailEnvelope emailEnvelope = new EmailEnvelope()
                {
                    message = new EmailMessage()
                    {
                        subject = vmSendEmail.Subject,
                        body = new EmailBody()
                        {
                            contentType = "Text",
                            content = vmSendEmail.Body
                        },
                        toRecipients = GetEmailRecipients(vmSendEmail.ToRecipients),
                        ccRecipients = GetEmailRecipients(vmSendEmail.CcRecipients),                       
                    },
                    saveToSentItems = vmSendEmail.SaveToSentItems.ToString().ToLower()
                };
                TokenResponse tokenResponse = await _tokenManager.GetToken();
                HttpResponseMessage responseMessage = await _emailsManager.SendEmailAsync(emailEnvelope, tokenResponse.access_token);                

                TempData["Token"] = JsonConvert.SerializeObject(tokenResponse, Formatting.Indented);
                TempData["EmailEnvelope"] = JsonConvert.SerializeObject(emailEnvelope, Formatting.Indented);
                TempData["ResponseMessage"] = JsonConvert.SerializeObject(responseMessage, Formatting.Indented);

                return RedirectToAction("EmailSent");

            }
            else
            {
                return View(vmSendEmail);
            }

        }

        [HttpGet]
        public IActionResult EmailSent()
        {

            return View();

        }

        private List<EmailRecipent> GetEmailRecipients(string recipients)
        {

            List<EmailRecipent> result = new List<EmailRecipent>();
            string[] addresses = string.IsNullOrEmpty(recipients) ? new string[0] : recipients.Split(',');

            foreach (string address in addresses)
            {
                EmailRecipent recipent = new EmailRecipent();
                recipent.emailAddress.address = address;
                result.Add(recipent);
            }

            return result;

        }

        #endregion

    }
}
