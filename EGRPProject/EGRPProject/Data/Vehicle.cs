using GrandTheftMultiplayer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGRPProject.Data
{
    class Vehicle
    {
        public int ID { get; set; }

        public VehicleHash Model { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public float Rot { get; set; }
        public int Color1 { get; set; }
        public int Color2 { get; set; }
        public bool Respawnable { get; set; }
        public bool JobVehicle { get; set; }

        public Vehicle()
        {

        }
    }
}
