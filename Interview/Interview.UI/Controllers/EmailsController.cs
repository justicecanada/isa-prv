﻿using AutoMapper;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;

namespace Interview.UI.Controllers
{
    public class EmailsController : Controller
    {

        #region Declarations

        private readonly DalSql _dal;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public EmailsController(DalSql dal, IMapper mapper)
        {
            _dal = dal;
            _mapper = mapper;
        }

        #endregion

        #region Public Index Methods

        [HttpGet]
        public async Task<IActionResult> Index(Guid id)
        {

            var contest = await _dal.GetEntity<Contest>(id, true) as Contest;
            var vmContest = _mapper.Map<VmContest>(contest);

            return View(vmContest);

        }

        [HttpPost]
        public async Task<IActionResult> Save()
        {

            return View("Index");

        }

        #endregion

    }
}