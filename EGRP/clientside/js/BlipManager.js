API.onResourceStart.connect(function () {
});

API.onServerEventTrigger.connect(function (eventName, args) {

    if (eventName === "Shop_Register") {
        API.createMarker(1, new Vector3(args[0], args[1], args[2]), new Vector3(), new Vector3(), new Vector3(1, 1, 1), 198, 255, 64, 125);
    }
});