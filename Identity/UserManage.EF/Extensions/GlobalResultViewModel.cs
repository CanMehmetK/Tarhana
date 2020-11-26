using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManage.EF.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public class GlobalResultViewModel
    {
        public string type = "gresult";  
        public GlobalResultViewModel()
        {
            HasError = false;
        }        
        public string Status { get; set; }
        public string Message { get; set; }        
        public bool HasError { get; set; }
        public string ErrorType { get; set; }
        public Exception Exception { get; set; }        
        public Object RelatedData { get; set; }

    }
}
