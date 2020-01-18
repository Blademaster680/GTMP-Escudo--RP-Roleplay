using System;
using System.Data;
using GrandTheftMultiplayer.Server.Elements;
using EGRPProject.Database;
using GrandTheftMultiplayer.Server.API;
using EGRPProject.Data;
using GrandTheftMultiplayer.Server.Constant;
using MySql.Data.MySqlClient;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using System.Threading.Tasks;

namespace EGRPProject
{
    public class Account : Script
    {
        public static string myConnectionString = "SERVER=ts.escudo-gaming.com;" + "PORT=3307;" + "DATABASE=egrp;" + "UID=egrp;" + "PASSWORD=D3c3mb3r;";
        public static MySqlConnection connection;
        public static MySqlCommand command;
        public static MySqlDataReader Reader;

        public static void RegisterPlayer(Client player, string username, string email, string password)
        {
            var userExist =
                MySQL.QueryResult(
                    $"SELECT Username FROM Accounts WHERE Username='{username}'");
            if (userExist == null)
            {
                MySQL.Query(
                    $"INSERT INTO Accounts (Username, Email, Password) VALUES ('{username}', '{email}', '{password}')");
                player.sendChatMessage("You have successfully registered. Please login");
                API.shared.triggerClientEvent(player, "AccountLogin");
                player.freeze(true);
            }
            else
            {
                player.sendChatMessage("That username is already taken.");
                API.shared.triggerClientEvent(player, "AccountRegister");
            }
            connection.Close();
        }

        public static void LoginPlayer(Client player, string username, string password)
        {
            var userExist =
                MySQL.QueryResult(
                    $"SELECT Username FROM Accounts WHERE Username='{username}' AND Password='{password}'");
            if (userExist != null)
            {

                connection = new MySqlConnection(myConnectionString);
                command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Accounts WHERE Username='" + username + "' AND Password='" + password + "'";

                connection.Open();
                Reader = command.ExecuteReader();
                while (Reader.Read())
                {
                    int ID = Reader.GetInt32("ID");
                    string Username = Reader.GetString("Username");
                    int CharacterAmount = Reader.GetInt32("CharacterAmount");
                    player.setData("AccountID", ID);
                    player.setData("AccountUsername", Username);
                    player.setData("AccountPassword", password);
                    player.setData("AccountCharacterAmount", CharacterAmount);
                }
                connection.Close();

                LoadCharacters(player);
                int id1 = Convert.ToInt32(player.getData("Char1ID"));
                int id2 = Convert.ToInt32(player.getData("Char2ID"));
                int id3 = Convert.ToInt32(player.getData("Char3ID"));
                int id4 = Convert.ToInt32(player.getData("Char4ID"));
                int numcharacters = player.getData("AccountCharacterAmount");
                string user = player.getData("AccountUsername");
                string firstname1 = player.getData("Char1FirstName");
                string lastname1 = player.getData("Char1LastName");
                int gender1 = Convert.ToInt32(player.getData("Char1Gender"));
                string firstname2 = player.getData("Char2FirstName");
                string lastname2 = player.getData("Char2LastName");
                int gender2 = Convert.ToInt32(player.getData("Char2Gender"));
                string firstname3 = player.getData("Char3FirstName");
                string lastname3 = player.getData("Char3LastName");
                int gender3 = Convert.ToInt32(player.getData("Char3Gender"));
                string firstname4 = player.getData("Char4FirstName");
                string lastname4 = player.getData("Char4LastName");
                int gender4 = Convert.ToInt32(player.getData("Char4Gender"));
                string empty = "Empty";
                string slot = "Slot";

                if (numcharacters == 0)
                {
                    API.shared.triggerClientEvent(player, "Menu_CharacterSelection", numcharacters, id1, user, empty, slot, empty, numcharacters, id2, user, empty, slot, empty, numcharacters, id3, user, empty, slot, empty, numcharacters, id4, user, empty, slot, empty);
                }
                else if (numcharacters == 1)
                {
                    API.shared.triggerClientEvent(player, "Menu_CharacterSelection", numcharacters, id1, user, firstname1, lastname1, gender1, numcharacters, id2, user, empty, slot, empty, numcharacters, id3, user, empty, slot, empty, numcharacters, id4, user, empty, slot, empty);
                }
                else if (numcharacters == 2)
                {
                    API.shared.triggerClientEvent(player, "Menu_CharacterSelection", numcharacters, id1, user, firstname1, lastname1, gender1, numcharacters, id2, user, firstname2, lastname2, gender2, numcharacters, id3, user, empty, slot, empty, numcharacters, id4, user, empty, slot, empty);
                }
                else if (numcharacters == 3)
                {
                    API.shared.triggerClientEvent(player, "Menu_CharacterSelection", numcharacters, id1, user, firstname1, lastname1, gender1, numcharacters, id2, user, firstname2, lastname2, gender2, numcharacters, id3, user, firstname3, lastname3, gender3, numcharacters, id4, user, empty, slot, empty);
                }
                else if (numcharacters == 4)
                {
                    API.shared.triggerClientEvent(player, "Menu_CharacterSelection", numcharacters, id1, user, firstname1, lastname1, gender1, numcharacters, id2, user, firstname2, lastname2, gender2, numcharacters, id3, user, firstname3, lastname3, gender3, numcharacters, id4, user, firstname4, lastname4, gender4);
                }
                player.freeze(true);
                connection.Close();
            }
            else API.shared.triggerClientEvent(player, "AccountLogin");
        }

