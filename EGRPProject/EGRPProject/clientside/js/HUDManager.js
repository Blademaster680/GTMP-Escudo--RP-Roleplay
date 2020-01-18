var drawVehicleHUD = false;
var drawAnimationHUD = false;
var currentMoney = null;
var currentParcel = null;
var parcelHUD = null;
var jobname = null;
var jobvehicle = null;
var playeronjob = null;
var res = API.getScreenResolutionMaintainRatio();

API.onUpdate.connect(function (sender, args) {
    var player = API.getLocalPlayer();
    if (currentMoney !== null) {
        API.drawText("$" + currentMoney, res.Width - 15, 50, 1, 115, 186, 131, 255, 4, 2, false, true, 0);
    }
    if (currentParcel !== null && playerjob >= 1 && jobname == "Go Postal" && API.isPlayerInAnyVehicle(player) === true) {
        API.drawText("Parcels in vehicle: " + currentParcel.replace(/\.00$/, ''), res.Width - 1550, res.Height - 540, 1, 115, 186, 131, 255, 4, 2, false, true, 0);
    }
    if (currentParcel !== null && playerjob >= 1 && jobname == "Money Transport" && API.isPlayerInAnyVehicle(player) === true) {
        API.drawText("Money in vehicle: $" + currentParcel.replace(/\.00$/, ''), res.Width - 1550, res.Height - 540, 0.8, 115, 186, 131, 255, 4, 2, false, true, 0);
    }
});

API.onServerEventTrigger.connect(function (eventName, args) {

    if (eventName === "HUD_UPDATE_MONEY_DISPLAY") {
        currentMoney = Number(args[0]).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
    }
    else if (eventName === "HUD_UPDATE_GOPOSTAL_PARCEL") {
        currentParcel = Number(args[0]).toFixed(2).replace(/(\d)(?=(\d{3})+\.0)/g, '1');
        playerjob = args[0];
        jobvehicle = args[1];
        jobname = args[2].toString();
    }
    else if (eventName === "HUD_UPDATE_MONEYTRANSPORT_CAPACITY") {
        currentParcel = Number(args[0]).toFixed(2).replace(/(\d)(?=(\d{3})+\.0)/g, '0');
        playerjob = args[0];
        jobvehicle = args[1];
        jobname = args[2].toString();;
    }
});

API.onPlayerEnterVehicle.connect(function (veh) {
});

API.onPlayerExitVehicle.connect(function (veh) {

});