using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.CustomIdentity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using UserManage.EF.Areas.Management.Controllers;
using UserManage.EF.Areas.Management.Models;
using UserManage.EF.Models;
using UserManage.EF.Services;

namespace UserManage.EF.Management.Controllers
{
    [Area("Management")]
    [Authorize(Roles = "Master")]
    public class TenantController : ManagementBaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public TenantController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {            
            return View();
        }

        [HttpPost]
        public IActionResult Users([FromBody]userManageCommandViewModel parameter)
        {            
            switch (parameter.command)
            {
                case userManageCommand.getuserlist:

                    var query = _userManager.Users;

                    if (!string.IsNullOrEmpty(parameter.sortOptions))
                    {
                        //             sortOptions:"[{"selector":"ad","desc":false}]"
                        var sortOptions = JObject.Parse(JArray.Parse(parameter.sortOptions)[0].ToString());
                        var columnName = (string)sortOptions.SelectToken("selector");
                        var descending = (bool)sortOptions.SelectToken("desc");
                        if (descending)
                            columnName += " DESC";
                        query = query.OrderBy(columnName, "");
                    }

                    // filteroptions : "[["id","contains","s"],"and",["ad","contains","t"]]"
                    if (!string.IsNullOrEmpty(parameter.filteroptions))
                    {
                        var filterTree = JArray.Parse(parameter.filteroptions);
                        if (filterTree[0].Type == JTokenType.String)
                        {
                            var ColumnName = filterTree[0].ToString();
                            var Clause = filterTree[1].ToString();
                            var Value = filterTree[2].ToString();

                            switch (filterTree[1].ToString())
                            {
                                case "=":
                                    Value = System.Text.RegularExpressions.Regex.IsMatch(Value, @"^\d+$") ? Value : String.Format("\"{0}\"", Value);
                                    query = query.Where(String.Format("{0} == {1}", ColumnName, Value));
                                    break;
                                case "contains":
                                    query = query.Where(ColumnName + ".Contains(@0)", Value);
                                    break;
                                case "<>":
                                    query = query.Where(string.Format("!{0}.StartsWith(\"{1}\")", ColumnName, Value));
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < filterTree.Count; i++)
                            {
                                if (filterTree[i].ToString().Equals("and"))
                                    continue;
                                var ColumnName = filterTree[i][0].ToString();
                                var Clause = filterTree[i][1].ToString();
                                var Value = filterTree[i][2].ToString();
                                switch (filterTree[i][1].ToString())
                                {
                                    case "=":
                                        Value = System.Text.RegularExpressions.Regex.IsMatch(Value, @"^\d+$") ? Value : String.Format("\"{0}\"", Value);
                                        query = query.Where(String.Format("{0} == {1}", ColumnName, Value));
                                        break;
                                    case "contains":
                                        query = query.Where(ColumnName + ".Contains(@0)", Value);
                                        break;
                                    case "<>":
                                        query = query.Where(string.Format("!{0}.StartsWith(\"{1}\")", ColumnName, Value));
                                        break;
                                    default:
                                        break;
                                }

                            }


                        }
                        // ReadExpression(query, filterTree);
                    }

                    query = query.Skip(parameter.skip).Take(parameter.take);
                    return new JsonResult (new { data = query.ToList(), totalCount = _userManager.Users.LongCount() });
                    
            }
            return new JsonResult(new { });
        }    
    }
}

