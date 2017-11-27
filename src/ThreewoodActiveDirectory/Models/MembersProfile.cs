using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreewoodActiveDirectory.Models
{
    public class MembersModel
    {
        public MembersModel()
        {
            
        }

        public List<UserProfile> Users { get; set; }
        public List<string> Roles { get; set; }
    }
}