        public static void SavePlayer(Client player)
        {
            int numcharacters = player.getData("AccountCharacterAmount");
            string username = player.getData("AccountUsername");
            MySQL.Query(
                   $"UPDATE accounts SET CharacterAmount='{numcharacters}' WHERE Username='{username}'");
            connection.Close();
        }

        public static void CreateCharacter(Client player, string firstname, string lastname)
        {
            var userExist =
                MySQL.QueryResult(
                    $"SELECT ID FROM Characters WHERE Firstname='{firstname}' AND Lastname='{lastname}'");
            if (userExist == null)
            {
                string username = player.getData("AccountUsername");
                int gender = player.getData("CharGender");
                int motherface = player.getData("CharmFace");
                int fatherface = player.getData("CharfFace");
                int motherskin = player.getData("CharmSkin");
                int fatherskin = player.getData("CharfSkin");
                float resshape = player.getData("CharresShape");
                float resskin = player.getData("CharresSkin");
                int eye = player.getData("CharEyes");
                int hair = player.getData("CharHair");
                int haircolor = player.getData("CharHairColor");

                Console.WriteLine("Gender: " + gender + "Motherface: " + motherface + "Fatherface: " + fatherface + "Motherskin: " + motherskin + "Fatherskin: " + fatherskin + 
                    "Ressemblance Shape: " + resshape + "Ressemblance Skin: " + resskin + "Eye: " + eye + "Hair: " + hair + "HairColor: " + haircolor);

                MySQL.Query(
                    $"INSERT INTO Characters (Username, Firstname, Lastname, Cash, Bank, Salary, PosX, PosY, PosZ, Gender, Motherface, Fatherface, Motherskin, Fatherskin, ResShape, ResSkin, Eye, Hair, " +
                    $"Haircolor) VALUES " +
                    $"('{username}', '{firstname}', '{lastname}', '{500}', '{5000}', '{0}', '{0.0}', '{0.5}', '{0.5}', '{gender}', '{motherface}', '{fatherface}', '{motherskin}', '{fatherskin}', '{resshape}', '{resskin}', " +
                    $"'{eye}', '{hair}', '{haircolor}')");
                player.sendChatMessage("Your character has been successfully created");
                player.freeze(false);

                int characternum = player.getData("AccountCharacterAmount");
                characternum++;
                player.setData("AccountCharacterAmount", characternum);
                connection.Close();

                UpdateMoney(player, "Cash", 500);
                UpdateMoney(player, "Bank", 5000);
                UpdateMoney(player, "Salary", 0);
                player.position = new Vector3(-1040.953, -2743.348, 13.94504);
                player.sendChatMessage("Welcome to Los Santos");
                player.setData("LoggedIn", true);
            }
            else
            {
                player.sendChatMessage("That name is already taken.");
                API.shared.triggerClientEvent(player, "CreateCharacter");
            }
        }

