var DropOffMarker;
var DropOffText;
var waypointx = 0;
var waypointy = 0;
var parcelsintruck = 0;
var capacitytruck = 0;
var dropoffamount = 0;
var isCEFBrowserOpen = false;
var isPlayerOnJob = false;

API.onServerEventTrigger.connect(function (eventName, args) {

    if (eventName === "SpawnJobVehicleSubtitle") {
        API.displaySubtitle("Press Y to spawn a vehicle", 5000);
    }
    else if (eventName === "JobVehicleStartJob") {
        if (isPlayerOnJob === false) {
            API.displaySubtitle("Press Y to start the job", 5000);
        }
    }
    else if (eventName === "FillGoPostalTruck") {
        parcelsintruck = 10;
    }
    else if (eventName === "FillMoneyTransportTruck") {
        capacitytruck = 100000;
    }
    else if (eventName === "SetFirstJobDropOffLocation") {
        //============================
        //Go Postal Set First Drop Off
        //============================
        if (args[4] === "Go Postal") {
            DropOffMarker = API.createMarker(1, args[0], new Vector3(), new Vector3(), new Vector3(3, 3, 2), 125, 198, 255, 64);
            DropOffText = API.createTextLabel("Press E to drop off the package", args[1], 15.0, 1.0);
            waypointx = args[2];
            waypointy = args[3];
            API.setWaypoint(waypointx, waypointy);
            API.sendNotification("Take your parcel to the location marked on your GPS");
        }
        else if (args[4] === "Money Transport") {
            DropOffMarker = API.createMarker(1, args[0], new Vector3(), new Vector3(), new Vector3(6, 6, 4), 125, 198, 255, 64);
            DropOffText = API.createTextLabel("Press E to unload your truck", args[1], 15.0, 1.0);
            waypointx = args[2];
            waypointy = args[3];
            API.setWaypoint(waypointx, waypointy);
            dropoffamount = args[5];
        }
    }
    else if (eventName === "SetJobDropOffLocation") {
        //==========================
        //Go Postal Set Drop Off
        //==========================
        if (args[4] === "Go Postal") {
            parcelsintruck--;
            if (parcelsintruck <= 0) {
                GoPostalReStartJob();
            }
            else {
                DropOffMarker = API.createMarker(1, args[0], new Vector3(), new Vector3(), new Vector3(3, 3, 2), 125, 198, 255, 64);
                DropOffText = API.createTextLabel("Press E to drop off the package", args[1], 15.0, 1.0);
                waypointx = args[2];
                waypointy = args[3];
                API.setWaypoint(waypointx, waypointy);
                API.sendNotification("Take your parcel to the location marked on your GPS");
                isPlayerOnJob = true;
            }
        }
        else if (args[4] === "Money Transport") {
            capacitytruck -= dropoffamount;
            DropOffMarker = API.createMarker(1, args[0], new Vector3(), new Vector3(), new Vector3(6, 6, 4), 125, 198, 255, 64);
            DropOffText = API.createTextLabel("Press E to unload your truck", args[1], 15.0, 1.0);
            waypointx = args[2];
            waypointy = args[3];
            API.setWaypoint(waypointx, waypointy);
            isPlayerOnJob = true;
            dropoffamount = args[5];
        }
    }
    else if (eventName === "DestroyJobDropOffLocation") {
        if (isPlayerOnJob === true) {
            API.deleteEntity(DropOffMarker);
            API.deleteEntity(DropOffText);
        }
    }
    else if (eventName === "StopJobActivated") {
        if (isPlayerOnJob === true) {
            API.deleteEntity(DropOffMarker);
            API.deleteEntity(DropOffText);
            isPlayerOnJob = false;
        }
    }
    else if (eventName === "GoPostalJobCEFBrowser") {
        if (isCEFBrowserOpen === false) {
            var res = API.getScreenResolution(); //this gets the client's screen resoulution
            myBrowser = API.createCefBrowser(res.Width, res.Height); //we're initializing the browser here. This will be the full size of the user's screen.
            API.waitUntilCefBrowserInit(myBrowser); //this stops the script from getting ahead of itself, it essentially pauses until the browser is initialized
            API.setCefBrowserPosition(myBrowser, 0, 0); //The orientation (top left) corner in relation to the user's screen.  This is useful if you do not want a full page browser.  0,0 is will lock the top left corner of the browser to the top left of the screen.
            API.loadPageCefBrowser(myBrowser, "clientside/web/gopostaljobmenu.html"); //This loads the HTML file of your choice.      .    API.setCefBrowserHeadless(myBrowser, true); //this will remove the scroll bars from the bottom/right side
            API.showCursor(true); //This will show the mouse cursor
            API.setCanOpenChat(false);  //This disables the chat, so the user can type in a form without opening the chat and causing issues.
            isCEFBrowserOpen = true;
        }
    }
    else if (eventName === "MoneyTransportJobCEFBrowser") {
        if (isCEFBrowserOpen === false) {
            var res = API.getScreenResolution(); //this gets the client's screen resoulution
            myBrowser = API.createCefBrowser(res.Width, res.Height); //we're initializing the browser here. This will be the full size of the user's screen.
            API.waitUntilCefBrowserInit(myBrowser); //this stops the script from getting ahead of itself, it essentially pauses until the browser is initialized
            API.setCefBrowserPosition(myBrowser, 0, 0); //The orientation (top left) corner in relation to the user's screen.  This is useful if you do not want a full page browser.  0,0 is will lock the top left corner of the browser to the top left of the screen.
            API.loadPageCefBrowser(myBrowser, "clientside/web/moneytransportjobmenu.html"); //This loads the HTML file of your choice.      .    API.setCefBrowserHeadless(myBrowser, true); //this will remove the scroll bars from the bottom/right side
            API.showCursor(true); //This will show the mouse cursor
            API.setCanOpenChat(false);  //This disables the chat, so the user can type in a form without opening the chat and causing issues.
            isCEFBrowserOpen = true;
        }
    }
});

