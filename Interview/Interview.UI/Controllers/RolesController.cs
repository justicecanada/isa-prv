using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using GoC.WebTemplate.Components.Entities;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Graph;
using Interview.UI.Models.Roles;
using Interview.UI.Models.Shared;
using Interview.UI.Services.DAL;
using Interview.UI.Services.Graph;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;

namespace Interview.UI.Controllers
{
    
    public class RolesController : BaseController
    {

        #region Declarations

        private readonly IMapper _mapper;
        private readonly IState _state;
        private readonly IStringLocalizer<RolesController> _localizer;
        private readonly IToken _tokenManager;
        private readonly IUsers _usersManager;
        private readonly IEmails _emailsManager;

        #endregion

        #region Constructors

        public RolesController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IState state, IStringLocalizer<RolesController> localizer, 
            IStringLocalizer<BaseController> baseLocalizer, IToken tokenManager, IUsers graphManager, IEmails emailsManager) 
            : base(modelAccessor, dal, baseLocalizer)
        {
            
            _mapper = mapper;
            _state = state;
            _localizer = localizer;
            _tokenManager = tokenManager;
            _usersManager = graphManager;
            _emailsManager = emailsManager;

        }

        #endregion

        #region Index Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            VmIndex result = new VmIndex();

            // // Handle equities (this will be handled by the role the logged in user is in)
            if (User.IsInRole(RoleTypes.Admin.ToString()))
            {
                var equities = await _dal.GetAllEquities();
                List<VmEquity> vmEquities = (List<VmEquity>)_mapper.Map(equities, typeof(List<Equity>), typeof(List<VmEquity>));
                result.Equities = vmEquities;
            }

            await IndexSetViewBag();
            IndexRegisterClientResources();

            if (TempData[Constants.RoleUserIdToUpdate] != null)
            {
                RoleUser roleUserToEdit = ((List<RoleUser>)ViewBag.RoleUsers).Where(x => x.Id == (Guid)TempData[Constants.RoleUserIdToUpdate]).First();
                result.RoleUserToEdit = (VmRoleUser)_mapper.Map(roleUserToEdit, typeof(RoleUser), typeof(VmRoleUser));
            }

            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(VmIndex vmIndex)
        {

            if (ModelState.IsValid) // Handles simple validation (just posted model)
                await IndexHandleModelState(vmIndex); // Handles Complex validation (checks db)

            if (ModelState.IsValid)
            {

                // Handle RolesUser
                Guid roleUserId;
                RoleUser roleUser = await GetRoleUser(vmIndex);
                roleUserId = await _dal.AddEntity<RoleUser>(roleUser);

                // Handle equities (this will be handled by the role the logged in user is in)
                if (User.IsInRole(RoleTypes.Admin.ToString()))
                {                   
                    foreach (var equity in vmIndex.Equities.Where(x => x.IsSelected).ToList())
                    {
                        var roleUserEquity = new RoleUserEquity()
                        {
                            RoleUserId = roleUserId,
                            EquityId = (Guid)equity.Id
                        };
                        await _dal.AddEntity<RoleUserEquity>(roleUserEquity);
                    }
                }

                return RedirectToAction("Index");

            }
            else
            {

                await IndexSetViewBag();
                IndexRegisterClientResources();

                return View(vmIndex);

            }

        }

        private async Task IndexHandleModelState(VmIndex vmIndex)
        {

            List<RoleUser> roleUsersForProcess = null;
            switch (vmIndex.UserType)
            {
                case UserTypes.Internal:                        // Ensure RoleUser hasn't been added to process
                    roleUsersForProcess = await _dal.GetRoleUsersByProcessId((Guid)_state.ProcessId);
                    if (roleUsersForProcess.Any(x => x.UserId == (Guid)vmIndex.InternalId))
                        ModelState.AddModelError("InternalName", _localizer["UserAlreadyInRole"].Value);
                    break;
                case UserTypes.ExistingExternal:                // Ensure RoleUser hasn't been added to process
                    roleUsersForProcess = await _dal.GetRoleUsersByProcessId((Guid)_state.ProcessId);
                    if (roleUsersForProcess.Any(x => ((bool)x.IsExternal && x.ExternalUserEmail.ToLower() == vmIndex.ExistingExternalEmail.ToLower())))
                        ModelState.AddModelError("ExistingExternalEmail", _localizer["UserAlreadyInRole"].Value);
                    break;
                case UserTypes.NewExternal:                     // Ensure new External User doesn't exist
                    List<ExternalUser> externalUsers = await _dal.GetExternalUsersByEmail(vmIndex.NewExternalEmail);
                    if (externalUsers.Any())
                        ModelState.AddModelError("NewExternalEmail", _localizer["ExternalUserAlreadyExists"].Value);
                    break;
            }

        }