        public static void LoadCharacters(Client player)
        {
            string username = player.getData("AccountUsername");
            MySqlConnection connection;
            MySqlCommand command;
            MySqlDataReader Reader;

            connection = new MySqlConnection(myConnectionString);

            command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Characters WHERE Username='" + username + "'"; 

            connection.Open();
            Reader = command.ExecuteReader();
            int i = 1;
            while (Reader.Read())
            {
                player.setData("Char" + i + "ID", Reader.GetInt32("ID"));
                player.setData("Char" + i + "FirstName", Reader.GetString("Firstname"));
                player.setData("Char" + i + "LastName", Reader.GetString("Lastname"));
                player.setData("Char" + i + "Gender", Reader.GetInt32("Gender"));
                i++;
            }
            connection.Close();

            player.freeze(true);
        }

        public static void LoadCharacter(Client player, int ID, int Gender)
        {
            string username = player.getData("AccountUsername");
            MySqlConnection connection;
            MySqlCommand command;
            MySqlDataReader Reader;

            connection = new MySqlConnection(myConnectionString);

            command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Characters WHERE ID='" + ID + "' AND Username='" + username + "'";

            connection.Open();
            Reader = command.ExecuteReader();
            while (Reader.Read())
            {
                player.setData("CharID", Reader.GetString("ID"));
                player.setData("CharUsername", Reader.GetString("Username"));
                player.setData("CharFirstName", Reader.GetString("Firstname"));
                player.setData("CharLastName", Reader.GetString("Lastname"));
                player.setData("CharCash", Reader.GetInt32("Cash"));
                player.setData("CharBank", Reader.GetInt32("Bank"));
                player.setData("CharSalary", Reader.GetInt32("Salary"));
                player.setData("CharPosX", Reader.GetFloat("PosX"));
                player.setData("CharPosY", Reader.GetFloat("PosY"));
                player.setData("CharPosZ", Reader.GetFloat("PosZ"));
                player.setData("CharPosX", Reader.GetFloat("PosX"));
                player.setData("CharGender", Reader.GetInt32("Gender"));
                player.setData("CharmFace", Reader.GetInt32("Motherface"));
                player.setData("CharfFace", Reader.GetInt32("Fatherface"));
                player.setData("CharmSkin", Reader.GetInt32("Motherskin"));
                player.setData("CharfSkin", Reader.GetInt32("Fatherskin"));
                player.setData("CharresShape", Reader.GetInt32("ResShape"));
                player.setData("CharresSkin", Reader.GetInt32("ResSkin"));
                player.setData("CharEyes", Reader.GetInt32("Eye"));
                player.setData("CharHair", Reader.GetInt32("Hair"));
                player.setData("CharHairColor", Reader.GetInt32("Haircolor"));
                player.setData("CharHat", Reader.GetInt32("Hat"));
                player.setData("CharGlasses", Reader.GetInt32("Glasses"));
                player.setData("CharEars", Reader.GetInt32("Ears"));
                player.setData("CharTop", Reader.GetInt32("Top"));
                player.setData("CharUndertop", Reader.GetInt32("Undertop"));
                player.setData("CharPants", Reader.GetInt32("Pants"));
                player.setData("CharShoes", Reader.GetInt32("Shoes"));
                player.setData("CharJob", Reader.GetString("Job"));
            }
            connection.Close();
            player.freeze(false);
            player.setData("LoggedIn", true);

            int gender = player.getData("CharGender");
            int motherface = player.getData("CharmFace");
            int fatherface = player.getData("CharfFace");
            int motherskin = player.getData("CharmSkin");
            int fatherskin = player.getData("CharfSkin");
            float resshape = player.getData("CharresShape");
            float resskin = player.getData("CharresSkin");
            int eye = player.getData("CharEyes");
            int hair = player.getData("CharHair");
            int haircolor = player.getData("CharHairColor");
            int hat = player.getData("CharHat");
            int glasses = player.getData("CharGlasses");
            int ears = player.getData("CharEars");
            int top = player.getData("CharTop");
            int undertop = player.getData("CharUndertop");
            int pants = player.getData("CharPants");
            int shoes = player.getData("CharShoes");

            string job = player.getData("CharJob");

            float x = player.getData("CharPosX");
            float y = player.getData("CharPosY");
            float z = player.getData("CharPosZ");

            if (Gender == 1)
            { API.shared.setPlayerSkin(player, PedHash.FreemodeMale01); }
            else if (Gender == 2)
            { API.shared.setPlayerSkin(player, PedHash.FreemodeFemale01); }

            API.shared.sendNativeToAllPlayers(0x9414E18B9434C2FE, player, motherface, fatherface, 0, motherskin, fatherskin, 0, resshape, resskin, 0.0, true);
            API.shared.sendNativeToAllPlayers(Hash._SET_PED_EYE_COLOR, player.handle, eye);
            API.shared.sendNativeToAllPlayers(Hash.SET_PED_COMPONENT_VARIATION, player.handle, 2, hair, 0, 0);
            API.shared.sendNativeToAllPlayers(Hash._SET_PED_HAIR_COLOR, player.handle, haircolor);

            API.shared.sendNativeToAllPlayers(Hash.SET_PED_PROP_INDEX, player.handle, 0, hat, 0, 0);
            API.shared.sendNativeToAllPlayers(Hash.SET_PED_PROP_INDEX, player.handle, 1, glasses, 0, 0);
            API.shared.sendNativeToAllPlayers(Hash.SET_PED_PROP_INDEX, player.handle, 2, ears, 0, 0);

            API.shared.sendNativeToAllPlayers(Hash.SET_PED_COMPONENT_VARIATION, player.handle, 11, top, 0, 0);
            API.shared.sendNativeToAllPlayers(Hash.SET_PED_COMPONENT_VARIATION, player.handle, 8, undertop, 0, 0);
            API.shared.sendNativeToAllPlayers(Hash.SET_PED_COMPONENT_VARIATION, player.handle, 4, pants, 0, 0);
            API.shared.sendNativeToAllPlayers(Hash.SET_PED_COMPONENT_VARIATION, player.handle, 6, shoes, 0, 0);

            initializePedFace(player.handle, motherface, fatherface, motherskin, fatherskin, resshape, resskin, hair, haircolor, 0, eye, 0, 0, 0, 0, 0, 0, 0, hat, glasses, ears, 
                top, undertop, pants, shoes);

            player.position = new Vector3(x, y, z);
            API.shared.setPlayerName(player, "" + player.getData("CharFirstName") + " " + player.getData("CharLastName"));
            API.shared.triggerClientEvent(player, "HUD_UPDATE_MONEY_DISPLAY", player.getData("CharCash"));
        }

