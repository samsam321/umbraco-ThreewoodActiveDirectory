using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using System.Web.Mvc;
using Umbraco.Web;
using ThreewoodActiveDirectory.Helper;
using ThreewoodActiveDirectory.Models;
using System.Web.Configuration;

namespace ThreewoodActiveDirectory.Controllers
{
    [PluginController("ThreewoodActiveDirectory")]
    public class DashboardController : UmbracoApiController
    {

        [HttpGet]
        public List<UserProfile> GetAllUsers()
        {
            ActiveDirectoryHelper activeDirectoryHelper = new ActiveDirectoryHelper();
            return activeDirectoryHelper.GetAllUser("(mail=*)(displayName=*)");
        }

        [HttpGet]
        public IEnumerable<string> GetAllMemberRoles()
        {            
            return MemberHelper.GetAllRoles();
        }

        [HttpPost]
        public ThreewoodActiveDirectoryResponse ImportUsers(MembersModel model)
        {
            string domain = WebConfigurationManager.AppSettings["ThreewoodActiveDirectory:DomainName"];
            int importedCount = 0;
            int updatedCount = 0;
            int failCount = 0;
            if (MemberHelper.ImportADAccount(ref importedCount, ref updatedCount, ref failCount, domain, model.Roles, model.Users))
            {
                var message = string.Format("{0} user(s) imported and {1} user(s) updated Successfully, {2} user(s) failure to update or import!", importedCount, updatedCount, failCount);
                return new ThreewoodActiveDirectoryResponse(true, message);
            }
            else
            {
                var message = "Import Failure";
                return new ThreewoodActiveDirectoryResponse(true, message);
            }
        }       

        [HttpDelete]
        public void DeleteAllMember()
        {
            MemberHelper.DeleteMembersOfType(1044);
        }
    }
}
