using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManage.EF.Areas.Management.Models
{
    
    public class userManageCommandViewModel
    {        
        public userManageCommand command { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public Guid UserId { get; set; }
        public int page { get; set; }
        public int pagesize { get; set; }
        public int skip { get; set; }
        public int take { get; set; }
        public bool requireTotalCount { get; set; }        
        public string sortOptions { get; set; }        
        public string filteroptions { get; set; }
    }
}
