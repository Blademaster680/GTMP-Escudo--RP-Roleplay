using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Shared;
using System.Collections.Generic;

namespace EGRPProject
{
    class Commands : Script
    {
        [Command("test", GreedyArg = true)]
        public void Command_Test(Client player)
        {
            API.sendChatMessageToPlayer(player, "In Store: " + player.getData("IN_SHOP_TYPE"));
        }

        [Command("help", GreedyArg = true)]
        public void Command_Help(Client player)
        {
            Global.CEFController.SendRequest(player, "http://cp.escudo-gaming.com/Game/help.aspx", null);
        }

        [Command("me", GreedyArg = true)]
        public void MeCommand(Client sender, string text)
        {
            ProxDetector(30, sender, "* " + sender.name.Replace("_", " ") + " " + text, "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
        }

        [Command("do", GreedyArg = true)]
        public void Do_Command(Client sender, string text)
        {
            ProxDetector(30, sender, "** " + text + " (" + sender.name.Replace("_", " ") + ")", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
        }

        [Command("Seatbelt", Alias = "sb")]
        public void Command_SeatBelt(Client player)
        {
            if (player.seatbelt == false && API.isPlayerInAnyVehicle(player))
            {
                player.seatbelt = true;
                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "puts their seatbelt on", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
            }
            else if (player.seatbelt == true && API.isPlayerInAnyVehicle(player))
            {
                player.seatbelt = false;
                ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "takes their seat belt off", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
            }
            else
            {
                API.sendChatMessageToPlayer(player, "You are not in a vehicle");
            }
        }

        [Command("gopostal")]
        public void COMMAND_GOPOSTAL(Client player, string dropoff)
        {
            Vector3 PlayerPos = API.getEntityPosition(player);
            API.sendChatMessageToPlayer(player, "X: " + PlayerPos.X + " Y: " + PlayerPos.Y + " Z: " + PlayerPos.Z);
            Console.WriteLine("GOPOSTAL " + dropoff +" LOCATION: " + player.position.X + " " + player.position.Y + " " + player.position.Z);
        }

        [Command("gomoney")]
        public void COMMAND_GOMONEY(Client player, string dropoff)
        {
            Vector3 PlayerPos = API.getEntityPosition(player);
            API.sendChatMessageToPlayer(player, "X: " + PlayerPos.X + " Y: " + PlayerPos.Y + " Z: " + PlayerPos.Z);
            Console.WriteLine("GOPOSTAL " + dropoff + " LOCATION: " + player.position.X + " " + player.position.Y + " " + player.position.Z);
        }



        [Command("pay")]
        public void COMMAND_PAYID(Client player, Client targetName, int amount)
        {
            int playercash = player.getData("CharCash");
            int targetcash = targetName.getData("CharCash");
            int playerfound = 0;

            List<Client> playerList = API.getPlayersInRadiusOfPlayer(20, player);
            foreach (Client playerItem in playerList)
            {
                if (playerItem.name == targetName.name)
                {
                    playerfound = 1;
                }
            }
            if(playerfound == 1)
            {
                if (playercash >= amount)
                {
                    Account.UpdateMoney(player, "Cash", playercash -= amount);
                    Account.UpdateMoney(targetName, "Cash", targetcash += amount);
                    ProxDetector(30, player, "* " + player.name.Replace("_", " ") + " " + "takes out $" + amount + " and hands it to " + targetName.name.Replace("_", " "), "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
                }
                else player.sendChatMessage("You do not have that much cash on you");
            }
            else player.sendChatMessage("You are not close enough to that player");
        }

        [Command("gotoco")]
        public void gotoco(Client player, float x, float y, float z, string interior)
        {
            player.position = new Vector3(x, y, z);

            if (interior == "0")
            {
                API.resetIplList();
            }
            else API.requestIpl(interior);
        }

        [Command("gotoid")]
        public void COMMAND_GOTOID(Client player, Client targetName)
        {
            var pos = targetName.position;
            player.position = pos;
            player.sendChatMessage("You have been teleported to " + targetName.name);
            targetName.sendChatMessage("" + player.name + " has teleported to you");
        }

        [Command("getpos")]
        public void GetPosition(Client player)
        {
            Vector3 PlayerPos = API.getEntityPosition(player);
            API.sendChatMessageToPlayer(player, "X: " + PlayerPos.X + " Y: " + PlayerPos.Y + " Z: " + PlayerPos.Z);
        }


        [Command("setskin")]
        public void setskin(Client player, PedHash Model)
        {
            API.setPlayerSkin(player, Model);
            player.setData("Skin", Model);
        }
        [Command("checkuser")]
        public void checkuser(Client player)
        {
            string username = player.getData("AccountUsername");
            API.sendChatMessageToPlayer(player, "Your username is " + username);
        }

        [Command("car", Alias = "v")]
        public void SpawnCarCommand(Client sender, VehicleHash model)
        {
            var veh = API.createVehicle(model, sender.position, new Vector3(0, 0, sender.rotation.Z), 0, 0);
            API.setVehicleEngineStatus(veh, false);
            API.setPlayerIntoVehicle(sender, veh, -1);
        }

        [Command("createdoor")]
        public void COMMAND_createdoor(Client player, int model, bool locked)
        {
            DoorManager.CreateDoor(player, model, player.position.X, player.position.Y, player.position.Z, locked);
        }

        [Command("createshop")]
        public void COMMAND_createshop(Client player, string ShopType)
        {
            float posx = player.position.X;
            float posy = player.position.Y;
            float posz = player.position.Z - 1.0f;
            ShopManager.CreateShop(player, ShopType, posx, posy, posz);
        }

        [Command("createbank")]
        public void COMMAND_createbank(Client player, string type, bool blip)
        {
            float posx = player.position.X;
            float posy = player.position.Y;
            float posz = player.position.Z - 1.0f;
            ATMManager.CreateATM(player, type, posx, posy, posz, blip);
        }

        [Command("removebank")]
        public void COMMAND_removebank(Client player)
        {
            int atmID = player.getData("IN_ATM");
            ATMManager.RemoveATM(player, atmID);
        }

        [Command("givecash")]
        public void COMMAND_GIVECASH(Client player, int amount)
        {
            int cash = player.getData("CharCash");
            Account.UpdateMoney(player, "Cash", cash + amount);
            player.sendChatMessage("An admin gave you $" + amount);
        }

        [Command("god")]
        public void COMMAND_INVINCIBLE(Client player)
        {
            if(player.invincible == false) player.invincible = true;
            else player.invincible = false;
            player.sendChatMessage("God status: " + player.invincible);
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
    }
}
