using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using EGRPProject.Database;
using GrandTheftMultiplayer.Server.Constant;
using EGRPProject.Data;
using EGRPProject.Jobs;
using System.Threading.Tasks;

namespace EGRPProject
{
    public class Main : Script
    {
        private static List<Player> players = new List<Player>();

        internal static List<Player> Players { get => players; set => players = value; }

        private static List<Character> characters = new List<Character>();

        internal static List<Character> Characters { get => characters; set => characters = value; }

        public const ulong SET_STATE_OF_CLOSEST_DOOR_OF_TYPE = 0xF82D8F1926A02C3D;

        public Main()
        {
            API.onResourceStart += OnServerStart;
            API.onResourceStop += OnServerShutdown;
            API.onPlayerConnected += OnPlayerConnect;
            API.onPlayerFinishedDownload += OnPlayerFinishDownload;
            API.onPlayerDisconnected += OnPlayerDisconnect;
            API.onPlayerExitVehicle += OnPlayerExitVehicleHandler;
            API.onClientEventTrigger += onClientEventTrigger;
            API.onChatMessage += onSentMessage;
            API.onUpdate += OnUpdateHandler;
        }

        private void OnServerStart()
        {
            MySQL.API = API;
            DoorManager.LoadDoors();
            ShopManager.LoadShops();
            ATMManager.LoadATMS();
            JobManager.LoadJobs();
            Account.autoSavePlayers();
        }

        private void OnServerShutdown()
        {
            var players = API.getAllPlayers();
            foreach (var player in players)
            {
                int id = Convert.ToInt32(player.getData("CharID"));
                Account.SavePlayer(player);
                Account.SaveCharacter(player, id);
            }
            API.consoleOutput("All players have been saved.");
        }

        private void OnPlayerConnect(Client player)
        {

            player.setData("CharJobVehicle", null);
            player.setData("notInJobVehicle", 0);
            player.setData("LoggedIn", false);
            player.setData("HoldingBox", false);
            player.setData("JobParcels", 0);
            player.setData("JobCapacity", 0);
        }

        private void OnPlayerFinishDownload(Client player)
        {
            API.shared.triggerClientEvent(player, "AccountLogin");
        }

        public void OnUpdateHandler()
        {
            var players = API.getAllPlayers();
            foreach (int id in DoorManager.doors)
            {
                API.shared.exported.doormanager.refreshDoorState(id);
            }
        }

        private void OnPlayerDisconnect(Client player, string reason)
        {
            int id = Convert.ToInt32(player.getData("CharID"));
            Account.SavePlayer(player);
            Account.SaveCharacter(player, id);
        }

