using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Groups;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Interview.UI.Controllers
{
    public class EmailsController : BaseController
    {

        #region Declarations

        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public EmailsController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IOptions<JusticeOptions> justiceOptions) 
            : base(modelAccessor, justiceOptions, dal)
        {
            _mapper = mapper;
        }

        #endregion

        #region Public Index Methods

        //Question about Language Option**
        //Question about Validation**

		

        [HttpGet]
        public async Task<IActionResult> Index(Guid id)
        {

            var emails = await _dal.GetEntity<EmailTemplate>(id, true) as EmailTemplate;
            var vmEmails = _mapper.Map<VmEmails>(emails);

            return View(vmEmails);

        }



		/* Need more research for this part */

		public async Task<IActionResult> Emails(Guid? emailsId)
		{

			VmEmails vmEmails = null;

			if (emailsId == null)
			{
				vmEmails = new VmEmails();
			}
			else
			{
				var emails = await _dal.GetEntity<EmailTemplate>((Guid)emailsId, true) as EmailTemplate;
				vmEmails = _mapper.Map<VmEmails>(emails);
			}

			return View(vmEmails);

		}

		/* Need more research for this part */

		public async Task<IActionResult> EmailsSave(VmEmails vmEmails)
		{

			if (ModelState.IsValid)
			{
				var emails = _mapper.Map<EmailTemplate>(vmEmails);
				EmailTemplate dbEmailTemplates = await _dal.GetEntity<EmailTemplate>((Guid)vmEmails.Id) as EmailTemplate;

				await _dal.UpdateEntity(emails);

				return RedirectToAction("Index", "Default");

			}
			else
			{

				return View("Emails", vmEmails);
			}

		}

		#endregion

	}
}
