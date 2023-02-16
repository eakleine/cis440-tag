function TestButtonHandler() {
    var webMethod = "ProjectServices.asmx/TestConnection";
    var parameters = "{}";

    //jQuery ajax method
    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var responseFromServer = msg.d;
            alert(responseFromServer);
        },
        error: function (e) {
            alert("this code will only execute if javascript is unable to access the webservice");
        }
    });
}

var contentPanels = ['logonPanel', 'avatarChoosingPanel', 'newAccountPanel', 'editAccountPanel', 'requestsPanel'];
//this function toggles which panel is showing, and also clears data from all panels
function showPanel(panelId) {
    clearData();
    for (var i = 0; i < contentPanels.length; i++) {
        if (panelId == contentPanels[i]) {
            $("#" + contentPanels[i]).css("visibility", "visible");
        }
        else {
            $("#" + contentPanels[i]).css("visibility", "hidden");
        }
    }
}

//this function clears data from all panels
function clearData() {
    $("#accountsBox").empty();
    $("#requestsBox").empty();
    clearNewAccount();
    clearLogon();
    clearEditAccount();
}

//HERE'S AN EXAMPLE OF AN AJAX CALL WITH JQUERY!
function LogOn(userId, pass) {
    var webMethod = "ProjectServices.asmx/LogOn";
    var parameters = "{\"uid\":\"" + encodeURI(userId) + "\",\"pass\":\"" + encodeURI(pass) + "\"}";

    //jQuery ajax method
    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d) {
                //server replied true, so show the avatar choosing panel
                showPanel('avatarChoosingPanel');
                ShowMenu();
            }
            else {
                alert("logon failed");
            }
        },
        error: function (e) {
            alert("boo...");
        }
    });
}

//when we retrieve accounts, we'll put them here
//so that we can reference them later for admins
//that may want to edit accounts
var accountsArray;
//to begin with, we assume that the account is not an admin
var admin = false;

//this function grabs accounts and loads our account window
function LoadAccounts() {
    var webMethod = "ProjectServices.asmx/GetAccounts";
    $.ajax({
        type: "POST",
        url: webMethod,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d.length > 0) {
                //let's put our accounts that we get from the
                //server into our accountsArray variable
                //so we can use them in other functions as well
                accountsArray = msg.d;
                //this clears out the div that will hold our account info
                $("#accountsBox").empty();
                //again, we assume we're not an admin unless we see data from the server
                //that we know only admins can see
                admin = false;
                for (var i = 0; i < accountsArray.length; i++) {
                    //we grab on to a specific html element in jQuery
                    //by using a  # followed by that element's id.
                    var acct;
                    //if they have access to admin-level info (like userid and password) then
                    //create output that has an edit option
                    if (accountsArray[i].userId != null) {
                        acct = "<div class='accountRow' id='acct" + accountsArray[i].id + "'>" +
                            "<a class='nameTag' href='mailto:" + accountsArray[i].email + "'>" +
                            accountsArray[i].firstName + " " + accountsArray[i].lastName +
                            "</a> <a href='#' onclick='LoadAccount(" + accountsArray[i].id+")' class='optionsTag'>edit</a></div><hr>"
                        admin = true;
                    }
                    //if not, then they're not an admin so don't include the edit option
                    else {
                        acct = "<div class='accountRow' id='acct" + accountsArray[i].id + "'>" +
                            "<a class='nameTag' href='mailto:" + accountsArray[i].email + "'>" +
                            accountsArray[i].firstName + " " + accountsArray[i].lastName +
                            "</a></div><hr>"
                    }
                    $("#accountsBox").append(
                        //anything we throw at our panel in the form of text
                        //will be added to the contents of that panel.  Here
                        //we're putting together a div that holds info on the
                        //account as well as an edit link if the user is an admin
                        acct
                    );
                }
            }
            //we're showing the account window, so let's track that...
            accountWindowShowing = true;
            //...because the ShowMenu function behaves differently depending on
            //if the account window is showing or not
            ShowMenu();
        },
        error: function (e) {
            alert("boo...");
        }
    });
}

//this is just like loading accounts!
function LoadRequests() {
    var webMethod = "ProjectServices.asmx/GetAccountRequests";
    $.ajax({
        type: "POST",
        url: webMethod,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d.length > 0) {
                $("#requestsBox").empty();
                admin = false;
                for (var i = 0; i < msg.d.length; i++) {
                    req = "<div class='accountRow' id='acctR" + msg.d[i].id + "'>" +
                        "<span class='nameTag'>" +
                        msg.d[i].firstName + " " + msg.d[i].lastName +
                        "</span> <span class='optionsTag'><a href='#' onclick='approveAccount(" + msg.d[i].id + ")'>yes</a> / " +
                        "<a href='#' onclick='rejectAccount(" + msg.d[i].id + ")'>no</a></span>" +
                        "<div style='font-size: smaller'>"+msg.d[i].email+"</div></div > <hr>";
                    $("#requestsBox").append(req);
                }
            }
            accountWindowShowing = false;
            ShowMenu();
        },
        error: function (e) {
            alert("boo...");
        }
    });
}