        private async Task IndexSetViewBag()
        {

            // Process
            var process = _state.ProcessId == null ? new Process() : await _dal.GetEntity<Process>((Guid)_state.ProcessId, true);
            ViewBag.Process = process;

            // RoleUsers
            var roleUsers = _state.ProcessId == null ? new List<RoleUser>() : await _dal.GetRoleUsersByProcessId((Guid)_state.ProcessId);
            ViewBag.RoleUsers = roleUsers;

            // Existing Users
            var externalUsers = await _dal.GetExternalUsers();
            ViewBag.ExternalUsers = externalUsers;

            // Show Equities
            ViewBag.ShowEquities = User.IsInRole(RoleTypes.Admin.ToString());

        }

        private void IndexRegisterClientResources()
        {

            // css
            WebTemplateModel.HTMLHeaderElements.Add($"<link rel='stylesheet' href='/lib/jquery-ui-1.13.2.custom/jquery-ui.min.css'>");
            WebTemplateModel.HTMLHeaderElements.Add($"<link rel='stylesheet' href='/lib/jquery-DataTables/datatables.min.css'>");

            // js
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/lib/jquery-ui-1.13.2.custom/jquery-ui.min.js'></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/js/Roles/Index.js?v={BuildId}'></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/lib/jquery-DataTables/datatables.min.js'></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/js/Roles/TablePartial.js?v={BuildId}'></script>");

        }

        private async Task<RoleUser> GetRoleUser(VmIndex vmIndex)
        {

            // finish this...

            RoleUser result = new RoleUser()
            {
                ProcessId = (Guid)_state.ProcessId,
                LanguageType = vmIndex.LanguageType == null ? null : (LanguageTypes)vmIndex.LanguageType,
                RoleUserType = (RoleUserTypes)vmIndex.RoleUserType,
                IsExternal = (UserTypes)vmIndex.UserType != UserTypes.Internal,
                DateInserted = DateTime.Now
            };
            Guid? id = null;
            string userFirstName = null;
            string userLastName = null;

            if (vmIndex.UserType == UserTypes.Internal)
            {
                GraphUser graphUser = await GetGraphUser((Guid)vmIndex.InternalId);
                id = graphUser.id;
                userFirstName = graphUser.givenName;
                userLastName = graphUser.surname;
            }
            else if (vmIndex.UserType == UserTypes.NewExternal)
            {
                ExternalUser externalUser = new ExternalUser()
                {
                    GivenName = vmIndex.NewExternalFirstName,
                    SurName = vmIndex.NewExternalLastName,
                    Email = vmIndex.NewExternalEmail
                };
                id = await _dal.AddEntity<ExternalUser>(externalUser);
                userFirstName = vmIndex.NewExternalFirstName;
                userLastName = vmIndex.NewExternalLastName;
                result.ExternalUserEmail = vmIndex.NewExternalEmail;
            }
            else if (vmIndex.UserType == UserTypes.ExistingExternal)
            {
                List<ExternalUser> externalUsers = await _dal.GetExternalUsersByEmail(vmIndex.ExistingExternalEmail);
                ExternalUser externalUser = externalUsers.First();
                id = externalUser.Id;
                userFirstName = externalUser.GivenName;
                userLastName = externalUser.SurName;
                result.ExternalUserEmail = vmIndex.ExistingExternalEmail;
            }

            result.UserId = (Guid)id;
            result.UserFirstname = userFirstName;
            result.UserLastname = userLastName;

            return result;

        }

        private async Task<GraphUser> GetGraphUser(Guid id)
        {

            GraphUser result = null;
            TokenResponse tokenResponse = await _tokenManager.GetToken();

            result = await _usersManager.GetUserInfoAsync(id.ToString(), tokenResponse.access_token);

            return result;

        }

        #endregion

        #region LegendPartial Methods

