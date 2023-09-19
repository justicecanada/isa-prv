﻿using AutoMapper;
using Interview.Entities;
using Interview.UI.Data;
using Interview.UI.Models;
using Interview.UI.Services.Automapper;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;

namespace Interview.UI.Controllers
{
    public class ContestsController : GoC.WebTemplate.CoreMVC.Controllers.WebTemplateBaseController
    {

        #region Declarations

        private readonly DalSql _dal;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public ContestsController(DalSql dal, IMapper mapper, GoC.WebTemplate.Components.Core.Services.IModelAccessor modelAccessor)
            : base(modelAccessor)
        {
            _dal = dal;
            _mapper = mapper;
        }

        #endregion

        #region Index Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var contests = await _dal.GetAllContests();
            var vmContests = _mapper.Map(contests, typeof(List<Contest>), typeof(List<VmContest>));

            ViewBag.VmContests = vmContests;

            return View();

        }

        [HttpGet]
        public async Task<IActionResult> DeleteContest(Guid id)
        {

            await _dal.DeleteEntity<Contest>(id);

            return RedirectToAction("Index");

        }

        #endregion

        #region Manage Methods

        [HttpGet]   
        public async Task<IActionResult> Contest(Guid? id)
        {

            VmContest vmContest = null;

            if (id == null)
            {
                vmContest = new VmContest();
            }
            else
            {
                var contest = await _dal.GetEntity<Contest>((Guid)id, true) as Contest;
                vmContest = _mapper.Map<VmContest>(contest);
            }

            return View(vmContest);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContestNext(VmContest vmContest)
        {
            
            if (ModelState.IsValid)
            {

                var contest = _mapper.Map<Contest>(vmContest);

                if (vmContest.Id == null)
                    await _dal.AddEntity(contest);
                else
                    await _dal.UpdateEntity(contest);

                return RedirectToAction("Index", "Emails", new { id = vmContest.Id });

            }
            else
            {
                return View("Contest", vmContest);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContestSave(VmContest vmContest)
        {

            if (ModelState.IsValid)
            {

                var contest = _mapper.Map<Contest>(vmContest);

                if (vmContest.Id == null)
                    await _dal.AddEntity(contest);
                else
                    await _dal.UpdateEntity(contest);

                return RedirectToAction("Index", "Default");

            }
            else
            {
                return View("Contest", vmContest);
            }

        }

        #endregion

    }

}