        public static void SaveCharacter(Client player, int ID)
        {
            if (player.getData("LoggedIn") == true)
            {
                int numcharacters = Convert.ToInt32(player.getData("AccountCharacterAmount"));
                string username = player.getData("AccountUsername");
                int cash = player.getData("CharCash");
                int bank = player.getData("CharBank");
                int salary = player.getData("CharSalary");
                float posx = player.position.X;
                float posy = player.position.Y;
                float posz = player.position.Z;
                int hair = API.shared.getEntitySyncedData(player, "GTAO_HAIR_STYLE");
                int haircolor = API.shared.getEntitySyncedData(player, "GTAO_HAIR_COLOR");

                int hat = API.shared.getEntitySyncedData(player, "GTAO_HAT");
                int glass = API.shared.getEntitySyncedData(player, "GTAO_GLASSES");
                int ear = API.shared.getEntitySyncedData(player, "GTAO_EARS");
                int top = API.shared.getEntitySyncedData(player, "GTAO_TOP");
                int undertop = API.shared.getEntitySyncedData(player, "GTAO_UNDERTOP");
                int pants = API.shared.getEntitySyncedData(player, "GTAO_PANTS");
                int shoes = API.shared.getEntitySyncedData(player, "GTAO_SHOES");

                string job = player.getData("CharJob");


                MySQL.Query(
                       $"UPDATE characters SET Cash='{cash}', Bank='{bank}', Salary='{salary}', PosX='{posx}', PosY='{posy}', PosZ='{posz}', Hair='{hair}', Haircolor='{haircolor}', Hat='{hat}', Glasses='{glass}', Ears='{ear}'" +
                       $", Top='{top}', Undertop='{undertop}', Pants='{pants}', Shoes='{shoes}', Job='{job}' WHERE ID='{ID}'");
            }
        }