        public async void onClientEventTrigger(Client player, string eventName, params object[] args)
        {
            if (eventName == "AccountLogin")
            {
                Account.LoginPlayer(player, args[0].ToString(), Main.GetSha256FromString(args[1].ToString()));
            }
            else if (eventName == "AccountRegister")
            {
                Account.RegisterPlayer(player, args[0].ToString(), args[1].ToString(), Main.GetSha256FromString(args[2].ToString()));
            }
            else if (eventName == "OpenRegisterForm")
            {
                API.shared.triggerClientEvent(player, "AccountRegister");
            }
            else if (eventName == "OpenLoginForm")
            {
                API.shared.triggerClientEvent(player, "AccountLogin");
            }
            else if (eventName == "Move_CreateCharacter")
            {
                API.shared.triggerClientEvent(player, "Menu_CreateCharacter");
            }
            else if (eventName == "CreateCharacter")
            {
                Account.CreateCharacter(player, args[0].ToString(), args[1].ToString());
            }
            else if (eventName == "Load_Character")
            {
                Account.LoadCharacter(player, Convert.ToInt32(args[0]), Convert.ToInt32(args[1]));
            }
            else if(eventName == "REOPEN_CHARACTERSELECTION")
            {
                string username = player.getData("AccountUsername");
                string password = player.getData("AccountPassword");
                Account.LoginPlayer(player, username, password);
            }
            else if (eventName == "UpdateCharacterVisualCharCreate")
            {
                API.sendNativeToAllPlayers(0x9414E18B9434C2FE, player, args[1], args[2], 0, args[3], args[4], 0, args[5], args[6], 0.0, true);
                API.sendNativeToAllPlayers(Hash._SET_PED_EYE_COLOR, player.handle, args[7]);
                API.sendNativeToAllPlayers(Hash.SET_PED_COMPONENT_VARIATION, player.handle, 2, args[8], 0, 0);
                API.sendNativeToAllPlayers(Hash._SET_PED_HAIR_COLOR, player.handle, args[9]);
                player.setData("CharGender", args[0]);
                player.setData("CharfFace", args[1]);
                player.setData("CharmFace", args[2]);
                player.setData("CharfSkin", args[4]);
                player.setData("CharmSkin", args[3]);
                player.setData("CharresShape", args[5]);
                player.setData("CharresSkin", args[6]);
                player.setData("CharEyes", args[7]);
                player.setData("CharHair", args[8]);
                player.setData("CharHairColor", args[9]);
            }
            else if (eventName == "IsPlayerInShop")
            {
                if(player.getData("IN_SHOP_TYPE") != null)
                {
                    var type = player.getData("IN_SHOP_TYPE");

                    if(type == "Accessory")
                    {
                        API.shared.triggerClientEvent(player, "Menu_AccessoryStore");
                    }
                    else if (type == "Tops")
                    {
                        API.shared.triggerClientEvent(player, "Menu_TopsStore");
                    }
                    else if (type == "Pants")
                    {
                        API.shared.triggerClientEvent(player, "Menu_PantsStore");
                    }
                    else if (type == "Shoes")
                    {
                        API.shared.triggerClientEvent(player, "Menu_ShoesStore");
                    }
                    else if (type == "Barber")
                    {
                        API.shared.triggerClientEvent(player, "Menu_BarberStore");
                    }
                }
            }
            else if (eventName == "STORE_CHARACTERBUY")
            {
                if (player.getData("IN_SHOP_TYPE") != null)
                {
                    var type = player.getData("IN_SHOP_TYPE");
                    int cash = player.getData("CharCash");
                    int bank = player.getData("CharBank");

                    if (type == "Accessory")
                    {
                        if (args[0].ToString() != "0")
                        {
                            if (cash >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_HAT", args[0]);
                                Account.UpdateMoney(player, "Cash", cash -= 1000);
                                API.setPlayerAccessory(player, 0, Convert.ToInt32(args[0]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "takes out $1000 and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else if (bank >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_HAT", args[0]);
                                Account.UpdateMoney(player, "Bank", bank -= 1000);
                                API.setPlayerAccessory(player, 0, Convert.ToInt32(args[0]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "swipes their card and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else API.sendChatMessageToPlayer(player, "You do not have enough money");
                        }
                        else if(args[1].ToString() != "0")
                        {
                            if (cash >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_GLASSES", args[1]);
                                Account.UpdateMoney(player, "Cash", cash -= 1000);
                                API.setPlayerAccessory(player, 0, Convert.ToInt32(args[1]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "takes out $1000 and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else if (bank >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_GLASSES", args[1]);
                                Account.UpdateMoney(player, "Bank", bank -= 1000);
                                API.setPlayerAccessory(player, 0, Convert.ToInt32(args[1]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "swipes their card and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else API.sendChatMessageToPlayer(player, "You do not have enough money");
                        }
                        else if (args[2].ToString() != "0")
                        {
                            if (cash >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_EARS", args[2]);
                                Account.UpdateMoney(player, "Cash", cash -= 1000);
                                API.setPlayerAccessory(player, 0, Convert.ToInt32(args[2]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "takes out $1000 and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else if (bank >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_EARS", args[2]);
                                Account.UpdateMoney(player, "Bank", bank -= 1000);
                                API.setPlayerAccessory(player, 0, Convert.ToInt32(args[2]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "swipes their card and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else API.sendChatMessageToPlayer(player, "You do not have enough money");
                        }
                    }
                    else if (type == "Tops")
                    {
                        if (args[0].ToString() != "0")
                        {
                            if (cash >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_TOP", args[0]);
                                Account.UpdateMoney(player, "Cash", cash -= 1000);
                                API.setPlayerClothes(player, 11, Convert.ToInt32(args[0]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "takes out $1000 and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else if (bank >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_TOP", args[0]);
                                Account.UpdateMoney(player, "Bank", bank -= 1000);
                                API.setPlayerClothes(player, 11, Convert.ToInt32(args[0]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "swipes their card and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else API.sendChatMessageToPlayer(player, "You do not have enough money");
                        }
                        else if (args[1].ToString() != "0")
                        {
                            if (cash >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_UNDERTOP", args[1]);
                                Account.UpdateMoney(player, "Cash", cash -= 1000);
                                API.setPlayerClothes(player, 8, Convert.ToInt32(args[0]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "takes out $1000 and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else if (bank >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_UNDERTOP", args[1]);
                                Account.UpdateMoney(player, "Bank", bank -= 1000);
                                API.setPlayerClothes(player, 8, Convert.ToInt32(args[0]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "swipes their card and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else API.sendChatMessageToPlayer(player, "You do not have enough money"); 
                        }  
                    }
                    else if (type == "Pants")
                    {
                        if (args[0].ToString() != "0")
                        {
                            if (cash >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_PANTS", args[0]);
                                Account.UpdateMoney(player, "Cash", cash -= 1000);
                                API.setPlayerClothes(player, 4, Convert.ToInt32(args[0]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "takes out $1000 and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else if (bank >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_PANTS", args[0]);
                                Account.UpdateMoney(player, "Bank", bank -= 1000);
                                API.setPlayerClothes(player, 4, Convert.ToInt32(args[0]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "swipes their card and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else API.sendChatMessageToPlayer(player, "You do not have enough money");
                        }
                    }
                    else if (type == "Shoes")
                    {
                        if (args[0].ToString() != "0")
                        {
                            if (cash >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_SHOES", args[0]);
                                Account.UpdateMoney(player, "Cash", cash -= 1000);
                                API.setPlayerClothes(player, 6, Convert.ToInt32(args[0]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "takes out $1000 and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else if (bank >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_SHOES", args[0]);
                                Account.UpdateMoney(player, "Bank", bank -= 1000);
                                API.setPlayerClothes(player, 6, Convert.ToInt32(args[0]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "swipes their card and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else API.sendChatMessageToPlayer(player, "You do not have enough money");
                        }
                    }
                    else if (type == "Barber")
                    {
                        if (args[0].ToString() != "0")
                        {
                            if (cash >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_HAIR_STYLE", args[0]);
                                Account.UpdateMoney(player, "Cash", cash -= 1000);
                                API.setPlayerClothes(player, 2, Convert.ToInt32(args[0]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "takes out $1000 and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else if (bank >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_HAIR_STYLE", args[0]);
                                player.setData("CharBank", bank -= 1000);
                                API.setPlayerClothes(player, 2, Convert.ToInt32(args[0]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "swipes their card and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else API.sendChatMessageToPlayer(player, "You do not have enough money");
                            
                        }
                        else if (args[1].ToString() != "0")
                        {
                            if (cash >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_HAIR_COLOR", args[1]);
                                Account.UpdateMoney(player, "Cash", cash -= 1000);
                                API.sendNativeToAllPlayers(Hash._SET_PED_HAIR_COLOR, player, Convert.ToInt32(args[1]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "takes out $1000 and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else if (bank >= 1000)
                            {
                                API.setEntitySyncedData(player, "GTAO_HAIR_COLOR", args[1]);
                                player.setData("CharBank", bank -= 1000);
                                API.sendNativeToAllPlayers(Hash._SET_PED_HAIR_COLOR, player, Convert.ToInt32(args[1]), 0);
                                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "swipes their card and payed the cashier", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            }
                            else API.sendChatMessageToPlayer(player, "You do not have enough money"); 
                        }
                    }
                }
            }
            else if (eventName == "IsPlayerInBank")
            {
                if (player.getData("IN_ATM") != null)
                {
                    string type = player.getData("IN_ATM_TYPE");
                    int amount = player.getData("IN_ATM_AMOUNT");
                    int cash = player.getData("CharCash");
                    int bank = player.getData("CharBank");
                    int salary = player.getData("CharSalary");
                    if (type == "ATM")
                    {
                        API.shared.triggerClientEvent(player, "MENU_OPEN_ATM", cash, bank, amount);
                    }
                    else if (type == "Bank")
                    {
                        API.shared.triggerClientEvent(player, "MENU_OPEN_BANK", cash, bank, amount, salary);
                    }
                }
            }
            else if (eventName == "ATM_WITHDRAW")
            {
                if (player.getData("IN_ATM") != null)
                {
                    int atmamount = player.getData("IN_ATM_AMOUNT");
                    int amount = Convert.ToInt32(args[0]);
                    int bank = player.getData("CharBank");
                    int cash = player.getData("CharCash");
                    if (bank >= amount)
                    {
                        if (atmamount >= amount)
                        {
                            ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "takes their cash from the ATM and puts it into their wallet", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                            player.setData("IN_ATM_AMOUNT", atmamount - amount);
                            Account.UpdateMoney(player, "Bank", bank - amount);
                            Account.UpdateMoney(player, "Cash", cash + amount);
                            API.shared.triggerClientEvent(player, "SOUND_ATM");
                        }
                        else player.sendChatMessage("This ATM does not have that much money");
                    }
                    else player.sendChatMessage("You do not have the appropriate funds");
                }
            }
            else if (eventName == "ATM_DEPOSIT")
            {
                if (player.getData("IN_ATM") != null)
                {
                    int atmamount = player.getData("IN_ATM_AMOUNT");
                    int amount = Convert.ToInt32(args[0]);
                    int bank = player.getData("CharBank");
                    int cash = player.getData("CharCash");
                    if (cash >= amount)
                    {
                        player.setData("IN_ATM_AMOUNT", atmamount + amount);
                        ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "takes out cash from their wallet and deposits it into the ATM.", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                        Account.UpdateMoney(player, "Bank", bank + amount);
                        Account.UpdateMoney(player, "Cash", cash - amount);
                        API.shared.triggerClientEvent(player, "SOUND_ATM");
                    }
                    else player.sendChatMessage("You do not have the appropriate funds");
                }
            }
            else if (eventName == "BANK_GET_SALARY")
            {
                if (player.getData("IN_ATM") != null)
                {
                    int salaryamount = Convert.ToInt32(args[0]);
                    int bankamount = player.getData("CharBank");
                    Account.UpdateMoney(player, "Salary", 0);
                    Account.UpdateMoney(player, "Bank", bankamount + salaryamount);
                    player.sendChatMessage("You're salary of $" + salaryamount + " has been deposited into your bank account");
                }
            }
            else if (eventName == "BANK_TRANSFER")
            {
                if (player.getData("IN_ATM") != null)
                {
                    Client target = API.getPlayerFromName(args[0].ToString());
                    int amount = Convert.ToInt32(args[1]);
                    int bank = player.getData("CharBank");
                    int targetbank = target.getData("CharBank");
                    if (bank >= amount)
                    {
                        Account.UpdateMoney(player, "Bank", bank - amount);
                        player.sendChatMessage("You have transfered $" + amount + " to " + target.name + "'s account");
                        target.sendChatMessage(player.name + " has transferred $" + amount + " to your account");
                        Account.UpdateMoney(target, "Bank", targetbank + amount);
                    }
                    else player.sendChatMessage("You do not have the appropriate funds");
                }
            }
            else if(eventName == "IsPlayerInJobVehicleSpawn")
            {
                if (player.getData("CharJob") == player.getData("InJobNameVehicleSpawn"))
                {
                    if (player.getData("InJobVehicleSpawn") != null)
                    {
                        if (player.getData("CharJobVehicle") == null)
                        {
                            if (player.getData("CharJob") == "Go Postal")
                            {
                                VehicleHash jobvehicle = player.getData("InJobVehicleSpawn");
                                var vehicle = API.createVehicle(jobvehicle, player.position, new Vector3(0, 0, player.rotation.Z), 111, 77);
                                API.setVehicleEngineStatus(vehicle, false);
                                API.setPlayerIntoVehicle(player, vehicle, -1);
                                player.setData("CharJobVehicle", vehicle);
                                player.setData("notInJobVehicle", 1);
                            }
                            else if (player.getData("CharJob") == "Money Transport")
                            {
                                VehicleHash jobvehicle = player.getData("InJobVehicleSpawn");
                                var vehicle = API.createVehicle(jobvehicle, player.position, new Vector3(0, 0, player.rotation.Z), 112, 59);
                                API.setVehicleEngineStatus(vehicle, false);
                                API.setPlayerIntoVehicle(player, vehicle, -1);
                                player.setData("CharJobVehicle", vehicle);
                                player.setData("notInJobVehicle", 1);
                            }
                        }
                    }
                }
            }
            else if (eventName == "IsPlayerInJobVehicle")
            {
                if (player.getData("CharJobVehicle") != null && player.getData("CharJobVehicle") == API.getPlayerVehicle(player))
                {
                    if (player.getData("CharJob") == "Go Postal")
                    {
                        API.shared.triggerClientEvent(player, "GoPostalJobCEFBrowser");
                    }
                    else if (player.getData("CharJob") == "Money Transport")
                    {
                        API.shared.triggerClientEvent(player, "MoneyTransportJobCEFBrowser");
                    }
                }
            }
            else if (eventName == "JobFirstLoadUp")
            {
                string jobtype = player.getData("CharJob");
                var veh = player.getData("CharJobVehicle");
                var parcel = player.getData("JobParcels");
                var capacity = player.getData("JobCapacity");

                //==========================
                //Go Postal Job First Loadup
                //==========================
                if (jobtype == "Go Postal" && player.position.DistanceTo(new Vector3(68.7, 127.5572, 78.11868)) <= 20 && veh == API.getPlayerVehicle(player) && parcel <= 0)
                {
                    player.setData("HoldingBox", false);
                    API.shared.triggerClientEvent(player, "DestroyJobDropOffLocation");

                    char[] delimiterChars = { ',' };
                    string dropofflocation = JobManager.RandomGoPostalDropOff(player);
                    string[] dropoff = dropofflocation.Split(delimiterChars);
                    Vector3 position1 = new Vector3(float.Parse(dropoff[0]), float.Parse(dropoff[1]), float.Parse(dropoff[2])-1);
                    Vector3 position2 = new Vector3(float.Parse(dropoff[0]), float.Parse(dropoff[1]), float.Parse(dropoff[2]));

                    API.sendNotificationToPlayer(player, "Please wait while we fill up your truck");
                    API.shared.triggerClientEvent(player, "FillGoPostalTruck");
                    for (int i = 1; i <= 10; i++)
                    {
                        API.setEntityPositionFrozen(veh, true);
                        player.setData("JobParcels", i);
                        API.shared.triggerClientEvent(player, "HUD_UPDATE_GOPOSTAL_PARCEL", i, veh, jobtype);
                        await Task.Delay(1000);
                    }
                    API.setEntityPositionFrozen(veh, false);
                    API.shared.triggerClientEvent(player, "SetFirstJobDropOffLocation", position1, position2, float.Parse(dropoff[0]), float.Parse(dropoff[1]), jobtype);
                    player.setData("JobDropOffLocation", position1);
                    int pay = Convert.ToInt32(player.position.DistanceTo(position1) * 0.25);
                    player.setData("CharJobPay", pay);
                }

                //================================
                //Money Transport Job First Loadup
                //================================
                else if (jobtype == "Money Transport" && player.position.DistanceTo(new Vector3(-142.888, -573.5522, 30.77533)) <= 20 && veh == API.getPlayerVehicle(player) && capacity <= 0)
                {
                    API.shared.triggerClientEvent(player, "DestroyJobDropOffLocation");

                    char[] delimiterChars = { ',' };
                    string dropofflocation = JobManager.RandomMoneyTransportDropOff(player);
                    string[] dropoff = dropofflocation.Split(delimiterChars);
                    Vector3 position1 = new Vector3(float.Parse(dropoff[0]), float.Parse(dropoff[1]), float.Parse(dropoff[2]) - 1);
                    Vector3 position2 = new Vector3(float.Parse(dropoff[0]), float.Parse(dropoff[1]), float.Parse(dropoff[2]));

                    player.sendPictureNotificationToPlayer("Please wait while we fill up your truck", "CHAR_BANK_MAZE", 1, 5, "~b~Money Transport", "Loading up");
                    API.shared.triggerClientEvent(player, "FillMoneyTransportTruck");
                    for (int i = 1; i <= 10; i++)
                    {
                        API.setEntityPositionFrozen(veh, true);
                        player.setData("JobCapacity", i * 10000);
                        API.shared.triggerClientEvent(player, "HUD_UPDATE_MONEYTRANSPORT_CAPACITY", i * 10000, veh, jobtype);
                        await Task.Delay(1000);
                    }
                    API.setEntityPositionFrozen(veh, false);
                    player.setData("JobDropOffLocation", position1);
                    int pay = Convert.ToInt32(player.position.DistanceTo(position1) * 0.25);
                    player.setData("CharJobPay", pay);

                    Random rnd = new Random();
                    int nextdropoffamount = rnd.Next(5000, 13000);
                    player.setData("JobNextDropOffAmount", nextdropoffamount);
                    player.sendPictureNotificationToPlayer("ATM requires: $" + nextdropoffamount + " we have set the destination on your GPS", "CHAR_BANK_MAZE", 1, 5, "~b~Money Transport", "Drop-off Location");
                    API.shared.triggerClientEvent(player, "SetFirstJobDropOffLocation", position1, position2, float.Parse(dropoff[0]), float.Parse(dropoff[1]), jobtype, nextdropoffamount);
                }
            }
            else if (eventName == "IsPlayerCloseToJobVehicle")
            {
                if (player.getData("CharJobVehicle") != null)
                {
                    //===========================
                    //Go Postal Close to Vehicle
                    //===========================
                    var jobtype = player.getData("CharJob");
                    if (jobtype == "Go Postal")
                    {
                        var veh = player.getData("CharJobVehicle");
                        var parcel = player.getData("JobParcels");
                        float distToTrunk = 4f;
                        Vector3 rear = veh.position - new Vector3((float)Math.Cos(veh.rotation.Z * Math.PI / 180f + Math.PI * 0.50d), (float)Math.Sin(veh.rotation.Z * Math.PI / 180f + Math.PI * 0.50d), 0) * distToTrunk;
                        if (player.position.DistanceTo(rear) <= 2 && parcel >= 1)
                        {
                            if (player.getData("HoldingBox") == true)
                            {
                                NetHandle box = player.getData("JobBox");
                                API.playPlayerAnimation(player, (int)(AnimationFlags.AllowPlayerControl), "pickup_object", "putdown_low");
                                API.setVehicleDoorState(veh.handle, 2, true);
                                API.setVehicleDoorState(veh.handle, 3, true);
                                player.setData("HoldingBox", false);
                                await Task.Delay(1000);
                                API.setVehicleDoorState(veh.handle, 2, false);
                                API.setVehicleDoorState(veh.handle, 3, false);
                                API.deleteEntity(box);
                                API.stopPlayerAnimation(player);
                                player.sendChatMessage("This is broken");
                            }
                            else
                            {
                                NetHandle box = API.createObject(-517243780, new Vector3(player.position.X, player.position.Y, player.position.Z), new Vector3(player.rotation.X, player.rotation.Y, player.rotation.Z));
                                API.attachEntityToEntity(box, player, "PH_R_Hand", new Vector3(0, 0, 0), new Vector3(0, 0, 0));
                                API.playPlayerAnimation(player, (int)(AnimationFlags.AllowPlayerControl), "pickup_object", "putdown_low");
                                API.setVehicleDoorState(veh.handle, 2, true);
                                API.setVehicleDoorState(veh.handle, 3, true);
                                player.setData("HoldingBox", true);
                                player.setData("JobBox", box);
                                await Task.Delay(1000);
                                API.setVehicleDoorState(veh.handle, 2, false);
                                API.setVehicleDoorState(veh.handle, 3, false);
                                API.playPlayerAnimation(player, (int)(AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.StopOnLastFrame), "anim@heists@box_carry@", "run");
                            }
                        }
                    }
                }
            }
            else if (eventName == "IsPlayerAtJobDropOff")
            {
                Vector3 dropoffdestination = player.getData("JobDropOffLocation");
                var jobtype = player.getData("CharJob");

                //==========================
                //Go Postal Job Drop-Off
                //==========================
                if (jobtype == "Go Postal")
                {
                    if (player.position.DistanceTo(dropoffdestination) <= 2 && player.getData("HoldingBox") == true)
                    {
                        NetHandle box = player.getData("JobBox");
                        var veh = player.getData("CharJobVehicle");
                        player.setData("HoldingBox", false);
                        var parcels = player.getData("JobParcels");
                        API.playPlayerAnimation(player, (int)(AnimationFlags.AllowPlayerControl), "pickup_object", "pickup_low");
                        parcels--;
                        API.shared.triggerClientEvent(player, "HUD_UPDATE_GOPOSTAL_PARCEL", parcels, veh);
                        API.shared.triggerClientEvent(player, "DestroyJobDropOffLocation");

                        API.detachEntity(box);

                        var frontposition = getPositionInfrontOfPlayer(player, 1);

                        API.setEntityPosition(box, frontposition);
                        player.setData("JobParcels", parcels);

                        int previouspay = player.getData("CharJobPay");
                        int salary = player.getData("CharSalary");
                        Account.UpdateMoney(player, "Salary", salary + previouspay);
                        player.sendChatMessage("$" + previouspay + " was just added into your salary");

                        char[] delimiterChars = { ',' };
                        string dropofflocation = JobManager.RandomGoPostalDropOff(player);
                        string[] dropoff = dropofflocation.Split(delimiterChars);
                        Vector3 position1 = new Vector3(float.Parse(dropoff[0]), float.Parse(dropoff[1]), float.Parse(dropoff[2]) - 1);
                        Vector3 position2 = new Vector3(float.Parse(dropoff[0]), float.Parse(dropoff[1]), float.Parse(dropoff[2]));

                        int pay = Convert.ToInt32(player.position.DistanceTo(position1) * 0.25);
                        player.setData("CharJobPay", pay);

                        API.shared.triggerClientEvent(player, "SetJobDropOffLocation", position1, position2, float.Parse(dropoff[0]), float.Parse(dropoff[1]), jobtype);
                        player.setData("JobDropOffLocation", position1);
                        await Task.Delay(10000);
                        API.stopPlayerAnimation(player);
                        API.deleteEntity(box);
                    }
                }
                //============================
                //Money Transport Job Drop-Off
                //============================
                else if (jobtype == "Money Transport")
                {
                    var veh = player.getData("CharJobVehicle");
                    var capacity = player.getData("JobCapacity");
                    var dropoffneeded = player.getData("JobNextDropOffAmount");
                    if (player.position.DistanceTo(dropoffdestination) <= 4 && veh == API.getPlayerVehicle(player))
                    {
                        if (capacity >= dropoffneeded)
                        {
                            player.sendPictureNotificationToPlayer("MT: Please wait while we unload your vehicle", "CHAR_BANK_MAZE", 1, 5, "~b~Money Transport", "Unloading");
                            for (int i = 0; i <= dropoffneeded; i++)
                            {
                                API.setEntityPositionFrozen(veh, true);
                                capacity--;
                            }
                            API.setEntityPositionFrozen(veh, false);
                            API.shared.triggerClientEvent(player, "HUD_UPDATE_MONEYTRANSPORT_CAPACITY", capacity, veh, jobtype);

                            Random rnd = new Random();
                            int nextdropoffamount = rnd.Next(5000, 13000);
                            player.setData("JobNextDropOffAmount", nextdropoffamount);
                            API.shared.triggerClientEvent(player, "DestroyJobDropOffLocation");

                            player.setData("JobCapacity", capacity);

                            int previouspay = player.getData("CharJobPay");
                            int salary = player.getData("CharSalary");
                            Account.UpdateMoney(player, "Salary", salary + previouspay);
                            player.sendChatMessage("$" + previouspay + " was just added into your salary");

                            char[] delimiterChars = { ',' };
                            string dropofflocation = JobManager.RandomMoneyTransportDropOff(player);
                            string[] dropoff = dropofflocation.Split(delimiterChars);
                            Vector3 position1 = new Vector3(float.Parse(dropoff[0]), float.Parse(dropoff[1]), float.Parse(dropoff[2]) - 1);
                            Vector3 position2 = new Vector3(float.Parse(dropoff[0]), float.Parse(dropoff[1]), float.Parse(dropoff[2]));

                            int pay = Convert.ToInt32(player.position.DistanceTo(position1) * 0.25);
                            player.setData("CharJobPay", pay);

                            API.shared.triggerClientEvent(player, "SetJobDropOffLocation", position1, position2, float.Parse(dropoff[0]), float.Parse(dropoff[1]), jobtype, nextdropoffamount);
                            player.sendPictureNotificationToPlayer("Next ATM requires: $" + nextdropoffamount + " we have set the destination on your GPS", "CHAR_BANK_MAZE", 1, 5, "~b~Money Transport", "Drop-off Location");
                            player.setData("JobDropOffLocation", position1);
                        }
                        else player.sendChatMessage("You do not have enough money in your truck to drop off here.");
                    }
                }
            }
            else if (eventName == "StopJob")
            {
                var veh = player.getData("CharJobVehicle");
                player.setData("OnJob", false);
                player.setData("HoldingBox", false);
                player.setData("JobParcels", 0);
                player.setData("JobCapacity", 0);
                API.shared.triggerClientEvent(player, "StopJobActivated");
                API.shared.triggerClientEvent(player, "HUD_UPDATE_GOPOSTAL_PARCEL", 0, veh, 0);
            }
            else if (eventName == "EngineOn")
            {
                var veh = API.getPlayerVehicle(player);
                if (API.getVehicleEngineStatus(veh) == false && API.isPlayerInAnyVehicle(player))
                {
                    API.setVehicleEngineStatus(veh, true);
                    ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "turns the key in their ignition turning the vehicle on", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                }
                
            }
            else if (eventName == "EngineOff")
            {
                var veh = API.getPlayerVehicle(player);
                if (API.getVehicleEngineStatus(veh) == true && API.isPlayerInAnyVehicle(player))
                {
                    API.setVehicleEngineStatus(veh, false);
                    ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "turns the key in their ignition turning the vehicle off", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                }
            }
            else if (eventName == "UpdateIndicator")
            {
                if (!player.isInVehicle || player.vehicleSeat != -1) return;
                int indicator = Convert.ToInt32(args[0]);
                string indicatorName = string.Format("Indicator_{0}", indicator);
                bool indicatorStatus = false;
                if (player.vehicle.hasSyncedData(indicatorName))
                {
                    indicatorStatus = player.vehicle.getSyncedData(indicatorName);
                    indicatorStatus = !indicatorStatus;
                }
                else
                {
                    indicatorStatus = true;
                }
                player.vehicle.setSyncedData(indicatorName, indicatorStatus);
                player.triggerEvent("IndicatorSubtitle", indicator, indicatorStatus);
                API.sendNativeToAllPlayers(Hash.SET_VEHICLE_INDICATOR_LIGHTS, player.vehicle.handle, indicator, indicatorStatus);
            }
        }

        private void OnPlayerExitVehicleHandler(Client player, NetHandle vehicle)
        {
            if (player.seatbelt == true)
            {
                player.seatbelt = false;
                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "takes their seat belt off", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
            }
        }

        public void onSentMessage(Client player, string message, CancelEventArgs e)
        {
            ProxDetector(30, player, player.name.Replace("_", " ") + " says: " + message, "~#FFFFFF~", "~#C8C8C8~", "~#AAAAAA~", "~#8C8C8C~");
            e.Cancel = true;
            return;
        }

        public void ProxDetector(float radius, Client player, string message, string col1, string col2, string col3, string col4)
        {
            var players = API.getPlayersInRadiusOfPlayer(radius, player);
            foreach (Client c in players)
            {
                if (player.position.DistanceTo(c.position) <= radius / 16)
                {
                    API.sendChatMessageToPlayer(c, col1, message);
                }
                else if (player.position.DistanceTo(c.position) <= radius / 8)
                {
                    API.sendChatMessageToPlayer(c, col2, message);
                }
                else if (player.position.DistanceTo(c.position) <= radius / 4)
                {
                    API.sendChatMessageToPlayer(c, col3, message);
                }
                else if (player.position.DistanceTo(c.position) <= radius / 2)
                {
                    API.sendChatMessageToPlayer(c, col4, message);
                }
            }
        }

        public static Vector3 StringToVector(string VectorString)
        {
            var Temp = VectorString.Split('/');
            return new Vector3(Convert.ToSingle(Temp[0]), Convert.ToSingle(Temp[1]), Convert.ToSingle(Temp[2]));
        }

        public static string GetSha256FromString(string strData)
        {
            var message = Encoding.ASCII.GetBytes(strData);
            var hashString = new SHA256Managed();
            var hex = "";

            var hashValue = hashString.ComputeHash(message);
            foreach (var x in hashValue)
                hex += string.Format("{0:x2}", x);
            return hex;
        }

        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        public Vector3 getPositionInfrontOfPlayer(Client player, float distance)
        {
            Vector3 pos = API.getEntityPosition(player);
            var a = API.getEntityRotation(player).Z;

            var rad = a * Math.PI / 180;

            var newpos = new Vector3(pos.X + (distance * Math.Sin(-rad)),
                pos.Y + (distance * Math.Cos(-rad)),
                pos.Z-0.86);
            return newpos;
        }
    }
}
