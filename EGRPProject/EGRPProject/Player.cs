using GrandTheftMultiplayer.Server.Elements;

namespace EGRPProject
{
    public class Player
    {
        public Player(Client player, int uuid, string username, string password)
        {
            Uuid = uuid;
            Username = username;
            Password = password;
        }

        public int Uuid { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
}
