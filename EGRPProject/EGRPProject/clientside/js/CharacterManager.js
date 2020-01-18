let menuPool = null;

var gender = 1;
var fShape = 0;
var mShape = 0;
var fSkin = 0;
var mSkin = 0;
var shapeSlider = 0.5;
var skinSlider = 0.5;
var eyes = 0;
var hair = 0;
var haircolor = 0;

API.onUpdate.connect(function () {
    if (menuPool !== null) {
        menuPool.ProcessMenus();
    }
});

API.onResourceStart.connect(function () {
    API.setPlayerSkin(1885233650);
    menuPool = API.getMenuPool();
});

API.onServerEventTrigger.connect(function (eventName, args) {

    if (eventName === "Menu_CreateCharacter") {

        var genderlist = new List(String);
        var mfacelist = new List(String);
        var ffacelist = new List(String);
        var faceshapelist = new List(String);
        var skinlist = new List(String);
        var eyelist = new List(String);
        var hairlist = new List(String);
        var haircolorlist = new List(String);

        genderlist.Add("Male");
        genderlist.Add("Female");

        for (var i = 1; i >= 20; i++)
        {
            mfacelist.Add(i);
        }

        for (i = 1; i >= 24; i++) {
            ffacelist.Add(i);
        }

        for (i = 0; i >= 10; i++) {
            faceshapelist.Add(i);
        }

        for (i = 0; i >= 10; i++) {
            skinlist.Add(i);
        }

        for (i = 0; i >= 8; i++) {
            eyelist.Add(i);
        }

        for (i = 0; i >= 36; i++) {
            hairlist.Add(i);
        }

        for (i = 1; i >= 65; i++) {
            haircolorlist.Add(i);
        }


        let menu = API.createMenu("Character Customization", 0, 0, 6);
        let item1 = API.createListItem("Gender", "", genderlist, 0);
        let item2 = API.createListItem("Mothers face", "", mfacelist, 0);
        let item3 = API.createListItem("Fathers face", "", ffacelist, 0);
        let item4 = API.createListItem("Resemblance shape", "", faceshapelist, 0);
        let item5 = API.createListItem("Resemblance skin", "", skinlist, 0);
        let item6 = API.createListItem("Eyes", "", eyelist, 0);
        let item7 = API.createListItem("Hair", "", hairlist, 0);
        let item8 = API.createListItem("Hair color", "", haircolorlist, 0);
        let done = API.createMenuItem("Done", "");

        menu.AddItem(item1);
        menu.AddItem(item2);
        menu.AddItem(item3);
        menu.AddItem(item4);
        menu.AddItem(item5);
        menu.AddItem(item6);
        menu.AddItem(item7);
        menu.AddItem(item8);
        menu.AddItem(done);
        menuPool.Add(menu);

        menu.Visible = true;

        menu.OnListChange.connect(function (sender, item, index) {
            var player = API.getLocalPlayer();
            if (item.Text === "Gender") {
                switch (index) {
                    case 0: // Male
                        API.setPlayerSkin(1885233650);
                        gender = 1;
                        break;
                    case 1: // Female
                        API.setPlayerSkin(-1667301416);
                        gender = 2;
                        break;
                }
            }
            else if (item.Text === "Mothers face") {
                fShape = index + 20;
                fSkin = index + 20;
                API.triggerServerEvent("UpdateCharacterVisualCharCreate", gender, fShape, mShape, fSkin, mSkin, shapeSlider, skinSlider, eyes, hair, haircolor);
            }
            else if (item.Text === "Fathers face") {
                mShape = index - 1;
                mSkin = index - 1;
                API.triggerServerEvent("UpdateCharacterVisualCharCreate", gender, fShape, mShape, fSkin, mSkin, shapeSlider, skinSlider, eyes, hair, haircolor);
            }
            else if (item.Text === "Resemblance shape") {
                if (index !== 10) {
                    index = index / 10;
                }
                shapeSlider = index;
                API.triggerServerEvent("UpdateCharacterVisualCharCreate", gender, fShape, mShape, fSkin, mSkin, shapeSlider, skinSlider, eyes, hair, haircolor);
            }
            else if (item.Text === "Resemblance skin") {
                if (index !== 10) {
                    index = index / 10;
                }
                skinSlider = index;
                API.triggerServerEvent("UpdateCharacterVisualCharCreate", gender, fShape, mShape, fSkin, mSkin, shapeSlider, skinSlider, eyes, hair, haircolor);
            }
            else if (item.Text === "Eyes") {
                eyes = index;
                API.triggerServerEvent("UpdateCharacterVisualCharCreate", gender, fShape, mShape, fSkin, mSkin, shapeSlider, skinSlider, eyes, hair, haircolor);
            }
            else if (item.Text === "Hair") {
                if (index >= 23 && gender === 1) {
                    index++;
                }
                else if (index >= 24 && gender === 2) {
                    index++;
                }
                hair = index;
                API.triggerServerEvent("UpdateCharacterVisualCharCreate", gender, fShape, mShape, fSkin, mSkin, shapeSlider, skinSlider, eyes, hair, haircolor);
            }
            else if (item.Text === "Hair color") {
                haircolor = index - 1;
                API.triggerServerEvent("UpdateCharacterVisualCharCreate", gender, fShape, mShape, fSkin, mSkin, shapeSlider, skinSlider, eyes, hair, haircolor);
            }
        });

        done.Activated.connect(function (menu, item) {
            var res = API.getScreenResolution(); //this gets the client's screen resoulution
            myBrowser = API.createCefBrowser(res.Width, res.Height); //we're initializing the browser here. This will be the full size of the user's screen.
            API.waitUntilCefBrowserInit(myBrowser); //this stops the script from getting ahead of itself, it essentially pauses until the browser is initialized
            API.setCefBrowserPosition(myBrowser, 0, 0); //The orientation (top left) corner in relation to the user's screen.  This is useful if you do not want a full page browser.  0,0 is will lock the top left corner of the browser to the top left of the screen.
            API.loadPageCefBrowser(myBrowser, "clientside/web/charactername.html"); //This loads the HTML file of your choice.      .    API.setCefBrowserHeadless(myBrowser, true); //this will remove the scroll bars from the bottom/right side
            API.showCursor(true); //This will show the mouse cursor
            API.setCanOpenChat(false);  //This disables the chat, so the user can type in a form without opening the chat and causing issues.
            menu.Visible = false;
        });

        menu.OnMenuClose.connect(function (sender) {
            API.triggerServerEvent("REOPEN_CHARACTERSELECTION");
        });
    }
    else if (eventName === "CreateCharacter") {

        var res = API.getScreenResolution(); //this gets the client's screen resoulution
        myBrowser = API.createCefBrowser(res.Width, res.Height); //we're initializing the browser here. This will be the full size of the user's screen.
        API.waitUntilCefBrowserInit(myBrowser); //this stops the script from getting ahead of itself, it essentially pauses until the browser is initialized
        API.setCefBrowserPosition(myBrowser, 0, 0); //The orientation (top left) corner in relation to the user's screen.  This is useful if you do not want a full page browser.  0,0 is will lock the top left corner of the browser to the top left of the screen.
        API.loadPageCefBrowser(myBrowser, "clientside/web/charactername.html"); //This loads the HTML file of your choice.      .    API.setCefBrowserHeadless(myBrowser, true); //this will remove the scroll bars from the bottom/right side
        API.showCursor(true); //This will show the mouse cursor
        API.setCanOpenChat(false);  //This disables the chat, so the user can type in a form without opening the chat and causing issues.
    }
    else if (eventName === "Menu_CharacterSelection") {
        let menu = API.createMenu("Character Selection", 0, 0, 6);

        let character1 = API.createMenuItem("" + args[3].toString() + " " + args[4].toString(), "");
        let character2 = API.createMenuItem("" + args[9].toString() + " " + args[10].toString(), "");
        let character3 = API.createMenuItem("" + args[15].toString() + " " + args[16].toString(), "");
        let character4 = API.createMenuItem("" + args[21].toString() + " " + args[22].toString(), "");

        menu.AddItem(character1);
        menu.AddItem(character2);
        menu.AddItem(character3);
        menu.AddItem(character4);

        let createcharacter = API.createMenuItem("Create new character", "");

        menu.AddItem(createcharacter);
        menuPool.Add(menu);

        menu.Visible = true;

        character1.Activated.connect(function (menu, item) {
            if (character1.Text === "Empty Slot")
                API.sendChatMessage("You cannot choose an empty slot");
            else {
                menu.Visible = false;
                API.triggerServerEvent("Load_Character", args[1].toString(), args[5]);
            }
        });
        character2.Activated.connect(function (menu, item) {
            if (character2.Text === "Empty Slot")
                API.sendChatMessage("You cannot choose an empty slot");
            else {
                menu.Visible = false;
                API.triggerServerEvent("Load_Character", args[7].toString(), args[11]);
            }
        });
        character3.Activated.connect(function (menu, item) {
            if (character3.Text === "Empty Slot")
                API.sendChatMessage("You cannot choose an empty slot");
            else {
                menu.Visible = false;
                API.triggerServerEvent("Load_Character", args[13].toString(), args[17]);
            }
        });
        character4.Activated.connect(function (menu, item) {
            if (character4.Text === "Empty Slot")
                API.sendChatMessage("You cannot choose an empty slot");
            else {
                menu.Visible = false;
                API.triggerServerEvent("Load_Character", args[19].toString(), args[23]);
            }
        });

        createcharacter.Activated.connect(function (menu, item) {
            if (args[0] === 4) {
                API.sendChatMessage("You already have 4 characters");
            }
            else {
                menu.Visible = false;
                API.triggerServerEvent("Move_CreateCharacter");
            }
        });

        menu.OnMenuClose.connect(function (sender) {
            API.triggerServerEvent("REOPEN_CHARACTERSELECTION"); 
        });
    }
    else if (eventName === "Menu_AccessoryStore") {
        var player = API.getLocalPlayer();
        var hat = API.getEntitySyncedData(player, "GTAO_HAT");
        var glass = API.getEntitySyncedData(player, "GTAO_GLASSES");
        var ear = API.getEntitySyncedData(player, "GTAO_EARS");

        var hatbuy = 0;
        var glassesbuy = 0;
        var earsbuy = 0;

        var hatlist = new List(String);
        var glasseslist = new List(String);
        var earslist = new List(String);

        let menu = API.createMenu("Hat Store", 0, 0, 6);

        hatlist.Add("Off");
        for (i = 1; i <= 103; i++) {
            hatlist.Add("" + i + "");
        } 
        glasseslist.Add("Off");
        for (i = 1; i <= 28; i++) {
            glasseslist.Add("" + i + "");
        } 
        earslist.Add("Off");
        for (i = 1; i <= 36; i++) {
            earslist.Add("" + i + "");
        } 

        let hats = API.createListItem("Hats", "", hatlist, 0); 
        let glasses = API.createListItem("Glasses", "", glasseslist, 0); 
        let ears = API.createListItem("Ears", "", earslist, 0); 
        
        let buy = API.createMenuItem("Buy: $1000", "");

        menu.AddItem(hats);
        menu.AddItem(glasses);
        menu.AddItem(ears);
        menu.AddItem(buy);
        menuPool.Add(menu);

        menu.Visible = true;

        menu.OnListChange.connect(function (sender, item, index) {
            if (item.Text === "Hats") {
                if (index.Text !== "Off") {
                    API.setPlayerAccessory(player, 0, index, 0);
                    hatbuy = index;
                }
                else {
                    API.setPlayerAccessory(player, 0, 8, 0);
                    hatbuy = index;
                }
                
            }
            else if (item.Text === "Glasses") {
                if (index.Text !== "Off") {
                    API.setPlayerAccessory(player, 1, index, 0);
                    glassesbuy = index;
                }
                else {
                    API.setPlayerAccessory(player, 1, 0, 0);
                    glassesbuy = index;
                }
                
            }
            else if (item.Text === "Ears") {
                if (index.Text !== "Off") {
                    API.setPlayerAccessory(player, 2, index, 0);
                    earsbuy = index;
                }
                else {
                    API.setPlayerAccessory(player, 2, 33, 0);
                    earsbuy = index;
                }
                
            }
        });

        buy.Activated.connect(function (menu, item) {
            menu.Visible = false;

            API.triggerServerEvent("STORE_CHARACTERBUY", hatbuy, glassesbuy, earsbuy);
        });
    }
    else if (eventName === "Menu_TopsStore") {
        //var player = API.getLocalPlayer();
        var top = API.getEntitySyncedData(player, "GTAO_TOP");
        var undertop = API.getEntitySyncedData(player, "GTAO_UNDERTOP");

        var topBuy = 0;
        var undertopBuy = 0;

        var toplist = new List(String);
        var undertoplist = new List(String);

        let menu = API.createMenu("Top Store", 0, 0, 6);

        toplist.Add("Off");
        for (i = 1; i <= 236; i++) {
            toplist.Add("" + i + "");
        }
        undertoplist.Add("Off");
        for (i = 1; i <= 118; i++) {
            undertoplist.Add("" + i + "");
        }

        let tops = API.createListItem("Tops", "", toplist, 0);
        let undertops = API.createListItem("Under Shirts", "", undertoplist, 0);
        let buy = API.createMenuItem("Buy: $1000", "");

        menu.AddItem(tops);
        menu.AddItem(undertops);
        menu.AddItem(buy);
        menuPool.Add(menu);

        menu.Visible = true;

        menu.OnListChange.connect(function (sender, item, index) {
            if (item.Text === "Tops") {
                if (index.Text !== "Off") {
                    API.setPlayerClothes(player, 11, index, 0);
                }
                else {
                    API.setPlayerClothes(player, 0);
                }
                topBuy = index;
            }
            else if (item.Text === "Under Shirts") {
                if (index.Text !== "Off") {
                    API.setPlayerClothes(player, 8, index, 0);
                }
                else {
                    API.clearPlayerClothes(player, 8);
                }
                undertopBuy = index;
            }
        });

        buy.Activated.connect(function (menu, item) {
            menu.Visible = false;

            API.triggerServerEvent("STORE_CHARACTERBUY", topBuy, undertopBuy);
        });
    }
    else if (eventName === "Menu_PantsStore") {
        //var player = API.getLocalPlayer();
        var pant = API.getEntitySyncedData(player, "GTAO_PANTS");

        var pantsBuy = 0;

        var pantslist = new List(String);

        let menu = API.createMenu("Pants Store", 0, 0, 6);

        for (i = 1; i <= 89; i++) {
            pantslist.Add("" + i + "");
        }

        let pants = API.createListItem("Pants", "", pantslist, 0);
        let buy = API.createMenuItem("Buy: $1000", "");

        menu.AddItem(pants);
        menu.AddItem(buy);
        menuPool.Add(menu);

        menu.Visible = true;

        menu.OnListChange.connect(function (sender, item, index) {
            if (item.Text === "Pants") {
                if (index.Text !== "Off") {
                    API.setPlayerClothes(player, 4, index, 0);
                }
                pantsBuy = index;
            }
        });

        buy.Activated.connect(function (menu, item) {
            menu.Visible = false;

            if (pantsBuy !== 0) {

                API.triggerServerEvent("STORE_CHARACTERBUY", pantsBuy);
            }
            else API.setPlayerClothes(player, 4, pant, 0);
            
        });
    }
    else if (eventName === "Menu_ShoesStore") {
       // var player = API.getLocalPlayer();
        var shoe = API.getEntitySyncedData(player, "GTAO_SHOES");

        var shoesBuy = 0;

        var shoeslist = new List(String);

        let menu = API.createMenu("Shoes Store", 0, 0, 6);

        for (i = 1; i <= 62; i++) {
            shoeslist.Add("" + i + "");
        }

        let shoes = API.createListItem("Shoes", "", shoeslist, 0);
        let buy = API.createMenuItem("Buy: $1000", "");

        menu.AddItem(shoes);
        menu.AddItem(buy);
        menuPool.Add(menu);

        menu.Visible = true;

        menu.OnListChange.connect(function (sender, item, index) {
            if (item.Text === "Shoes") {
                if (index.Text !== "Off") {
                    API.setPlayerClothes(player, 6, index, 0);
                }
                shoesBuy = index;
            }
        });

        buy.Activated.connect(function (menu, item) {
            menu.Visible = false;
            if (shoesBuy !== 0) {

                API.triggerServerEvent("STORE_CHARACTERBUY", shoesBuy);
            }
            else API.setPlayerClothes(player, 6, shoe, 0);
            
        });
    }
    else if (eventName === "Menu_BarberStore") {
       // var player = API.getLocalPlayer();
        var hair = API.getEntitySyncedData(player, "GTAO_HAIR_STYLE");
        var haircolor = API.getEntitySyncedData(player, "GTAO_HAIR_COLOR");

       // var hairlist = new List(String);
       // var haircolorlist = new List(String);

        var hairBuy = 0;
        var haircolorBuy = 0;

        let menu = API.createMenu("Barber Store", 0, 0, 6);

        for (i = 1; i <= 39; i++) {
            hairlist.Add("" + i + "");
        }
        for (i = 1; i <= 64; i++) {
            haircolorlist.Add("" + i + "");
        }

        let hairs = API.createListItem("Hair Style", "", hairlist, 0);
        let haircolors = API.createListItem("Hair Color", "", haircolorlist, 0);
        let buy = API.createMenuItem("Buy: $1000", "");

        menu.AddItem(hairs);
        menu.AddItem(haircolors);
        menu.AddItem(buy);
        menuPool.Add(menu);

        menu.Visible = true;

        menu.OnListChange.connect(function (sender, item, index) {
            if (item.Text === "Hair Style") {
                if (index.Text !== "Off") {
                    API.setPlayerClothes(player, 2, index, 0);
                }
                hairBuy = index;
            }
            else if (item.Text === "Hair Color") {
                if (index.Text !== "Off") {
                    API.callNative("_SET_PED_HAIR_COLOR", player, index, 0);
                }
                haircolorBuy = index;
            }
        });

        buy.Activated.connect(function (menu, item) {
            menu.Visible = false;

            if (hairBuy !== 0 || haircolorBuy !== 0) {
                API.triggerServerEvent("STORE_CHARACTERBUY", hairBuy, haircolorBuy);
            }
            else {
                API.setPlayerClothes(player, 2, hair, haircolor);
            }
        });
    }
    else if (eventName === "Menu_StoreClose") {

        Visible = false
    }
    else if (eventName === "CEFDestroy") {

        myBrowser.destroy();
    }
});

