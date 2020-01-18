using EGRPProject.Database;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Server.Constant;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace EGRPProject
{
    class ShopManager : Script
    {
        public static string myConnectionString = "SERVER=ts.escudo-gaming.com;" + "PORT=3307;" + "DATABASE=egrp;" + "UID=egrp;" + "PASSWORD=D3c3mb3r;";

        public static List<int> shops = new List<int>();

        public static Dictionary<int, ColShape> _shopColShapes = new Dictionary<int, ColShape>();

        static int shopcount = 0;

        public ShopManager()
        {
            API.onEntityEnterColShape += ColShapeTriggerEnter;
            API.onEntityExitColShape += ColShapeTriggerExit;
        }

        public static void CreateShop(Client player, string shoptype, float posx, float posy, float posz)
        {
            MySQL.Query(
                    $"INSERT INTO Shops (ShopType, PosX, PosY, PosZ) VALUES ('{shoptype}', '{posx}', '{posy}', '{posz}')");
            player.sendChatMessage("Shop has been saved");
            LoadShops();
        }

        public static void LoadShops()
        {
            MySqlConnection connection;
            MySqlCommand command;
            MySqlDataReader Reader;

            connection = new MySqlConnection(myConnectionString);

            command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Shops";


            connection.Open();
            Reader = command.ExecuteReader();
            while (Reader.Read())
            {
                string shoptype = Reader.GetString("ShopType");
                float posx = Reader.GetFloat("PosX");
                float posy = Reader.GetFloat("PosY");
                float posz = Reader.GetFloat("PosZ");

                int i = RegisterShop(shoptype, posx, posy, posz);
                shops.Add(i);
                
            }
            connection.Close();
            Console.WriteLine("Shops loaded: " + shopcount);
        }

        public static int RegisterShop(string Type, float PosX, float PosY, float PosZ)
        {
            var shopID = ++shopcount;
            var info = new ShopInfo();
            info.ID = shopID;
            info.ShopType = Type;
            info.Position = new Vector3(PosX, PosY, PosZ);

            API.shared.createMarker(1, new Vector3(PosX, PosY, PosZ), new Vector3(), new Vector3(), new Vector3(1, 1, 1), 125, 198, 255, 64, 0);
            API.shared.createTextLabel("Press Y to buy " + Type.ToLower(), new Vector3(PosX, PosY, PosZ+1), 15f, 1f);

            var colShape = API.shared.createSphereColShape(info.Position, 2f);
            colShape.setData("SHOP_INFO", info);
            colShape.setData("SHOP_ID", shopID);
            colShape.setData("SHOP_TYPE", Type);
            colShape.setData("IS_SHOP_TRIGGER", true);
            _shopColShapes.Add(shopID, colShape);

            if(Type == "Tops")
            {
                var clothesBlip = API.shared.createBlip(info.Position);
                API.shared.setBlipSprite(clothesBlip, 73);
                clothesBlip.shortRange = true;
            }
            else if (Type == "Barber")
            {
                var barberBlip = API.shared.createBlip(info.Position);
                API.shared.setBlipSprite(barberBlip, 71);
                barberBlip.shortRange = true;
            }
            else if (Type == "Store")
            {
                var storeBlip = API.shared.createBlip(info.Position);
                API.shared.setBlipSprite(storeBlip, 52);
                storeBlip.shortRange = true;
            }

            return shopID;
        }

        private void ColShapeTriggerEnter(ColShape colshape, NetHandle entity)
        {
            var player = API.getPlayerFromHandle(entity);

            if (player == null) return;

            if (colshape != null && colshape.getData("IS_SHOP_TRIGGER") == true)
            {
                var id = colshape.getData("SHOP_ID");
                var type = colshape.getData("SHOP_TYPE");
                var info = colshape.getData("SHOP_INFO");
                player.setData("IN_SHOP_TYPE", type);
            }
        }

        private void ColShapeTriggerExit(ColShape colshape, NetHandle entity)
        {
            var player = API.getPlayerFromHandle(entity);

            if (player == null) return;

            if (colshape != null && colshape.getData("IS_SHOP_TRIGGER") == true)
            {
                var id = colshape.getData("SHOP_ID");
                var type = colshape.getData("SHOP_TYPE");
                var info = colshape.getData("SHOP_INFO");
                player.setData("IN_SHOP_TYPE", null);
                API.shared.triggerClientEvent(player, "Menu_StoreClose");
            }
        }
    }

    public struct ShopInfo
    {
        public int ID;
        public string ShopType;
        public Vector3 Position;
    }
}
