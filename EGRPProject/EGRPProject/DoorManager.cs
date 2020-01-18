using EGRPProject.Database;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace EGRPProject
{
    

    class DoorManager
    {
        public static List<int> doors = new List<int>();

        public static string myConnectionString = "SERVER=ts.escudo-gaming.com;" + "PORT=3307;" + "DATABASE=egrp;" + "UID=egrp;" + "PASSWORD=D3c3mb3r;";

        public static void CreateDoor(Client player, int model, float posx, float posy, float posz, bool locked)
        {
            MySQL.Query(
                    $"INSERT INTO Doors (Model, PosX, PosY, PosZ, Locked) VALUES ('{model}', '{posx}', '{posy}', '{posz}', '{locked}')");
            player.sendChatMessage("Door has been saved");
            int i = API.shared.exported.doormanager.registerDoor(model, new Vector3(posx, posy, posz));
            API.shared.exported.doormanager.setDoorState(i, locked, 0);
        }

        public static void LoadDoors()
        {
            int doorcount = 0;

            MySqlConnection connection;
            MySqlCommand command;
            MySqlDataReader Reader;

            connection = new MySqlConnection(myConnectionString);

            command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Doors";

            //List<string> doors = new List<string>();
            
          
            connection.Open();
            Reader = command.ExecuteReader();
            while (Reader.Read())
            {
                int model = Reader.GetInt32("Model");
                float posx = Reader.GetFloat("PosX");
                float posy = Reader.GetFloat("PosY");
                float posz = Reader.GetFloat("PosZ");
                int i = API.shared.exported.doormanager.registerDoor(model, new Vector3(posx, posy, posz));
                API.shared.exported.doormanager.setDoorState(i, false, 0);
                //doors.Add("|i" + i.ToString() + "|" + model.ToString() + "|" + posx + "|" + posy + "|" + posz + "|");
                doors.Add(i);
                doorcount++;
            }
            connection.Close();
            Console.WriteLine("Doors loaded: " + doorcount);
        }
    }
}
