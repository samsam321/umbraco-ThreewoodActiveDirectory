using ThreewoodActiveDirectory.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using Umbraco.Core.Logging;

namespace ThreewoodActiveDirectory.Helper
{
    public class ActiveDirectoryHelper
    {
        private DirectoryEntry _directoryEntry = null;

        private DirectoryEntry SearchRoot
        {
            get
            {
                if (_directoryEntry == null)
                {
                    using (HostingEnvironment.Impersonate())
                    {
                        _directoryEntry = new DirectoryEntry(LDAPPath);
                    }
                }
                return _directoryEntry;
            }
        }

        private String LDAPPath
        {
            get
            {
                return ConfigurationManager.AppSettings["LDAPConnectionString"];
            }
        }

        private String LDAPUser
        {
            get
            {
                return ConfigurationManager.AppSettings["LDAPUser"];
            }
        }

        private String LDAPPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["LDAPPassword"];
            }
        }

        private String LDAPDomain
        {
            get
            {
                return ConfigurationManager.AppSettings["LDAPDomain"];
            }
        }

        public List<UserProfile> GetAllUser()
        {
            return GetAllUser(string.Empty);
        }

        public List<UserProfile> GetAllUser(string ldapQueryString)
        {
            try
            {
                using (HostingEnvironment.Impersonate())
                {
                    _directoryEntry = null;
                    DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
                    directorySearch.Filter = string.Format("(&(objectClass=user)(objectClass=person)(!userAccountControl:1.2.840.113556.1.4.803:=2){0})", ldapQueryString);
                    directorySearch.PageSize = 500;
                    //directorySearch.SizeLimit = 20; //limited output 20 records for testing purpose only
                    SearchResultCollection allUsers = directorySearch.FindAll();

                    if (allUsers != null)
                    {
                        List<UserProfile> users = new List<UserProfile>();
                        foreach (SearchResult u in allUsers)
                        {
                            DirectoryEntry de = new DirectoryEntry(u.Path);
                            users.Add(UserProfile.GetUser(de));
                        }
                        return users;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<ActiveDirectoryHelper>("GetUserByFullName Exception: ", ex);
                return null;
            }
        }

        internal UserProfile GetUserByFullName(String userName)
        {
            try
            {
                using (HostingEnvironment.Impersonate())
                {
                    _directoryEntry = null;
                    DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
                    directorySearch.Filter = "(&(objectClass=user)(cn=" + userName + "))";
                    SearchResult results = directorySearch.FindOne();

                    if (results != null)
                    {
                        DirectoryEntry user = new DirectoryEntry(results.Path);
                        return UserProfile.GetUser(user);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<ActiveDirectoryHelper>("GetUserByFullName Exception: ", ex);
                return null;
            }
        }

        public UserProfile GetUserByLoginName(String userName)
        {
            try
            {
                using (HostingEnvironment.Impersonate())
                {
                    DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
                    directorySearch.Filter = "(&(objectClass=user)(SAMAccountName=" + userName + "))";
                    SearchResult results = directorySearch.FindOne();

                    if (results != null)
                    {
                        DirectoryEntry user = new DirectoryEntry(results.Path);
                        return UserProfile.GetUser(user);
                    }
                    return null;
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error<ActiveDirectoryHelper>("GetUserByLoginName Exception: ", ex);
                return null;
            }
        }

        public UserProfile GetUserDetailsByFullName(String FirstName, String MiddleName, String LastName)
        {
            try
            {
                using (HostingEnvironment.Impersonate())
                {
                    _directoryEntry = null;
                    DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);

                    if (FirstName != "" && MiddleName != "" && LastName != "")
                    {
                        directorySearch.Filter = "(&(objectClass=user)(givenName=" + FirstName + ")(initials=" + MiddleName + ")(sn=" + LastName + "))";
                    }
                    else if (FirstName != "" && MiddleName != "" && LastName == "")
                    {
                        directorySearch.Filter = "(&(objectClass=user)(givenName=" + FirstName + ")(initials=" + MiddleName + "))";
                    }
                    else if (FirstName != "" && MiddleName == "" && LastName == "")
                    {
                        directorySearch.Filter = "(&(objectClass=user)(givenName=" + FirstName + "))";
                    }
                    else if (FirstName != "" && MiddleName == "" && LastName != "")
                    {
                        directorySearch.Filter = "(&(objectClass=user)(givenName=" + FirstName + ")(sn=" + LastName + "))";
                    }
                    else if (FirstName == "" && MiddleName != "" && LastName != "")
                    {
                        directorySearch.Filter = "(&(objectClass=user)(initials=" + MiddleName + ")(sn=" + LastName + "))";
                    }
                    SearchResult results = directorySearch.FindOne();

                    if (results != null)
                    {
                        DirectoryEntry user = new DirectoryEntry(results.Path);
                        return UserProfile.GetUser(user);
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<ActiveDirectoryHelper>("GetUserDetailsByFullName Exception: ", ex);
                return null;
            }
        }
        
        public List<UserProfile> GetUserFromGroup(String groupName)
        {
            List<UserProfile> userlist = new List<UserProfile>();
            try
            {
                using (HostingEnvironment.Impersonate())
                {
                    _directoryEntry = null;
                    DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
                    directorySearch.Filter = "(&(objectClass=group)(SAMAccountName=" + groupName + "))";
                    SearchResult results = directorySearch.FindOne();
                    if (results != null)
                    {

                        DirectoryEntry deGroup = new DirectoryEntry(results.Path);
                        System.DirectoryServices.PropertyCollection pColl = deGroup.Properties;
                        int count = pColl["member"].Count;


                        for (int i = 0; i < count; i++)
                        {
                            string respath = results.Path;
                            string[] pathnavigate = respath.Split("CN".ToCharArray());
                            respath = pathnavigate[0];
                            string objpath = pColl["member"][i].ToString();
                            string path = respath + objpath;


                            DirectoryEntry user = new DirectoryEntry(path);
                            UserProfile userobj = UserProfile.GetUser(user);
                            userlist.Add(userobj);
                            user.Close();
                        }
                    }
                    return userlist;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<ActiveDirectoryHelper>("GetUserFromGroup Exception: ", ex);
                return userlist;
            }

        }        

        public List<UserProfile> GetUsersByFirstName(string fName)
        {
            using (HostingEnvironment.Impersonate())
            {
                List<UserProfile> userlist = new List<UserProfile>();
                string filter = "";

                _directoryEntry = null;
                DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
                directorySearch.Asynchronous = true;
                directorySearch.CacheResults = true;
                filter = string.Format("(givenName={0}*", fName);                

                directorySearch.Filter = filter;

                SearchResultCollection userCollection = directorySearch.FindAll();
                foreach (SearchResult users in userCollection)
                {
                    DirectoryEntry userEntry = new DirectoryEntry(users.Path);
                    UserProfile userInfo = UserProfile.GetUser(userEntry);

                    userlist.Add(userInfo);

                }

                directorySearch.Filter = "(&(objectClass=group)(SAMAccountName=" + fName + "*))";
                SearchResultCollection results = directorySearch.FindAll();
                if (results != null)
                {

                    foreach (SearchResult r in results)
                    {
                        DirectoryEntry deGroup = new DirectoryEntry(r.Path);

                        UserProfile agroup = UserProfile.GetUser(deGroup);
                        userlist.Add(agroup);
                    }

                }
                return userlist;
            }
        }
                
        public bool AddUserToGroup(string userLogin, string groupName)
        {
            try
            {
                using (HostingEnvironment.Impersonate())
                {
                    _directoryEntry = null;
                    Manager adManager = new Manager(LDAPDomain);
                    adManager.AddUserToGroup(userLogin, groupName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<ActiveDirectoryHelper>("AddUserToGroup Exception: ", ex);
                return false;
            }
        }
        
        public bool RemoveUserToGroup(string userlogin, string groupName)
        {
            try
            {
                using (HostingEnvironment.Impersonate())
                {
                    _directoryEntry = null;
                    Manager admanager = new Manager("xxx");
                    admanager.RemoveUserFromGroup(userlogin, groupName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<ActiveDirectoryHelper>("RemoveUserToGroup Exception: ", ex);
                return false;
            }
        }

        public bool ValidateUser(string username, string password)
        {
            try
            {
                using (HostingEnvironment.Impersonate())
                {
                    var connectionString = ConfigurationManager.ConnectionStrings["LDAPConnectionString"].ConnectionString;

                    string domainName = username.Split(@"\".ToCharArray())[0];
                    string userName = username.Split(@"\".ToCharArray())[1];

                    DirectoryEntry directoryEntry = new DirectoryEntry(connectionString, domainName + @"\" + userName, password);

                    DirectorySearcher searcher = new DirectorySearcher(directoryEntry);

                    searcher.Filter = "(SAMAccountName=" + userName + ")";

                    SearchResult result = searcher.FindOne();

                    return (result != null);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<ActiveDirectoryHelper>("ValidateUserByAD Exception: ", ex);
                return false;
            }
        }       
    }
}
