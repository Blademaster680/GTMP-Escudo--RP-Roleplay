using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;

namespace EGRPProject.Data
{
    public class Character
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Cash { get; set; }
        public int Bank { get; set; }

        public string Job { get; set; }
        public bool doingJob { get; set; }
        public NetHandle jobVehicle { get; set; }
        public int notInJobVehicle { get; set; }
    }
}
