using ThreewoodActiveDirectory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;

namespace ThreewoodActiveDirectory.Helper
{
    public class MemberHelper
    {
        private const string GUID = "guid";
        public static IMember FindMemberByEmail(string email)
        {
            return ApplicationContext.Current.Services.MemberService.GetByEmail(email);
        }

        public static IMember FindMemberByUsername(string username)
        {
            return ApplicationContext.Current.Services.MemberService.GetByUsername(username);            
        }

        public static IEnumerable<IMember> FindMemberByProperty(string property, string value)
        {
            return ApplicationContext.Current.Services.MemberService.GetMembersByPropertyValue(property, value);            
        }

        public static IEnumerable<IMember> FindMemberByGuid(string guid)
        {
            return FindMemberByProperty(GUID, guid);
        }

        public static void AssignRole(IMember member, List<string> roles)
        {
            if (roles != null)
            {
                foreach (string role in roles)
                {
                    if (!String.IsNullOrEmpty(role))
                    {
                        ApplicationContext.Current.Services.MemberService.AssignRole(member.Id, role);
                    }
                }
            }
        }
        
        public static int CreateMember(string usernameWithDomain, string name, string email, string password = null, List<string> roles = null, List<KeyValuePair<string, string>> properties = null, string memberType = "Member")
        {            
            try
            {
                IMember member = ApplicationContext.Current.Services.MemberService.CreateMember(usernameWithDomain, email, name, memberType);

                member.IsApproved = true;
                if (properties != null)
                {
                    foreach (var property in properties)
                    {
                        member.SetValue(property.Key, property.Value);
                    }
                }

                ApplicationContext.Current.Services.MemberService.Save(member);

                if (string.IsNullOrEmpty(password))
                {
                    password = Guid.NewGuid().ToString().Substring(0, 8);
                }
                ApplicationContext.Current.Services.MemberService.SavePassword(member, password);

                AssignRole(member, roles);
                //if (roleNames != null)
                //{
                //    foreach (string roleName in roleNames)
                //    {
                //        if (!String.IsNullOrEmpty(roleName))
                //        {
                //            ApplicationContext.Current.Services.MemberService.AssignRole(member.Id, roleName);
                //        }
                //    }
                //}

                LogHelper.Info<MemberHelper>(string.Format("{0}(name) created, {1}(id)", name, member.Id));
                return member.Id;
            }
            catch (Exception ex)
            {
                LogHelper.Error<MemberHelper>("CreateMember exception: ", ex);
                return -1;
            }
        }

        public static int CreateMember(UmbracoMemberProfile member, string domain)
        {
            string usernameWithDomain = string.Empty;

            if (!string.IsNullOrEmpty(domain))
            {
                usernameWithDomain = string.Format("{0}\\{1}", domain, member.UserProfile.LoginName);
            }
            else
            {
                usernameWithDomain = member.UserProfile.LoginNameWithDomain;
            }            

            return CreateMember(usernameWithDomain, member.UserProfile.DisplayName, member.UserProfile.EmailAddress, null, member.Roles, member.Properties);            
        }

        public static int CreateMember(UmbracoMemberProfile member)
        {
            return CreateMember(member, null);            
        }

        public static bool UpdateMember(UmbracoMemberProfile member)
        {
            try
            {
                IEnumerable<IMember> members = FindMemberByGuid(member.Properties.Where(x => x.Key == GUID).Select(x => x.Value).FirstOrDefault());
                if (members != null)
                {
                    foreach (IMember m in members)
                    {
                        m.Name = member.UserProfile.DisplayName;
                        m.Email = member.UserProfile.EmailAddress;
                        ApplicationContext.Current.Services.MemberService.Save(m);
                        AssignRole(m, member.Roles);
                        LogHelper.Info<MemberHelper>(string.Format("{0}(name) updated, {1}(id)", m.Name, m.Id));
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                LogHelper.Error<MemberHelper>("UpdateMember exception: ", ex);
                return false;
            }
        }

        public static void DeleteMembersOfType(int type)
        {           
            ApplicationContext.Current.Services.MemberService.DeleteMembersOfType(type);
        }

        public static bool ImportADAccount(ref int importedCount, ref int updatedCount, ref int failCount, string domain = null, List<string> groups = null)
        {
            ActiveDirectoryHelper activeDirectoryHelper = new ActiveDirectoryHelper();
            List<UserProfile> users = activeDirectoryHelper.GetAllUser("(mail=*)(displayName=*)");
            return ImportADAccount(ref importedCount, ref updatedCount, ref failCount, domain, groups, users);
        }

        public static bool ImportADAccount(ref int importedCount, ref int updatedCount, ref int failCount, string domain = null, List<string> groups = null, List<UserProfile> users = null)
        {
            bool result = false;
            
            if (users != null)
            {                
                foreach (UserProfile user in users)
                {
                    List<KeyValuePair<string, string>> properties = new List<KeyValuePair<string, string>>();
                    properties.Add(new KeyValuePair<string, string>("guid", user.GUID.ToString()));
                    properties.Add(new KeyValuePair<string, string>("distinguishedName", user.DistinguishedName));
                    UmbracoMemberProfile member = new UmbracoMemberProfile(user, properties, groups);

                    if (MemberHelper.FindMemberByGuid(user.GUID.ToString()).Count() == 0)
                    {
                        if(MemberHelper.CreateMember(member, domain) != -1)
                        {
                            importedCount++;
                        }
                        else
                        {
                            failCount++;
                        }
                    }
                    else
                    {
                        if (MemberHelper.UpdateMember(member))
                        {
                            updatedCount++;
                        }
                        else
                        {
                            failCount++;
                        }

                    }
                }
                result = true;
            }
            return result;
        }

        public static IEnumerable<string> GetAllRoles()
        {
            return ApplicationContext.Current.Services.MemberService.GetAllRoles();
        }
    }
}