        [HttpGet]
        public async Task<IActionResult> EmailExternalExceptAlreadySent()
        {

            var emailTemplates = await _dal.GetEmailTemplatesByProcessId((Guid)_state.ProcessId, EmailTypes.CandidateExternal);
            var emailTemplate = emailTemplates.FirstOrDefault();

            if (emailTemplate == null)
            {
                Notify("There is no email template", "danger");
                return RedirectToAction("Index");
            }

            var roleUsers = await _dal.GetRoleUsersByProcessId((Guid)_state.ProcessId);
            var externalRoleUsers = roleUsers.Where(x => ((bool)x.IsExternal && x.DateExternalEmailSent == null)).ToList();

            await SendExteralEmailsForExternaRolelUsers(emailTemplate, externalRoleUsers);

            Notify("Email has been sent", "success");

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> EmailAllExternalCandidates()
        {

            var emailTemplates = await _dal.GetEmailTemplatesByProcessId((Guid)_state.ProcessId, EmailTypes.CandidateExternal);
            var emailTemplate = emailTemplates.FirstOrDefault();

            if (emailTemplate == null)
            {
                Notify("There is no email template", "danger");
                return RedirectToAction("Index");
            }

            var roleUsers = await _dal.GetRoleUsersByProcessId((Guid)_state.ProcessId);
            var externalRoleUsers = roleUsers.Where(x => (bool)x.IsExternal).ToList();

            await SendExteralEmailsForExternaRolelUsers(emailTemplate, externalRoleUsers);

            Notify("Email has been sent", "success");

            return RedirectToAction("Index");

        }

        #endregion

        #region TablePartial Methods

        [HttpGet]
        public IActionResult UpdateRoleUser(Guid roleUserId)
        {

            TempData[Constants.RoleUserIdToUpdate] = roleUserId;

            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoleUser(VmRoleUser vmRoleUser)
        {

            var dbRoleUser = await _dal.GetEntity<RoleUser>((Guid)vmRoleUser.Id) as RoleUser;

            // Handle RoleUser
            dbRoleUser.RoleUserType = (RoleUserTypes)vmRoleUser.RoleUserType;
            dbRoleUser.LanguageType = vmRoleUser.LanguageType == null ? null : (LanguageTypes)vmRoleUser.LanguageType;
            await _dal.UpdateEntity(dbRoleUser);

            // Handle Equities
            if (User.IsInRole(RoleTypes.Admin.ToString()))
            {
                var dbRoleUserEquities = await _dal.GetRoleUserEquitiesByRoleUserId((Guid)vmRoleUser.Id);
                var postedEquities = vmRoleUser.Equities.Where(x => x.IsSelected).ToList();

                // Delete Equities
                foreach (RoleUserEquity dbRoleUserEquity in dbRoleUserEquities)
                    if (!postedEquities.Any(x => x.Id == dbRoleUserEquity.EquityId))
                        await _dal.DeleteEntity<RoleUserEquity>(dbRoleUserEquity.Id);

                // Add Equities
                foreach (VmEquity vmEquity in postedEquities)
                {
                    if (!dbRoleUserEquities.Any(x => x.EquityId == vmEquity.Id))
                    {
                        RoleUserEquity roleUserEquity = new RoleUserEquity()
                        {
                            EquityId = (Guid)vmEquity.Id,
                            RoleUserId = (Guid)vmRoleUser.Id
                        };
                        await _dal.AddEntity<RoleUserEquity>(roleUserEquity);
                    }             
                }

            }

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> DeleteRoleUser(Guid roleUserId)
        {

            await _dal.DeleteEntity<RoleUser>(roleUserId);

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> EmailExternalUser(Guid roleUserId)
        {

            var emailTemplates = await _dal.GetEmailTemplatesByProcessId((Guid)_state.ProcessId, EmailTypes.CandidateExternal);
            var emailTemplate = emailTemplates.FirstOrDefault();

            if (emailTemplate == null)
            {
                Notify("There is no email template", "danger");
                return RedirectToAction("Index");
            }

            var roleuser = await _dal.GetEntity<RoleUser>(roleUserId) as RoleUser;
            var roleusers = new List<RoleUser>();

            roleusers.Add(roleuser);
            await SendExteralEmailsForExternaRolelUsers(emailTemplate, roleusers);

            Notify("Email has been sent", "success");

            return RedirectToAction("Index");

        }

        #endregion

        #region Private Email Methods

        private async Task SendExteralEmailsForExternaRolelUsers(EmailTemplate emailTemplate, List<RoleUser> externalRoleUsers)
        {

            var tokenResponse = await _tokenManager.GetToken();

            foreach (var externalRoleUser in externalRoleUsers)
            {
                var externalUser = await _dal.GetEntity<ExternalUser>(externalRoleUser.UserId) as ExternalUser;
                
                await SendExternalEmail(emailTemplate, externalUser, tokenResponse.access_token);

                externalRoleUser.DateExternalEmailSent = DateTime.Now;
                await _dal.UpdateEntity(externalRoleUser);
            }

        }

        private async Task SendExternalEmail(EmailTemplate emailTemplate, ExternalUser externalUser, string accessToken)
        {

            string body = emailTemplate.EmailBody
                .Replace("{0}", externalUser.GivenName)
                .Replace("{1}", externalUser.SurName)
                .Replace("{8}", externalUser.Id.ToString().Substring(0, 5))
                .Replace("{9}", "<strong>Get PathBase from somewhere - this is just a placeholder</strong>");

            EmailEnvelope emailEnvelope = new EmailEnvelope()
            {
                message = new EmailMessage()
                {
                    subject = emailTemplate.EmailSubject,
                    body = new EmailBody()
                    {
                        contentType = "HTML",
                        content = body
                    },
                    toRecipients = GetEmailRecipients(externalUser.Email),
                    //ccRecipients = GetEmailRecipients(emailTemplate.CcRecipients),
                },
                saveToSentItems = "false"
            };
            HttpResponseMessage responseMessage = await _emailsManager.SendEmailAsync(emailEnvelope, accessToken, User.Identity.Name);

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
