About
=====

SimpleWebTicket is a WebTicket solution for QlikView 11 with the aim of being as simple as possible to use and understand with minimal configuration in order to quickly be up and running on integrating QlikView with other systems.

Get Started
-----------

As can be seen in the Page_Load function below there's only one required thing to change. You need to provide the user identity for which you want to retreive the webticket in the _userId variable. Both _userFriendlyName and _userGroups are highly optional.

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

License
-------

This software is made available "AS IS" without warranty of any kind under The Mit License (MIT). QlikTech support agreement does not cover support for this software.

Meta
----

* Code: `git clone git://github.com/braathen/qv-simple-webticket.git`
* Home: <https://github.com/braathen/qv-simple-webticket>
* Bugs: <https://github.com/braathen/qv-simple-webticket/issues>
