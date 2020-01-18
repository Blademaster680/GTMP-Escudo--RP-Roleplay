using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Shared;
using EGRPProject.Data;

namespace EGRPProject.Jobs
{
    class JobManager : Script
    {
        public static Dictionary<int, ColShape> _JobJoinColShapes = new Dictionary<int, ColShape>();
        public static Dictionary<int, ColShape> _JobVehicleColShapes = new Dictionary<int, ColShape>();

        public static int previouseSecond = 0;

        public JobManager()
        {
            API.onEntityEnterColShape += ColShapeTriggerEnter;
            API.onEntityExitColShape += ColShapeTriggerExit;
            API.onUpdate += OnUpdateHandler;
            API.onPlayerEnterVehicle += OnPlayerEnterVehicleHandler;
            API.onPlayerExitVehicle += OnPlayerExitVehicleHandler;
        }

        public static void OnUpdateHandler()
        {
            if (DateTime.Now.Second != previouseSecond)
            {
                previouseSecond = DateTime.Now.Second;
                List<Client> clients = API.shared.getAllPlayers();
                foreach (Client player in clients)
                {
                    var notInJobVehicle = player.getData("notInJobVehicle");
                    var jobVehicle = player.getData("CharJobVehicle");
                    if (jobVehicle != null)
                    {
                        if (API.shared.isPlayerInAnyVehicle(player))
                        {
                            if (jobVehicle != API.shared.getPlayerVehicle(player))
                            {
                                player.setData("notInJobVehicle", notInJobVehicle++);
                            }
                        }
                        else
                        {
                            notInJobVehicle++;
                            player.setData("notInJobVehicle", notInJobVehicle);
                        }
                        if (notInJobVehicle >= 300)
                        {
                            API.shared.deleteEntity(jobVehicle);
                            player.setData("CharJobVehicle", null);
                            player.setData("notInJobVehicle", 0);
                        }
                    }
                }
            }
        }

        public void OnPlayerEnterVehicleHandler(Client player, NetHandle vehicle)
        {
            var notInJobVehicle = player.getData("notInJobVehicle");
            var jobVehicle = player.getData("CharJobVehicle");
            if (jobVehicle == API.shared.getPlayerVehicle(player))
            {
                player.setData("notInJobVehicle", 1);
                player.triggerEvent("JobVehicleStartJob");
            }
        }

        public void OnPlayerExitVehicleHandler(Client player, NetHandle vehicle)
        {
            if (player.getData("CharJobVehicle") == API.shared.getPlayerVehicle(player))
            {
                player.setData("notInJobVehicle", 1);
            }
        }

        public static void LoadJobs()
        {
            for (int i = 1; i <= 5; i++)
            {
                // Go Postal
                if (i == 1)
                {
                    //Join Job Markers
                    API.shared.createMarker(1, new Vector3(132.7467, 95.91277, 82.50764), new Vector3(), new Vector3(), new Vector3(1, 1, 1), 125, 198, 255, 64, 0);
                    API.shared.createTextLabel("/joinjob to join Go Postal", new Vector3(132.7467, 95.91277, 82.50764 + 1), 15f, 1f);
                    //Setting the collider for the join Go Postal
                    var colShapeGoPostalJoin = API.shared.createSphereColShape(new Vector3(132.7467, 95.91277, 82.50764), 3f);
                    colShapeGoPostalJoin.setData("IS_JOB_JOIN_TRIGGER", true);
                    colShapeGoPostalJoin.setData("JOB_NAME", "Go Postal");
                    _JobJoinColShapes.Add(i, colShapeGoPostalJoin);

                    //Spawn Vehicle for Go Postal
                    API.shared.createMarker(1, new Vector3(78.78649, 111.913, 80.16817), new Vector3(), new Vector3(), new Vector3(1, 1, 1), 125, 198, 255, 64, 0);
                    API.shared.createTextLabel("Press Y to get a vehicle", new Vector3(78.78649, 111.913, 80.16817 + 1), 15f, 1f);
                    var colShapeGoPostalVehicle = API.shared.createSphereColShape(new Vector3(78.78649, 111.913, 80.16817), 15f);
                    colShapeGoPostalVehicle.setData("IS_JOB_VEHICLE_SPAWN_TRIGGER", true);
                    colShapeGoPostalVehicle.setData("JOB_NAME", "Go Postal");
                    colShapeGoPostalVehicle.setData("JOB_VEHICLE", VehicleHash.Boxville2);
                    _JobVehicleColShapes.Add(i, colShapeGoPostalVehicle);

                    //Go Postal Blip
                    var GoPostalBlip = API.shared.createBlip(new Vector3(78.78649, 111.913, 80.16817));
                    API.shared.setBlipSprite(GoPostalBlip, 408);
                    GoPostalBlip.shortRange = true;
                }
                else if (i == 2)
                {
                    //Join Job Markers
                    API.shared.createMarker(1, new Vector3(-198.098, -575.2167, 39.48929), new Vector3(), new Vector3(), new Vector3(1, 1, 1), 125, 198, 255, 64, 0);
                    API.shared.createTextLabel("/joinjob to join Money Transport", new Vector3(-198.098, -575.2167, 40.48929), 15f, 1f);
                    //Setting the collider for the join Money Transport
                    var colShapeMoneyTransportJoin = API.shared.createSphereColShape(new Vector3(-198.098, -575.2167, 40.48929), 3f);
                    colShapeMoneyTransportJoin.setData("IS_JOB_JOIN_TRIGGER", true);
                    colShapeMoneyTransportJoin.setData("JOB_NAME", "Money Transport");
                    _JobJoinColShapes.Add(i, colShapeMoneyTransportJoin);

                    //Spawn Vehicle for Money Transport
                    API.shared.createMarker(1, new Vector3(-173.9839, -616.0065, 31.42448), new Vector3(), new Vector3(), new Vector3(1, 1, 1), 125, 198, 255, 64, 0);
                    API.shared.createTextLabel("Press Y to get a vehicle", new Vector3(-173.9839, -616.0065, 32.42448), 15f, 1f);
                    var colShapeMoneyTransportVehicle = API.shared.createSphereColShape(new Vector3(-173.9839, -616.0065, 32.42448), 15f);
                    colShapeMoneyTransportVehicle.setData("IS_JOB_VEHICLE_SPAWN_TRIGGER", true);
                    colShapeMoneyTransportVehicle.setData("JOB_NAME", "Money Transport");
                    colShapeMoneyTransportVehicle.setData("JOB_VEHICLE", VehicleHash.Stockade);
                    _JobVehicleColShapes.Add(i, colShapeMoneyTransportVehicle);

                    //Money Transport Blip
                    var MoneyTransportBlip = API.shared.createBlip(new Vector3(-164.0759, -610.6197, 31.77557));
                    API.shared.setBlipSprite(MoneyTransportBlip, 408);
                    MoneyTransportBlip.shortRange = true;
                }
            }
        }