//===============================
//===== Go Postal Functions =====
//===============================

function GoPostalStartJob() {
    if (isPlayerOnJob === false) {
        API.showCursor(false); //stop showing the cursor
        API.destroyCefBrowser(myBrowser); //destroy the CEF browser
        API.setCanOpenChat(true); //allow the player to use the chat again.
        isCEFBrowserOpen = false;
        DropOffMarker = API.createMarker(1, new Vector3(68.7, 127.5572, 78.11868), new Vector3(), new Vector3(), new Vector3(20, 20, 3), 125, 198, 255, 64);
        DropOffText = API.createTextLabel("Press E to fill your truck", new Vector3(68.7, 127.5572, 78.11868), 15.0, 1.0);
        isPlayerOnJob = true;
        API.setWaypoint(75.89594, 110.0569);
        API.sendNotification("Go Post: You GPS has been set to the pickup dock");
    }
    else API.sendChatMessage("You already have a job");
}

function GoPostalReStartJob() {
        DropOffMarker = API.createMarker(1, new Vector3(68.7, 127.5572, 78.11868), new Vector3(), new Vector3(), new Vector3(20, 20, 3), 125, 198, 255, 64);
        DropOffText = API.createTextLabel("Press E to fill your truck", new Vector3(68.7, 127.5572, 78.11868), 15.0, 1.0);
        isPlayerOnJob = true;
        API.setWaypoint(75.89594, 110.0569);
        API.sendNotification("Go Post: You GPS has been set to the pickup dock");
}

function GoPostalStopJob() {
    API.showCursor(false); //stop showing the cursor
    API.destroyCefBrowser(myBrowser); //destroy the CEF browser
    API.setCanOpenChat(true); //allow the player to use the chat again.
    isCEFBrowserOpen = false;
    API.triggerServerEvent("StopJob");
    API.sendNotification("Go Post: You have canceled all your trips for today");
    API.removeWaypoint();
    parcelsintruck = 0;
    isPlayerOnJob = true;
}

