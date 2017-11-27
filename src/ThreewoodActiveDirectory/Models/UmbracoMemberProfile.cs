using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreewoodActiveDirectory.Models
{
    public class UmbracoMemberProfile
    {
        private UserProfile _userProfile;
        private List<KeyValuePair<string, string>> _properties;
        private List<string> _roles;

        public UserProfile UserProfile
        {
            get { return _userProfile; }
        }

        public List<KeyValuePair<string, string>> Properties
        {
            get { return _properties; }
        }

        public List<string> Roles
        {
            get { return _roles; }
        }

        public UmbracoMemberProfile(UserProfile userProfile, List<KeyValuePair<string, string>> properties, List<string> roles)
        {
            _userProfile = userProfile;
            _properties = properties;
            _roles = roles;
        }
    }
}