//a simple ajax call that passes the id to be approved
function approveAccount(id) {
    var webMethod = "ProjectServices.asmx/ActivateAccount";
    var parameters = "{\"id\":\"" + encodeURI(id) + "\"}";

    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            showPanel('requestsPanel');
            LoadRequests();
        },
        error: function (e) {
            alert("boo...");
        }
    });
}

//virtually identical with approve
function rejectAccount(id) {
    var webMethod = "ProjectServices.asmx/RejectAccount";
    var parameters = "{\"id\":\"" + encodeURI(id) + "\"}";

    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            showPanel('requestsPanel');
            LoadRequests();
        },
        error: function (e) {
            alert("boo...");
        }
    });
}

//here's the variable to track if the account window is showing
var accountWindowShowing = false;
//and here's a function that adjusts the menu options if you're an admin or a user
//and if you're looking at accounts or requests
function ShowMenu() {

    $("#menu").css("visibility", "visible");
    if (admin) {
        if (accountWindowShowing) {
            $("#adminLink").text("requests");
        }
        else {
            $("#adminLink").text("accounts");
        }
    }
}

//just hides the menu
function HideMenu() {

    $("#menu").css("visibility", "hidden");
    $("#adminLink").text("");
}

//when an admin clicks either accounts or requests,
//they're shown teh appropriate screen
function adminClick() {
    if (accountWindowShowing) {
        //show requests
        showPanel('requestsPanel');
        accountWindowShowing = false;
        LoadRequests()
        ShowMenu();
    }
    else {
        showPanel('accountsPanel');
        LoadAccounts();
        ShowMenu();
    }
}

//resets the new account inputs
function clearNewAccount() {
    $('#newLogonId').val("");
    $('#newLogonPassword').val("");
    $('#newLogonFName').val("");
    $('#newLogonLName').val("");
    $('#newLogonEmail').val("");
}

//resets the edit account inputs
function clearEditAccount() {
    $('#editLogonId').val("");
    $('#editLogonPassword').val("");
    $('#editLogonFName').val("");
    $('#editLogonLName').val("");
    $('#editLogonEmail').val("");
}

//resets the logon inputs
function clearLogon() {
    $('#logonId').val("");
    $('#logonPassword').val("");
}

//passes account info to the server, to create an account request
function CreateAccount(id, pass, fname, lname, email) {
    var webMethod = "ProjectServices.asmx/RequestAccount";
    var parameters = "{\"uid\":\"" + encodeURI(id) + "\",\"pass\":\"" + encodeURI(pass) + "\",\"firstName\":\"" + encodeURI(fname) + "\",\"lastName\":\"" + encodeURI(lname) + "\",\"email\":\"" + encodeURI(email) + "\"}";

    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            showPanel('logonPanel');
            alert("Account request pending approval...");
        },
        error: function (e) {
            alert("boo...");
        }
    });
}

//logs the user off both at the client and at the server
function LogOff() {

    var webMethod = "ProjectServices.asmx/LogOff";
    $.ajax({
        type: "POST",
        url: webMethod,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d) {
                //we logged off, so go back to logon page,
                //stop checking messages
                //and clear the chat panel
                showPanel('logonPanel');
                HideMenu();
            }
            else {
            }
        },
        error: function (e) {
            alert("boo...");
        }
    });
}

//this function executes once jQuery has successfully loaded and is
//ready for business.  Usually, if we're wiring up event handlers via jQuery
//then this is where they go.
jQuery(function () {
    //when the app loads, show the logon panel to start with!
    showPanel('logonPanel');
});

//an ajax to kill an account
function DeactivateAccount() {
    var webMethod = "ProjectServices.asmx/DeleteAccount";
    var parameters = "{\"id\":\"" + encodeURI(currentAccount.id) + "\"}";

    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            showPanel('accountsPanel');
            LoadAccounts();
        },
        error: function (e) {
            alert("boo...");
        }
    });
}

//hold on to the account being currently edited here
var currentAccount;
//load up an account for editing
function LoadAccount(id) {
    showPanel('editAccountPanel');
    currentAccount = null;
    //find the account with a matching id in our array
    for (var i = 0; i < accountsArray.length; i++) {
        if (accountsArray[i].id == id) {
            currentAccount = accountsArray[i]
        }
    }
    //set up our inputs
    if (currentAccount != null) {
        $('#editLogonId').val(currentAccount.userId);
        $('#editLogonPassword').val(currentAccount.password);
        $('#editLogonFName').val(currentAccount.firstName);
        $('#editLogonLName').val(currentAccount.lastName);
        $('#editLogonEmail').val(currentAccount.email);
    }
}

//ajax to send the edits of an account to the server
function EditAccount() {
    var webMethod = "ProjectServices.asmx/UpdateAccount";
    var parameters = "{\"id\":\"" + encodeURI(currentAccount.id) + "\",\"uid\":\"" + encodeURI($('#editLogonId').val()) + "\",\"pass\":\"" + encodeURI($('#editLogonPassword').val()) + "\",\"firstName\":\"" + encodeURI($('#editLogonFName').val()) + "\",\"lastName\":\"" + encodeURI($('#editLogonLName').val()) + "\",\"email\":\"" + encodeURI($('#editLogonEmail').val()) + "\"}";

    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            showPanel('accountsPanel');
            LoadAccounts();
        },
        error: function (e) {
            alert("boo...");
        }
    });
}