function GoPostalSetWaypointDepot() {
    API.showCursor(false); //stop showing the cursor
    API.destroyCefBrowser(myBrowser); //destroy the CEF browser
    API.setCanOpenChat(true); //allow the player to use the chat again.
    isCEFBrowserOpen = false;
    API.setWaypoint(75.89594, 110.0569);
    API.sendNotification("Go Post: You GPS has been set to the depot");
}

function GoPostalWaypointToDropoff() {
    if (isPlayerOnJob == true)
    {
        API.showCursor(false); //stop showing the cursor
        API.destroyCefBrowser(myBrowser); //destroy the CEF browser
        API.setCanOpenChat(true); //allow the player to use the chat again.
        isCEFBrowserOpen = false;
        API.setWaypoint(waypointx, waypointy);
        API.sendNotification("Go Post: Your GPS has been set back to your dropoff location");
    }
    else API.sendChatMessage("You are not currently on a job");
}

//=====================================
//===== Money Transport Functions =====
//=====================================

function MoneyTransportStartJob() {
    if (isPlayerOnJob === false) {
        API.showCursor(false); //stop showing the cursor
        API.destroyCefBrowser(myBrowser); //destroy the CEF browser
        API.setCanOpenChat(true); //allow the player to use the chat again.
        isCEFBrowserOpen = false;
        DropOffMarker = API.createMarker(1, new Vector3(-142.888, -573.5522, 30.77533), new Vector3(), new Vector3(), new Vector3(20, 20, 3), 125, 198, 255, 64);
        DropOffText = API.createTextLabel("Press E to pickup packages", new Vector3(-142.888, -573.5522, 31.77533), 15.0, 1.0);
        isPlayerOnJob = true;
        API.setWaypoint(-142.888, -573.5522);
        API.sendNotification("Money Transport: You GPS has been set to the pickup vault");
    }
    else API.sendChatMessage("You already have a job");
}

function MoneyTransportReStartJob() {
    DropOffMarker = API.createMarker(1, new Vector3(-142.888, -573.5522, 30.77533), new Vector3(), new Vector3(), new Vector3(20, 20, 3), 125, 198, 255, 64);
    DropOffText = API.createTextLabel("Press E to pickup packages", new Vector3(-142.888, -573.5522, 31.77533), 15.0, 1.0);
    isPlayerOnJob = true;
    API.setWaypoint(-142.888, -573.5522);
    API.sendNotification("Money Transport: You GPS has been set to the pickup vault");
}

function MoneyTransportStopJob() {
    API.showCursor(false); //stop showing the cursor
    API.destroyCefBrowser(myBrowser); //destroy the CEF browser
    API.setCanOpenChat(true); //allow the player to use the chat again.
    isCEFBrowserOpen = false;
    API.triggerServerEvent("StopJob");
    API.sendNotification("Money Transport: You have canceled all your trips for today");
    API.removeWaypoint();
    parcelsintruck = 0;
    isPlayerOnJob = true;
}

function MoneyTransportSetWaypointDepot() {
    API.showCursor(false); //stop showing the cursor
    API.destroyCefBrowser(myBrowser); //destroy the CEF browser
    API.setCanOpenChat(true); //allow the player to use the chat again.
    isCEFBrowserOpen = false;
    API.setWaypoint(-142.888, -573.5522);
    API.sendNotification("Money Transport: You GPS has been set to the HQ");
}

function MoneyTransportWaypointToDropoff() {
    if (isPlayerOnJob == true) {
        API.showCursor(false); //stop showing the cursor
        API.destroyCefBrowser(myBrowser); //destroy the CEF browser
        API.setCanOpenChat(true); //allow the player to use the chat again.
        isCEFBrowserOpen = false;
        API.setWaypoint(waypointx, waypointy);
        API.sendNotification("Money Transport: Your GPS has been set back to your dropoff location");
    }
    else API.sendChatMessage("You are not currently on a job");
}