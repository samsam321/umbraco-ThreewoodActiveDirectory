using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreewoodActiveDirectory.Models
{
    public class ThreewoodActiveDirectoryResponse
    {
        public ThreewoodActiveDirectoryResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; set; }
        public string Message { get; set; }        
    }
}
