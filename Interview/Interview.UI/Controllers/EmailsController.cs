using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Groups;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Server;
using System.Diagnostics;

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

        //id is incorrect...

        [HttpGet]
        public async Task<IActionResult> Index(Guid? contestId)
        {
            Models.VmContest vmContest = null;
            if (contestId == null)
            {
                vmContest = new Models.VmContest();
            }
            else
            {
                var contest = await _dal.GetEntity<Contest>((Guid)contestId, true) as Contest;
                vmContest = _mapper.Map<Models.VmContest>(contest);
                EmailTemplate? dbEmails = await _dal.GetEntity<EmailTemplate>((Guid)contestId) as EmailTemplate;
                Debug.WriteLine(dbEmails.Id);
            }

            return View();
        }

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmailSave (VmEmails vmEmails)
        {
            Debug.WriteLine("Debug Isafasfasfg ");
           
            //Debug.WriteLine(dbEmails.Id);
            if (ModelState.IsValid)
			{
				var emails = _mapper.Map<EmailTemplate>(vmEmails);
				EmailTemplate? dbEmails = await _dal.GetEntity<EmailTemplate>((Guid)vmEmails.Id) as EmailTemplate;

				//emails.Id = dbEmails.Id;
                Debug.WriteLine(emails.Id);
				//emails.EmailType = dbEmails.EmailType;
                Debug.WriteLine(emails.EmailType);
                //emails.EmailSubject = dbEmails.EmailSubject;
                Debug.WriteLine(emails.EmailSubject);
                //emails.EmailCC = dbEmails.EmailCC;
                Debug.WriteLine(emails.EmailCC);
                await _dal.AddEntity<EmailTemplate>(emails);

                return RedirectToAction("Index", "Default");

			}
			else
			{
                Debug.WriteLine("Debug Isafasfasfg ");
                return View("Emails", vmEmails);
			}
        }
        */


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Emails(VmEmails vmEmails)
        {
            //var name = vmEmails.Name;
            Debug.Write("TESTESTTEST");

            return View();


        }
        #endregion
    }
}
