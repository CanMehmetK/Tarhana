using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManage.EF.Extensions;

namespace UserManage.EF.Areas.Management.Controllers
{
    [Area("Management")]    
    public class ManagementBaseController : Controller
    {
        public ManagementBaseController()
        {

        }
    }
}