let menuPool = null;

API.onResourceStart.connect(function () {
    menuPool = API.getMenuPool();
});

API.onUpdate.connect(function () {
    if (menuPool !== null) {
        menuPool.ProcessMenus();
    }
});

API.onServerEventTrigger.connect(function (eventName, args) {

    if (eventName === "MENU_OPEN_ATM") {
        freeze = true;
        var cash = args[0];
        var bank = args[1];
        var amount = args[2];

        let menu = API.createMenu("ATM", 0, 0, 6);
        let banktotal = API.createMenuItem("Account Available: $" + bank, "");
        let atmlimit = API.createMenuItem("ATM Limit: $" + amount, "");
        let withdraw = API.createMenuItem("Withdraw", "");
        let deposit = API.createMenuItem("Deposit", "");
        let done = API.createMenuItem("Done", "");

        menu.AddItem(banktotal);
        menu.AddItem(atmlimit);
        menu.AddItem(withdraw);
        menu.AddItem(deposit);
        menu.AddItem(done);
        menuPool.Add(menu);

        menu.Visible = true;

        withdraw.Activated.connect(function (menu, item) {
            widthdrawamount = API.getUserInput("", 7);
            API.sendChatMessage("$" + widthdrawamount);
            API.triggerServerEvent("ATM_WITHDRAW", widthdrawamount);
            menu.Visible = false;
        });

        deposit.Activated.connect(function (menu, item) {
            depositamount = API.getUserInput("", 7);
            API.sendChatMessage("$" + depositamount);
            API.triggerServerEvent("ATM_DEPOSIT", depositamount);
            menu.Visible = false;
        });

        done.Activated.connect(function (menu, item) {
            menu.Visible = false;
            freeze = false;
        });
    }
    else if (eventName === "MENU_OPEN_BANK") {
        freeze = true;
        var cash = args[0];
        var bank = args[1];
        var amount = args[2];
        var salary = args[3];

        let menu = API.createMenu("Bank", 0, 0, 6);
        let banktotal = API.createMenuItem("Account Available: $" + bank, "");
        let salarytotal = API.createMenuItem("Get Salary: $" + salary, "");
        let transfer = API.createMenuItem("Transfer", "");
        let withdraw = API.createMenuItem("Withdraw", "");
        let deposit = API.createMenuItem("Deposit", "");
        let done = API.createMenuItem("Done", "");

        menu.AddItem(banktotal);
        menu.AddItem(salarytotal);
        menu.AddItem(transfer);
        menu.AddItem(withdraw);
        menu.AddItem(deposit);
        menu.AddItem(done);
        menuPool.Add(menu);

        menu.Visible = true;

        salarytotal.Activated.connect(function (menu, item) {
            API.triggerServerEvent("BANK_GET_SALARY", salary);
            menu.Visible = false;
        });

        transfer.Activated.connect(function (menu, item) {
            transferid = API.getUserInput("", 32);
            transferamount = API.getUserInput("", 8);
            API.triggerServerEvent("BANK_TRANSFER", transferid, transferamount);
            menu.Visible = false;
        });

        withdraw.Activated.connect(function (menu, item) {
            widthdrawamount = API.getUserInput("", 7);
            API.sendChatMessage("$" + widthdrawamount);
            API.triggerServerEvent("ATM_WITHDRAW", widthdrawamount);
            menu.Visible = false;
        });

        deposit.Activated.connect(function (menu, item) {
            depositamount = API.getUserInput("", 7);
            API.sendChatMessage("$" + depositamount);
            API.triggerServerEvent("ATM_DEPOSIT", depositamount);
            menu.Visible = false;
        });

        done.Activated.connect(function (menu, item) {
            menu.Visible = false;
            freeze = false;
        });
    }
});