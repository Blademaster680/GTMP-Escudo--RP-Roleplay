API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "SOUND_ATM") {
        API.startMusic("clientside/sounds/ATM.mp3", false);
    }
});