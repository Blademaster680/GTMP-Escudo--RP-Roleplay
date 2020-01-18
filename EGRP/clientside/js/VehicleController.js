var blockedClasses = [13, 14, 15, 16, 21];

function isVehicleClassBlocked(vehicle) {
    return (blockedClasses.indexOf(API.getVehicleClass(API.getEntityModel(vehicle))) > -1);
}


API.onKeyDown.connect(function (sender, e) {
    if (e.KeyCode === Keys.Up) {
        if (API.isPlayerInAnyVehicle != false) {
            var veh = API.getPlayerVehicle;
            API.triggerServerEvent("EngineOn");
        }
    }
    else if (e.KeyCode === Keys.Down) {
        if (API.isPlayerInAnyVehicle != false) {
            var veh = API.getPlayerVehicle;
            API.triggerServerEvent("EngineOff");
        }
    }
    else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) {
        var localplayer = API.getLocalPlayer();
        if (API.isPlayerInAnyVehicle(localplayer) && API.getPlayerVehicleSeat(localplayer) == -1 && !isVehicleClassBlocked(API.getPlayerVehicle(localplayer))) API.triggerServerEvent("UpdateIndicator", ((e.KeyCode == Keys.Left) ? 1 : 0));
    }
});

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "IndicatorSubtitle") API.displaySubtitle(((args[0] == 1) ? "Left" : "Right") + " Indicator: " + ((args[1]) ? "~g~On" : "~r~Off"), 1000);
    else if (eventName == "Limiter")
    {

    }
});

API.onEntityStreamIn.connect(function (ent, entType) {
    if (entType == 1) {
        for (var i = 0; i < 2; i++) {
            var indicatorName = "Indicator_" + i;
            if (API.hasEntitySyncedData(ent, indicatorName)) API.callNative("SET_VEHICLE_INDICATOR_LIGHTS", ent, i, API.getEntitySyncedData(ent, indicatorName));
        }
    }
});

API.onPlayerEnterVehicle.connect(function (vehicle) {

});

API.onPlayerExitVehicle.connect(function (vehicle) {
    
});

API.onUpdate.connect(function () {
    
});