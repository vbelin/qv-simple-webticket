About
=====

SimpleWebTicket is a WebTicket solution for QlikView 11 with the aim of being as simple as possible to use and understand with minimal configuration in order to quickly be up and running on integrating QlikView with other systems.

Get Started
-----------

As can be seen in the Page_Load function below there's only one required thing to change. You need to provide the user identity for which you want to retreive the webticket in the _userId variable. Both _userFriendlyName and _userGroups are highly optional. This setup should work out of the box with a QlikView Server installation running on localhost with Windows Authentication.

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
        Response.Write(String.Format("Failed to retrieve web ticket for user id \"{0}\", try to verify the authentication settings.", _userId));
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

It's also strongly recommended to prohobit anonymous users in QlikView Server and last but not least set a custom login page for Authentication in the QlikView Web Server configuration and then you set this page which retrieves the webticket as login page of course. This will redirect the user to get a ticket when they're trying to access QlikView.

In Default.aspx.cs there are a couple of variables that needs to be changed according to different scenarious. Basically there are 3 ways to do this, all of them involves the options above. For example if using Option 2 above with IP whitelists and Anonymous Authentication in IIS the Anonymous variable needs to be set to true.

```c#
private const bool Anonymous = true;
```

Likewise, if using Option 1 and Windows Authentication the above flag should be false but then you need to supply the credentials in the UserName and Password variables.

```c#
private const string UserName = "QTSEL\\rfn";
private const string Password = "MyVerySecretPassword";
```

Instead of entering the username and password it's also possible to set the Application Pool for the website that runs the SimpleWebTicket code to a Windows account. Try using the "QlikView IIS" Application Pool for example.

This is it, this is the most common things people forget about when trying to do webtickets with QlikView.

Advanced
--------

Sometimes you want to bypass AccessPoint and go directly to an application instead. This is possible by specifying the application name in the RedirectToQlikView() function call, please note though that when using an application as target it's REQUIRED to also enter the hostname of the QlikView Server.

```c#
if (!String.IsNullOrEmpty(_webTicket))
    RedirectToQlikView("Movies Database.qvw", "QVS@sesth-rfn");
```

To go even further you might want to select something inside of the application. This is also possible, but unfortunately limited to only ONE selection at this time due to how everything works. I've tried to work around it with little success, any ideas for a solution is welcome! Still, this works...

```c#
if (!String.IsNullOrEmpty(_webTicket))
    RedirectToQlikView("Movies Database.qvw", "QVS@sesth-rfn", "LB39,Banana");
```

Good To Know
------------

If you want to include SimpleWebTicket into an already existing project then you need to copy the code from Default.aspx.cs together with the Commhelper class. Don't forget to change the namespace in the classes to suit your project and you should be good to go!

License
-------

This software is made available "AS IS" without warranty of any kind under The Mit License (MIT). QlikTech support agreement does not cover support for this software.

Meta
----

* Code: `git clone git://github.com/braathen/qv-simple-webticket.git`
* Home: <https://github.com/braathen/qv-simple-webticket>
* Bugs: <https://github.com/braathen/qv-simple-webticket/issues>
