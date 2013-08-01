About
=====

SimpleWebTicket is a WebTicket solution for QlikView 11 with the aim of being as simple as possible to use and understand with minimal configuration in order to quickly be up and running on integrating QlikView with other systems.

Get Started
-----------

As can be seen in the Page_Load function below there's only one required thing to change. You need to provide the user identity for which you want to retreive the webticket in the _userId variable. Both _userFriendlyName and _userGroups are highly optional.

```c#
protected void Page_Load(object sender, EventArgs e)
{
    // [REQUIRED] Name of the user to get a ticket for
    _userId = "rfn";
    // [OPTIONAL] A friendly name for the user. For example if the username is a social security number of phonenumber the friendly name could be his/hers real name
    _userFriendlyName = "Rikard Braathen";
    // [OPTIONAL] Semicolon separated string with groups/roles the user belongs to for use with Section Access or authorization
    _userGroups = "PreSales;Europe;Stockholm Office";

    GetWebTicket();

    if (!String.IsNullOrEmpty(_webTicket))
        RedirectToQlikView();
    else
        Response.Write("An error occured!");
}
```

Configuration
-------------

* First of all, while not mandatory it can be easier to use IIS for the QlikView Web Server when working with ticketing. It may be required to host your login page anyway.
* QlikView needs to be an Enterprise Edition Licence
* QlikView needs to be running in DMS mode for security (see manual)
* The QlikView web site in IIS needs to be set up to use Anonymous permissions – it will be expecting windows permissions by default – specifically it is the QVAJAXZFC directory that needs its permission changing.
* QlikView needs to trust the code asking for the ticket. There is a web page within the QlikView web server called GetWebTicket.aspx which handles requests for tickets, this will only return a ticket to a trusted user/process and this is identified using one of two options:

* __Option 1 – use windows permissions__
  * The code or process asking for a ticket needs to run as or provide a windows user identity. The user ID must be a member of the QlikView Administrators windows group on the QlikView server.
  * As the GetWebTicket page runs under the QVAJAXZFC directory on the web server and one of the above steps made the directory work with Anonymous users –for this page only– enable windows authentication in IIS
  * Now the Login page must authenticate itself when requesting a ticket as a windows user and that user must be a QlikView Administrator. In the attached example the login is hardcoded, however this could be configured in other ways

* __Option 2 – use an IP address white list__
  * For some code technologies NTLM is not available or it may just not be appropriate to use it. For these scenarios a “white list” of approved IP addresses can be used rather than a named user. Using this approach the GetWebTicket page will only return a ticket to code running from a specific IP address
  * To configure this option:
    * Open the web server config file from C:\ProgramData\QlikTech\WebServer\config.xml
    * Locate the line &lt;GetWebTicket url="/QvAjaxZfc/GetWebTicket.aspx" /&gt;
    * Replace it with the following specifying the IP address(s) of the web server(s) running the code

```xml
<GetWebTicket url="/QvAjaxZfc/GetWebTicket.aspx">
    <TrustedIP>192.168.0.1</TrustedIP>
</GetWebTicket>
```


License
-------

This software is made available "AS IS" without warranty of any kind under The Mit License (MIT). QlikTech support agreement does not cover support for this software.

Meta
----

* Code: `git clone git://github.com/braathen/qv-simple-webticket.git`
* Home: <https://github.com/braathen/qv-simple-webticket>
* Bugs: <https://github.com/braathen/qv-simple-webticket/issues>
