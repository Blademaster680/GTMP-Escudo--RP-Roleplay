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
    class ATMManager : Script
    {
        public static string myConnectionString = "SERVER=ts.escudo-gaming.com;" + "PORT=3307;" + "DATABASE=egrp;" + "UID=egrp;" + "PASSWORD=D3c3mb3r;";

        public static List<int> atms = new List<int>();

        public static Dictionary<int, ColShape> _bankColShapes = new Dictionary<int, ColShape>();

        static int atmcount = 0;

        public ATMManager()
        {
            API.onEntityEnterColShape += ColShapeTriggerEnter;
            API.onEntityExitColShape += ColShapeTriggerExit;
        }

        public static void CreateATM(Client player, string Type, float posx, float posy, float posz, bool blip)
        {
            MySQL.Query(
                    $"INSERT INTO ATM (Type, PosX, PosY, PosZ, Blip) VALUES ('{Type}', '{posx}', '{posy}', '{posz}', '{blip}')");
            player.sendChatMessage("Bank has been saved");
            LoadATM(atmcount);
        }

        public static void RemoveATM(Client player, int atmID)
        {
            
            MySQL.Query(
                    $"DELETE from ATM where ID = '{atmID}'");
            NetHandle blip = player.getData("IN_ATM_BLIP");
            NetHandle marker = player.getData("IN_ATM_MARKER");
            NetHandle text = player.getData("IN_ATM_TEXT");

            API.shared.deleteEntity(blip);
            API.shared.deleteEntity(marker);
            API.shared.deleteEntity(text);

            player.sendChatMessage("ATM ID: " + atmID + " deleted");
            player.setData("IN_ATM_BLIP", null);
            player.setData("IN_ATM_MARKER", null);
            player.setData("IN_ATM_TEXT", null);
            player.setData("IN_ATM_COLSHAPE", null);

            player.sendChatMessage("ATM ID: " + atmID + " has been deleted");
        }

        public static void LoadATM(int atmid)
        {
            MySqlConnection connection;
            MySqlCommand command;
            MySqlDataReader Reader;

            connection = new MySqlConnection(myConnectionString);

            command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM ATM WHERE ID=" + atmid + "";


            connection.Open();
            Reader = command.ExecuteReader();
            while (Reader.Read())
            {
                atmcount++;
                int id = Convert.ToInt32(Reader.GetString("ID"));
                string type = Reader.GetString("Type");
                float posx = Reader.GetFloat("PosX");
                float posy = Reader.GetFloat("PosY");
                float posz = Reader.GetFloat("PosZ");
                int amount = Reader.GetInt32("Amount");
                bool blip = Reader.GetBoolean("Blip");

                int i = RegisterATM(atmcount, type, posx, posy, posz, amount, blip);
                atms.Add(i)
                    ;
                atmcount++;
            }
            connection.Close();
            Console.WriteLine("ATMs loaded: " + atmcount);
        }

        public static void LoadATMS()
        {
            MySqlConnection connection;
            MySqlCommand command;
            MySqlDataReader Reader;

            connection = new MySqlConnection(myConnectionString);

            command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM ATM";


            connection.Open();
            Reader = command.ExecuteReader();
            while (Reader.Read())
            {
                int id = Convert.ToInt32(Reader.GetString("ID"));
                string type = Reader.GetString("Type");
                float posx = Reader.GetFloat("PosX");
                float posy = Reader.GetFloat("PosY");
                float posz = Reader.GetFloat("PosZ");
                int amount = Reader.GetInt32("Amount");
                bool blip = Reader.GetBoolean("Blip");

                int i = RegisterATM(id, type, posx, posy, posz, amount, blip);
                atms.Add(i);
                atmcount++;
            }
            connection.Close();
            Console.WriteLine("ATMs loaded: " + atmcount);
        }

        public static int RegisterATM(int id, string Type, float PosX, float PosY, float PosZ, int amount, bool blip)
        {
            Console.Write("1 \n");
            var info = new ATMInfo();
            Console.Write("2 \n");
            info.ID = id;
            Console.Write("3 \n");
            info.Type = Type;
            Console.Write("4 \n");
            info.Amount = amount;
            Console.Write("5 \n");
            info.Position = new Vector3(PosX, PosY, PosZ);
            Console.Write("6 \n");

            var colShape = API.shared.createSphereColShape(info.Position, 2f);
            Console.Write("7 \n");
            if (Type == "ATM")
            {
                Console.Write("8 \n");
                var marker = API.shared.createMarker(1, new Vector3(PosX, PosY, PosZ), new Vector3(), new Vector3(), new Vector3(1, 1, 1), 125, 198, 255, 64, 0);
                Console.Write("9 \n");
                var text = API.shared.createTextLabel("Press Y to access the ATM", new Vector3(PosX, PosY, PosZ + 1), 15f, 1f);
                Console.Write("10 \n");
                colShape.setData("ATM_MARKER", marker);
                Console.Write("11 \n");
                colShape.setData("ATM_TEXT", text);
                Console.Write("12 \n");
            }
            else
            {
                var marker = API.shared.createMarker(1, new Vector3(PosX, PosY, PosZ), new Vector3(), new Vector3(), new Vector3(1, 1, 1), 125, 198, 255, 64, 0);
                Console.Write("13 \n");
                var text = API.shared.createTextLabel("Press Y to access the Bank", new Vector3(PosX, PosY, PosZ + 1), 15f, 1f);
                Console.Write("14 \n");
                colShape.setData("ATM_MARKER", marker);
                Console.Write("15 \n");
                colShape.setData("ATM_TEXT", text);
                Console.Write("16 \n");
            }
            
            colShape.setData("ATM_TYPE", Type);
            Console.Write("17 \n");
            colShape.setData("ATM_INFO", info);
            Console.Write("18 \n");
            colShape.setData("ATM_ID", id);
            Console.Write("19 \n");
            colShape.setData("ATM_AMOUNT", amount);
            Console.Write("20 \n");
            colShape.setData("IS_ATM_TRIGGER", true);
            Console.Write("21 \n");

            if (blip == true)
            {
                if (Type == "ATM")
                {
                    Console.Write("22 \n");
                    var atmBlip = API.shared.createBlip(info.Position);
                    Console.Write("23 \n");
                    API.shared.setBlipSprite(atmBlip, 108);
                    Console.Write("24 \n");
                    atmBlip.shortRange = true;
                    Console.Write("25 \n");
                    colShape.setData("ATM_BLIP", atmBlip);
                    Console.Write("26 \n");
                }
                else
                {
                    var bankBlip = API.shared.createBlip(info.Position);
                    Console.Write("27 \n");
                    API.shared.setBlipSprite(bankBlip, 500);
                    Console.Write("28 \n");
                    API.shared.setBlipColor(bankBlip, 69);
                    Console.Write("29 \n");
                    bankBlip.shortRange = true;
                    Console.Write("30 \n");
                    colShape.setData("ATM_BLIP", bankBlip);
                    Console.Write("31 \n");
                }
            }
            _bankColShapes.Add(id, colShape);
            Console.Write("32 \n");

            return id;
        }

        private void ColShapeTriggerEnter(ColShape colshape, NetHandle entity)
        {
            var player = API.getPlayerFromHandle(entity);

            if (player == null) return;

            if (colshape != null && colshape.getData("IS_ATM_TRIGGER") == true)
            {
                var id = colshape.getData("ATM_ID");
                var type = colshape.getData("ATM_TYPE");
                var amount = colshape.getData("ATM_AMOUNT");
                var info = colshape.getData("ATM_INFO");
                var blip = colshape.getData("ATM_BLIP");
                var marker = colshape.getData("ATM_MARKER");
                var text = colshape.getData("ATM_TEXT");
                player.setData("IN_ATM_COLSHAPE", colshape);
                player.setData("IN_ATM", id);
                player.setData("IN_ATM_TYPE", type);
                player.setData("IN_ATM_AMOUNT", amount);
                player.setData("IN_ATM_ID", id);
                player.setData("IN_ATM_BLIP", blip);
                player.setData("IN_ATM_MARKER", marker);
                player.setData("IN_ATM_TEXT", text);
            }
        }

        private void ColShapeTriggerExit(ColShape colshape, NetHandle entity)
        {
            var player = API.getPlayerFromHandle(entity);

            if (player == null) return;

            if (colshape != null && colshape.getData("IS_ATM_TRIGGER") == true)
            {
                var id = colshape.getData("ATM_ID");
                var type = colshape.getData("ATM_TYPE");
                var amount = colshape.getData("ATM_AMOUNT");
                var newamount = player.getData("IN_ATM_AMOUNT");
                var info = colshape.getData("ATM_INFO");
                colshape.setData("ATM_AMOUNT", newamount);
                player.setData("IN_ATM_COLSHAPE", null);
                player.setData("IN_ATM", null);
                player.setData("IN_ATM_TYPE", null);
                player.setData("IN_ATM_ID", null);
                player.setData("IN_ATM_BLIP", null);
                player.setData("IN_ATM_MARKER", null);
                player.setData("IN_ATM_TEXT", null);
                API.shared.triggerClientEvent(player, "Menu_StoreClose");
            }
        }
    }

    public struct ATMInfo
    {
        public int ID;
        public string Type;
        public int Amount;
        public Vector3 Position;
    }
}
