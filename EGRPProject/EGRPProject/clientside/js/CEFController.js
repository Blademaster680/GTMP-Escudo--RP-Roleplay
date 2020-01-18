var myBrowser = null;

/*API.onResourceStart.connect(function () {

    var res = API.getScreenResolution(); //this gets the client's screen resoulution
    myBrowser = API.createCefBrowser(res.Width, res.Height); //we're initializing the browser here. This will be the full size of the user's screen.
    API.waitUntilCefBrowserInit(myBrowser); //this stops the script from getting ahead of itself, it essentially pauses until the browser is initialized
    API.setCefBrowserPosition(myBrowser, 0, 0); //The orientation (top left) corner in relation to the user's screen.  This is useful if you do not want a full page browser.  0,0 is will lock the top left corner of the browser to the top left of the screen.
    API.loadPageCefBrowser(myBrowser, "clientside/web/login.html"); //This loads the HTML file of your choice.      .    API.setCefBrowserHeadless(myBrowser, true); //this will remove the scroll bars from the bottom/right side
    API.showCursor(true); //This will show the mouse cursor
    API.setCanOpenChat(false);  //This disables the chat, so the user can type in a form without opening the chat and causing issues.
}); */

API.onServerEventTrigger.connect(function (eventName, args) {

   if (eventName === "CEFDestroy") {

       myBrowser.destroy();
   }
   else if (eventName === "AccountLogin") {

       var res = API.getScreenResolution(); //this gets the client's screen resoulution
       myBrowser = API.createCefBrowser(res.Width, res.Height); //we're initializing the browser here. This will be the full size of the user's screen.
       API.waitUntilCefBrowserInit(myBrowser); //this stops the script from getting ahead of itself, it essentially pauses until the browser is initialized
       API.setCefBrowserPosition(myBrowser, 0, 0); //The orientation (top left) corner in relation to the user's screen.  This is useful if you do not want a full page browser.  0,0 is will lock the top left corner of the browser to the top left of the screen.
      // API.setCefFramerate(myBrowser, 30);
       API.loadPageCefBrowser(myBrowser, "clientside/web/login.html"); //This loads the HTML file of your choice.      .    API.setCefBrowserHeadless(myBrowser, true); //this will remove the scroll bars from the bottom/right side
       API.showCursor(true); //This will show the mouse cursor
       API.setCanOpenChat(false);  //This disables the chat, so the user can type in a form without opening the chat and causing issues.
   }
   else if (eventName === "AccountRegister") {
       res = API.getScreenResolution(); //this gets the client's screen resoulution
       myBrowser = API.createCefBrowser(res.Width, res.Height); //we're initializing the browser here. This will be the full size of the user's screen.
       API.waitUntilCefBrowserInit(myBrowser); //this stops the script from getting ahead of itself, it essentially pauses until the browser is initialized
       API.setCefBrowserPosition(myBrowser, 0, 0); //The orientation (top left) corner in relation to the user's screen.  This is useful if you do not want a full page browser.  0,0 is will lock the top left corner of the browser to the top left of the screen.
      // API.setCefFramerate(myBrowser, 30);
       API.loadPageCefBrowser(myBrowser, "clientside/web/register.html"); //This loads the HTML file of your choice.      .    API.setCefBrowserHeadless(myBrowser, true); //this will remove the scroll bars from the bottom/right side
       API.showCursor(true); //This will show the mouse cursor
       API.setCanOpenChat(false);  //This disables the chat, so the user can type in a form without opening the chat and causing issues.
   }
});

function loginform() {
    API.showCursor(false); //stop showing the cursor
    API.destroyCefBrowser(myBrowser); //destroy the CEF browser
    API.setCanOpenChat(true); //allow the player to use the chat again.
    API.triggerServerEvent("OpenLoginForm");
}

function login(Username, Password) {
    //API.sendChatMessage("Your username is " + Username + " and your password is " + Password); //send a chat message with the data they entered.
    API.showCursor(false); //stop showing the cursor
    API.destroyCefBrowser(myBrowser); //destroy the CEF browser
    API.setCanOpenChat(true); //allow the player to use the chat again.
    API.triggerServerEvent("AccountLogin", Username, Password);
}

function registerform() {
    API.showCursor(false); //stop showing the cursor
    API.destroyCefBrowser(myBrowser); //destroy the CEF browser
    API.setCanOpenChat(true); //allow the player to use the chat again.
    API.triggerServerEvent("OpenRegisterForm");
}

function register(Username, Email, Password) {
    //API.sendChatMessage("Your username is " + Username + " and your password is " + Password + "And your email is " + Email + " "); //send a chat message with the data they entered.
    API.showCursor(false); //stop showing the cursor
    API.destroyCefBrowser(myBrowser); //destroy the CEF browser
    API.setCanOpenChat(true); //allow the player to use the chat again.
    API.triggerServerEvent("AccountRegister", Username, Email, Password);
}