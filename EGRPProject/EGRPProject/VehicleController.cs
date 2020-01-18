using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;

namespace EGRPProject
{
    public class VehicleController : Script
    {
        private void OnPlayerExitVehicle(Client player, NetHandle vehicle)
        {
            API.triggerClientEvent(player, "HIDE_VEHICLE_HUD");
        }
    }
}
