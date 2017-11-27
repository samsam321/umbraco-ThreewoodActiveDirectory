using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Umbraco.Core.Logging;
using Umbraco.Web.Security.Providers;
using ThreewoodActiveDirectory.Helper;
using ThreewoodActiveDirectory.Models;
using Umbraco.Core.Models;

namespace ThreewoodActiveDirectory.Provider
{
    public class ExtendedMembersMembershipProvider : MembersMembershipProvider
    {        
        public override bool ValidateUser(string username, string password)
        {
            //MemberHelper.DeleteMembersOfType(1044);
            //List<string> groups = new List<string>();
            //groups.Add("Test Group");
            //ImportADAccount(CIC_DOMAIN, groups);

            ActiveDirectoryHelper activeDirectoryHelper = new ActiveDirectoryHelper();
            if(MemberHelper.FindMemberByUsername(username) != null && activeDirectoryHelper.ValidateUser(username, password))            
            {            
                return true;
            }
            else
            {
                return false;
            }            
        }
            
    }
}
