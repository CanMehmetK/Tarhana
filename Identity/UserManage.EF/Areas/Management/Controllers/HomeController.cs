using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.CustomIdentity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using Newtonsoft.Json;
using UserManage.EF.Models;
using UserManage.EF.Services;
using UserManage.EF.Extensions;
using UserManage.EF.Controllers;
using UserManage.EF.Areas.Management.Models;
using System.Linq;
using UserManage.EF.Areas.Management.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace UserManage.EF.Management.Controllers
{
    [Authorize(Roles = "Master,Manager")]
    [Area("Management")]
    public class HomeController : ManagementBaseController
    {               
        private readonly ILogger _logger;
        public HomeController(ILogger<UserController> logger)
        {            
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {            
            return View();
        }       
    }
}

