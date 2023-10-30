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
using System;
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
                Debug.WriteLine(vmContest.Id);
                //Requires additional changes here.
                //EmailTemplate? dbEmails = await _dal.GetEntity<EmailTemplate>((Guid)contestId) as EmailTemplate;




            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Save(VmEmails vmEmails)
        {
            /*Fetch ContestId before saving, likely requires changes to Index method
            Append ContestId to EmailTemplatesId
            Add EmailSubject / EmailCC / EmailBody / EmailType (to be configured) when saved
            Configure Language options
            */
            return View("Index");
        }

        #endregion
    }
}
