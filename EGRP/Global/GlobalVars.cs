using GrandTheftMultiplayer.Server.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGRPProject.Global
{
    class GlobalVars
    {
        public static string ListeningServer;
        public static int ListeningPort;
        public static string ListeningString;

        public static string WebServer;
        public static int WebServerPort;
        public static string WebServerConnectionString;

        public static string WebRTCServer;
        public static int WebRTCServerPort;
        public static string WebRTCServerConnectionString;



        public static string ServerName = "Escudo Gaming - Roleplay";
        public static PedHash DefaultPedModel = PedHash.DrFriedlander;
    }
}
