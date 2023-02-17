// Notes: This is the function that haven't been used


//this function clears data from all panels
function clearData() {
    $("#accountsBox").empty();
    $("#requestsBox").empty();
    clearNewAccount();
    clearLogon();
    clearEditAccount();
}


//when we retrieve accounts, we'll put them here
//so that we can reference them later for admins
//that may want to edit accounts
var accountsArray;
//to begin with, we assume that the account is not an admin
var admin = false;


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

//resets the edit account inputs
function clearEditAccount() {
    $('#editLogonId').val("");
    $('#editLogonPassword').val("");
    $('#editLogonFName').val("");
    $('#editLogonLName').val("");
    $('#editLogonEmail').val("");
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