        public static void initializePedFace(NetHandle ent, int shapea, int shapeb, int skina, int skinb, float shapem, float skinm, int hair, int haircolor, int hairhighlight, int eye, int eyebrow,
            int eyebrowcolor, int makeupcolor, int lipstick, int eyebrowscolor2, int makeupcolor2, int lipstick2, int hat, int glasses, int ears, int top, int undertop, int pants, int shoes)
        {
            API.shared.setEntitySyncedData(ent, "GTAO_HAS_CHARACTER_DATA", true);

            API.shared.setEntitySyncedData(ent, "GTAO_SHAPE_FIRST_ID", shapea);
            API.shared.setEntitySyncedData(ent, "GTAO_SHAPE_SECOND_ID", shapeb);
            API.shared.setEntitySyncedData(ent, "GTAO_SKIN_FIRST_ID", skina);
            API.shared.setEntitySyncedData(ent, "GTAO_SKIN_SECOND_ID", skinb);
            API.shared.setEntitySyncedData(ent, "GTAO_SHAPE_MIX", shapem);
            API.shared.setEntitySyncedData(ent, "GTAO_SKIN_MIX", skinm);
            API.shared.setEntitySyncedData(ent, "GTAO_HAIR_STYLE", hair);
            API.shared.setEntitySyncedData(ent, "GTAO_HAIR_COLOR", haircolor);
            API.shared.setEntitySyncedData(ent, "GTAO_HAIR_HIGHLIGHT_COLOR", hairhighlight);
            API.shared.setEntitySyncedData(ent, "GTAO_EYE_COLOR", eye);
            API.shared.setEntitySyncedData(ent, "GTAO_EYEBROWS", eyebrow);

            //API.setEntitySyncedData(ent, "GTAO_MAKEUP", 0); // No lipstick by default. 
            //API.setEntitySyncedData(ent, "GTAO_LIPSTICK", 0); // No makeup by default.

            API.shared.setEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR", eyebrowcolor);
            API.shared.setEntitySyncedData(ent, "GTAO_MAKEUP_COLOR", makeupcolor);
            API.shared.setEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR", lipstick);
            API.shared.setEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR2", eyebrowscolor2);
            API.shared.setEntitySyncedData(ent, "GTAO_MAKEUP_COLOR2", makeupcolor2);
            API.shared.setEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR2", lipstick2);

            API.shared.setEntitySyncedData(ent, "GTAO_HAT", hat);
            API.shared.setEntitySyncedData(ent, "GTAO_GLASSES", glasses);
            API.shared.setEntitySyncedData(ent, "GTAO_EARS", ears);

            API.shared.setEntitySyncedData(ent, "GTAO_TOP", top);
            API.shared.setEntitySyncedData(ent, "GTAO_UNDERTOP", undertop);
            API.shared.setEntitySyncedData(ent, "GTAO_PANTS", pants);
            API.shared.setEntitySyncedData(ent, "GTAO_SHOES", shoes);

            var list = new float[21];

            for (var i = 0; i < 21; i++)
            {
                list[i] = 0f;
            }

            API.shared.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_LIST", list);
        }

        public static void UpdateMoney(Client player, string type, int amount)
        {
            if(type == "Cash")
            {
                player.setData("CharCash", amount);
                API.shared.triggerClientEvent(player, "HUD_UPDATE_MONEY_DISPLAY", amount);
            }
            else if(type == "Bank")
            {
                player.setData("CharBank", amount);
            }
            else if(type == "Salary")
            {
                player.setData("CharSalary", amount);
            }
        }

        public async static void autoSavePlayers()
        {
            await Task.Delay(120000);
            var players = API.shared.getAllPlayers();
            foreach (var player in players)
            {
                if (player.getData("LoggedIn") == true)
                {
                    int id = Convert.ToInt32(player.getData("CharID"));
                    SaveCharacter(player, id);                    
                }
            }
            autoSavePlayers();
        }
    }
}
