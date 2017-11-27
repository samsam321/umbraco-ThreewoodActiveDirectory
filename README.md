# Threewood Active Directory

Threewood Active Directory for Umbraco version 7.X
	
Thank you for installing Threewood Umbraco ThreewoodActiveDirectory.
	
When you uninstall package completed, it will show a error message, the reason is the dll deleted and un-referenced.You need modify web.config by yourself to umbraco default setting. Add the default UmbracoMembershipProvider to web.config.
	
Example :

```
<add name="UmbracoMembershipProvider" type="Umbraco.Web.Security.Providers.MembersMembershipProvider, Umbraco" minRequiredNonalphanumericCharacters="0" minRequiredPasswordLength="10" useLegacyEncoding="false" enablePasswordRetrieval="false" enablePasswordReset="false" requiresQuestionAndAnswer="false" defaultMemberTypeAlias="Member" passwordFormat="Hashed" allowManuallyChangingPassword="false" requiresUniqueEmail="false" />
```

          
After installed,
	
1. In Membership provider section add

```	
<add name="UmbracoMembershipProvider" type="ThreewoodActiveDirectory.Provider.ExtendedMembersMembershipProvider, ThreewoodActiveDirectory" minRequiredNonalphanumericCharacters="0" minRequiredPasswordLength="8" useLegacyEncoding="true" enablePasswordRetrieval="false" enablePasswordReset="true" requiresQuestionAndAnswer="false" defaultMemberTypeAlias="Member" passwordFormat="Hashed" allowManuallyChangingPassword="true"/>
```

this setting will let umbraco use ThreewoodActiveDirectory As Membership Provider, member login authenicate by your own AD
	
	
2. In  <appSettings> section add

```
<!-- ThreewoodActiveDirectory-->
<add key="ThreewoodActiveDirectory:DomainName" value="your AD domain host name"/>    
<!-- End of ThreewoodActiveDirectory-->
```	
	
this setting will affect the login name of member
	
example : 
	
domain host name = abc

```	
<add key="ThreewoodActiveDirectory:DomainName" value="abc"/>    
```
	
all member imported login name become abc\yourlogin
	
3. In  <connectionStrings> add
	
```    
<add name="LDAPConnectionString" connectionString="LDAP://yourownAD" />
```