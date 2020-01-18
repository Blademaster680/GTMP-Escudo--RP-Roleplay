let menuPool = null;

API.onResourceStart.connect(function () {
    menuPool = API.getMenuPool();
});


API.onUpdate.connect(function () {
    if (menuPool !== null) {
        menuPool.ProcessMenus();
    }
});