        private void ColShapeTriggerEnter(ColShape colshape, NetHandle entity)
        {
            var player = API.getPlayerFromHandle(entity);

            if (player == null) return;

            if (colshape != null && colshape.getData("IS_JOB_JOIN_TRIGGER") == true)
            {
                string jobname = colshape.getData("JOB_NAME");
                player.setData("InJobJoin", jobname);
            }
            else if (colshape != null && colshape.getData("IS_JOB_VEHICLE_SPAWN_TRIGGER") == true)
            {
                string jobname = colshape.getData("JOB_NAME");
                VehicleHash jobvehicle = colshape.getData("JOB_VEHICLE");
                player.setData("InJobNameVehicleSpawn", jobname);
                player.setData("InJobVehicleSpawn", jobvehicle);
                player.triggerEvent("SpawnJobVehicleSubtitle");
            }
        }

        private void ColShapeTriggerExit(ColShape colshape, NetHandle entity)
        {
            var player = API.getPlayerFromHandle(entity);

            if (player == null) return;

            if (colshape != null && colshape.getData("IS_JOB_TRIGGER") == true)
            {
                player.setData("InJobJoin", null);
            }
            else if (colshape != null && colshape.getData("IS_JOB_VEHICLE_SPAWN_TRIGGER") == true)
            {
                player.setData("InJobVehicleSpawn", null);
            }
        }

        public static string RandomGoPostalDropOff(Client player)
        {
            string dropofflocation = "";

            Random rnd = new Random();
            dropofflocation = "" + GoPostalDropOffs[rnd.Next(0, 29)];

            return dropofflocation;
        }

        public static string RandomMoneyTransportDropOff(Client player)
        {
            string dropofflocation = "";

            Random rnd = new Random();
            dropofflocation = "" + MoneyTransportDropOffs[rnd.Next(0, 10)];

            return dropofflocation;
        }

        public static string[] GoPostalDropOffs = new string[]
        {
            "-968.9243,434.8509,80.57148",
            "-1160.028,480.894,86.0938",
            "-1075.28,-1027.067,4.545239",
            "-1367.612,611.9219,133.8821",
            "-1014.643,819.1169,172.378",
            "-494.4905,795.3749,184.3416",
            "-1255.358,-1331.536,4.080746",
            "330.161,-1844.808,27.74821",
            "552.2387,-1766.655,33.44263",
            "257.1364,-2023.559,19.26749",
            "85.03226,-1958.28,21.12164",
            "980.542,2667.313,40.06124",
            "-131.516,-1462.39,36.99214",
            "1335.945,-1579.575,54.06731",
            "1192.487,-1622.943,45.22145",
            "-682.6616,5770.819,17.511",
            "1315.552,-1733.128,54.69999",
            "-346.9008,6224.233,31.51527",
            "31.29926,6596.49,32.79631",
            "1220.852,-668.7648,63.52042",
            "1046.336,-497.0342,64.07932",
            "1728.917,3851.208,34.7838",
            "1388.979,-569.4628,74.49641",
            "1918.822,3913.39,33.44163",
            "929.4107,-639.4225,58.24129",
            "191.285,3082.439,43.47285",
            "-1109.52,-1526.735,6.779534",
            "-1114.765,-1579.956,8.679536",
            "-1661.8,-378.0461,48.91005",
            "-928.8743,19.35282,48.12824"
        };

        public static string[] MoneyTransportDropOffs = new string[]
        {
            "-190.6474, -865.4128, 28.7469",
            "-711.2708, -826.8418, 22.9875",
            "-1319.516, -838.6011, 16.3819",
            "-3240.488, 993.4891, 11.8068",
            "-1402.8, -98.51221, 51.90395",
            "87.50439, -5.274318, 67.80811",
            "230.4264, 346.0845, 105.0023",
            "517.0613, -159.7098, 55.77721",
            "-388.5413, 6042.505, 30.88178",
            "180.9279, 6631.22, 30.97577",
            "1707.841, 6421.667, 32.01794"
        };
    }
}
