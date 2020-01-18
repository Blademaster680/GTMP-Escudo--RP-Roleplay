using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Shared;
using System.Collections.Generic;

namespace EGRPProject.Jobs
{
    class Commands : Script
    {
        [Command("joinjob")]
        public void COMMAND_JOINJOB(Client player)
        {
            if (player.getData("InJobJoin") != null)
            {
                if (player.getData("CharJob") == "Unemployed")
                {
                    string job = player.getData("InJobJoin");
                    player.setData("CharJob", job);
                    player.sendChatMessage("You have joined the " + job + " job");
                }
                else player.sendChatMessage("You already have a job. Use /quitjob to resign");
            }
            else player.sendChatMessage("You are not at a job join location");
        }

        [Command("quitjob")]
        public void COMMAND_QUITJOB(Client player)
        {
            if (player.getData("CharJob") == "Unemployed")
                player.sendChatMessage("You are already unemployed");
            else
            {
                player.setData("CharJob", "Unemployed");
                player.sendChatMessage("You are now unemployed");
            }
        }
    }
}