function createchar(Fname, Lname) {
    //API.sendChatMessage("Your username is " + Username + " and your password is " + Password); //send a chat message with the data they entered.
    API.showCursor(false); //stop showing the cursor
    API.destroyCefBrowser(myBrowser); //destroy the CEF browser
    API.setCanOpenChat(true); //allow the player to use the chat again.
    API.triggerServerEvent("CreateCharacter", Fname, Lname);
}

API.onEntityStreamIn.connect(function (entity, entityType) {
    if (entityType === 6 || entityType === 8) {// Player or ped
        setPedCharacter(entity);
    }
});

function setPedCharacter(ent) {
    if (API.isPed(ent) &&
        API.getEntitySyncedData(ent, "GTAO_HAS_CHARACTER_DATA") === true &&
        (API.getEntityModel(ent) === 1885233650 || // FreemodeMale
            API.getEntityModel(ent) === -1667301416)) // FreemodeFemale
    {
        // FACE
        var shapeFirstId = API.getEntitySyncedData(ent, "GTAO_SHAPE_FIRST_ID");
        var shapeSecondId = API.getEntitySyncedData(ent, "GTAO_SHAPE_SECOND_ID");

        var skinFirstId = API.getEntitySyncedData(ent, "GTAO_SKIN_FIRST_ID");
        var skinSecondId = API.getEntitySyncedData(ent, "GTAO_SKIN_SECOND_ID");

        var shapeMix = API.f(API.getEntitySyncedData(ent, "GTAO_SHAPE_MIX"));
        var skinMix = API.f(API.getEntitySyncedData(ent, "GTAO_SKIN_MIX"));

        API.callNative("SET_PED_HEAD_BLEND_DATA", ent, shapeFirstId, shapeSecondId, 0, skinFirstId, skinSecondId, 0, shapeMix, skinMix, 0, false);

        // HAIR 
        var hair = API.getEntitySyncedData(ent, "GTAO_HAIR_STYLE");
        var hairColor = API.getEntitySyncedData(ent, "GTAO_HAIR_COLOR");
        var highlightColor = API.getEntitySyncedData(ent, "GTAO_HAIR_HIGHLIGHT_COLOR");

        API.setPlayerClothes(ent, 2, hair, 0);
        API.callNative("_SET_PED_HAIR_COLOR", ent, hairColor, highlightColor);

        // EYE COLOR

        var eyeColor = API.getEntitySyncedData(ent, "GTAO_EYE_COLOR");

        API.callNative("_SET_PED_EYE_COLOR", ent, eyeColor);

        // EYEBROWS, MAKEUP, LIPSTICK
        var eyebrowsStyle = API.getEntitySyncedData(ent, "GTAO_EYEBROWS");
        var eyebrowsColor = API.getEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR");
        var eyebrowsColor2 = API.getEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR2");

        API.callNative("SET_PED_HEAD_OVERLAY", ent, 2, eyebrowsStyle, API.f(1));

        API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", ent, 2, 1, eyebrowsColor, eyebrowsColor2);

        if (API.hasEntitySyncedData(ent, "GTAO_LIPSTICK")) {
            var lipstick = API.getEntitySyncedData(ent, "GTAO_LIPSTICK");
            var lipstickColor = API.getEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR");
            var lipstickColor2 = API.getEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR2");

            API.callNative("SET_PED_HEAD_OVERLAY", ent, 8, lipstick, API.f(1));
            API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", ent, 8, 2, lipstickColor, lipstickColor2);
        }

        if (API.hasEntitySyncedData(ent, "GTAO_MAKEUP")) {
            var makeup = API.getEntitySyncedData(ent, "GTAO_MAKEUP");
            var makeupColor = API.getEntitySyncedData(ent, "GTAO_MAKEUP_COLOR");
            var makeupColor2 = API.getEntitySyncedData(ent, "GTAO_MAKEUP_COLOR2");

            API.callNative("SET_PED_HEAD_OVERLAY", ent, 4, makeup, API.f(1));
            API.callNative("SET_PED_HEAD_OVERLAY", ent, 8, lipstick, API.f(1));
            API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", ent, 4, 0, makeupColor, makeupColor2);
        }

        // FACE FEATURES (e.g. nose length, chin shape, etc)

        var faceFeatureList = API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_LIST");

        for (var i = 0; i < 21; i++) {
            API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.f(faceFeatureList[i]));
        }

        // Accessories
        var hat = API.getEntitySyncedData(ent, "GTAO_HAT");
        var glass = API.getEntitySyncedData(ent, "GTAO_GLASSES");
        var ear = API.getEntitySyncedData(ent, "GTAO_EARS");
        API.setPlayerAccessory(ent, 0, hat, 0);
        API.setPlayerAccessory(ent, 1, glass, 0);
        API.setPlayerAccessory(ent, 2, ear, 0);

        // Tops
        var top = API.getEntitySyncedData(ent, "GTAO_TOP");
        API.setPlayerClothes(ent, 11, top, 0);
        var undertop = API.getEntitySyncedData(ent, "GTAO_UNDERTOP");
        API.setPlayerClothes(ent, 8, undertop, 0);

        // Pants
        var pants = API.getEntitySyncedData(ent, "GTAO_PANTS");
        API.setPlayerClothes(ent, 4, pants, 0);

        // Shoes
        var shoes = API.getEntitySyncedData(ent, "GTAO_SHOES");
        API.setPlayerClothes(ent, 6, shoes, 0);

    }
}

function getPositionInfrontOfPlayer(distance) {
    var pos = API.getEntityPosition(API.getLocalPlayer());
    var a = API.getEntityRotation(API.getLocalPlayer()).Z;

    var rad = a * Math.PI / 180;

    var newpos = new Vector3(pos.X + (distance * Math.sin(-rad)),
        pos.Y + (distance * Math.cos(-rad)),
        pos.Z);
    return newpos;
}