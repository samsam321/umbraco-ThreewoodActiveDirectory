using ThreewoodActiveDirectory.Config;
using ThreewoodActiveDirectory.Helper;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreewoodActiveDirectory.Models
{
    public class UserProfile
    {        
        private String _manager;
        private String _managerName;

        public UserProfile()
        {                
        }

        public UserProfile Manager
        {
            get
            {
                if (!String.IsNullOrEmpty(_managerName))
                {
                    ActiveDirectoryHelper activeDirectoryHelper = new ActiveDirectoryHelper();
                    return activeDirectoryHelper.GetUserByFullName(_managerName);
                }
                return null;
            }
        }

        public String ManagerName
        {
            get { return _managerName; }
        }

        public Guid GUID { get; set; }
        public string DistinguishedName { get; set; }
        public String Department { get; set; }
        public String FirstName { get; set; }
        public String MiddleName { get; set; }
        public String LastName { get; set; }
        public String DisplayName { get; set; }
        public String LoginName { get; set; }
        public String LoginNameWithDomain { get; set; }
        public String StreetAddress { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public String PostalCode { get; set; }
        public String Country { get; set; }
        public String HomePhone { get; set; }
        public String Extension { get; set; }
        public String Mobile { get; set; }
        public String Fax { get; set; }
        public String EmailAddress { get; set; }
        public String Title { get; set; }
        public String Company { get; set; }

        private UserProfile(DirectoryEntry directoryUser)
        {

            String domainAddress;
            String domainName;

            GUID = directoryUser.Guid;
            DistinguishedName = GetProperty(directoryUser, Properties.DISTINGUISHEDNAME);
            FirstName = GetProperty(directoryUser, Properties.FIRSTNAME);
            MiddleName = GetProperty(directoryUser, Properties.MIDDLENAME);
            LastName = GetProperty(directoryUser, Properties.LASTNAME);
            DisplayName = GetProperty(directoryUser, Properties.DISPLAYNAME);
            LoginName = GetProperty(directoryUser, Properties.LOGINNAME);
            String userPrincipalName = GetProperty(directoryUser, Properties.USERPRINCIPALNAME);

            if (!string.IsNullOrEmpty(userPrincipalName))
            {
                domainAddress = userPrincipalName.Split('@')[1];
            }
            else
            {
                domainAddress = String.Empty;
            }

            if (!string.IsNullOrEmpty(domainAddress))
            {
                domainName = domainAddress.Split('.').First();
            }
            else
            {
                domainName = String.Empty;
            }

            LoginNameWithDomain = String.Format(@"{0}\{1}", domainName, LoginName);
            StreetAddress = GetProperty(directoryUser, Properties.STREETADDRESS);
            City = GetProperty(directoryUser, Properties.CITY);
            State = GetProperty(directoryUser, Properties.STATE);
            PostalCode = GetProperty(directoryUser, Properties.POSTALCODE);
            Country = GetProperty(directoryUser, Properties.COUNTRY);
            Company = GetProperty(directoryUser, Properties.COMPANY);
            Department = GetProperty(directoryUser, Properties.DEPARTMENT);
            HomePhone = GetProperty(directoryUser, Properties.HOMEPHONE);
            Extension = GetProperty(directoryUser, Properties.EXTENSION);
            Mobile = GetProperty(directoryUser, Properties.MOBILE);
            Fax = GetProperty(directoryUser, Properties.FAX);
            EmailAddress = GetProperty(directoryUser, Properties.EMAILADDRESS);
            Title = GetProperty(directoryUser, Properties.TITLE);
            _manager = GetProperty(directoryUser, Properties.MANAGER);

            if (!String.IsNullOrEmpty(_manager))
            {
                String[] managerArray = _manager.Split(',');
                _managerName = managerArray[0].Replace("CN=", "");
            }
        }


        private static String GetProperty(DirectoryEntry userDetail, String propertyName)
        {
            if (userDetail.Properties.Contains(propertyName))
            {
                return userDetail.Properties[propertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static UserProfile GetUser(DirectoryEntry directoryUser)
        {
            return new UserProfile(directoryUser);
        }
    }
}
