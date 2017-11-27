using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreewoodActiveDirectory
{
    public class Manager
    {

        PrincipalContext context;

        public Manager()
        {
            context = new PrincipalContext(ContextType.Machine, "xxx", "xxx", "xxx");
        }
        
        public Manager(string domain, string container)
        {
            context = new PrincipalContext(ContextType.Domain, domain, container);
        }

        public Manager(string domain)//, string username, string password)
        {
            context = new PrincipalContext(ContextType.Domain);//, username, password);
        }

        public bool AddUserToGroup(string userName, string groupName)
        {
            bool done = false;
            GroupPrincipal group = GroupPrincipal.FindByIdentity(context, groupName);
            if (group == null)
            {
                group = new GroupPrincipal(context, groupName);
            }
            UserPrincipal user = UserPrincipal.FindByIdentity(context, userName);
            if (user != null & group != null)
            {
                group.Members.Add(user);
                group.Save();
                done = (user.IsMemberOf(group));
            }
            return done;
        }

        public bool RemoveUserFromGroup(string userName, string groupName)
        {
            bool done = false;
            UserPrincipal user = UserPrincipal.FindByIdentity(context, userName);
            GroupPrincipal group = GroupPrincipal.FindByIdentity(context, groupName);
            if (user != null & group != null)
            {
                group.Members.Remove(user);
                group.Save();
                done = !(user.IsMemberOf(group));
            }
            return done;
        }
    }
}
