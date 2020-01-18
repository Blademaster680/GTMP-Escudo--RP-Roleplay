API.onKeyDown.connect(function (sender, args) {

    if (args.KeyCode === Keys.Y) {
        API.triggerServerEvent("IsPlayerInShop");
        API.triggerServerEvent("IsPlayerInBank");
        API.triggerServerEvent("IsPlayerInJobVehicleSpawn");
        API.triggerServerEvent("IsPlayerInJobVehicle");
    }
    else if (args.KeyCode === Keys.E) {
        API.triggerServerEvent("JobFirstLoadUp");
        API.triggerServerEvent("IsPlayerCloseToJobVehicle");
        API.triggerServerEvent("IsPlayerAtJobDropOff");
    }
});