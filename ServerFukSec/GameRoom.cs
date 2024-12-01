public class GameRoom
{
    public int RoomID { get; private set; }
    public List<UserPeer> Players { get; private set; }
    private bool _isGameStarted;

    public GameRoom(int roomId)
    {
        RoomID = roomId;
        Players = new List<UserPeer>();
        _isGameStarted = false;
    }

    public bool IsFull => Players.Count >= 2;

    public void AddPlayer(UserPeer player)
    {
        if (IsFull)
        {
            Console.WriteLine($"GameRoom {RoomID} is full.");
            return;
        }
        Players.Add(player);
        Console.WriteLine($"Player {player.UID} joined GameRoom {RoomID}.");
    }

    public void RemovePlayer(UserPeer player)
    {
        Players.Remove(player);
        Console.WriteLine($"Player {player.UID} left GameRoom {RoomID}.");
    }

    public void CheckReadyState()
    {
        if (Players.Count == 2 && Players.TrueForAll(p => p.GameReady))
        {
            StartGame();
        }
    }

    private void StartGame()    //좀 삐리함
    {
        _isGameStarted = true;
        var startPacket = new PacketGameStart();
        foreach (var player in Players)
        {
         //   startPacket.AddPlayerInfo(player.UID, player.ID);
        }
        Broadcast(startPacket);
    }

    private void Broadcast(Packet packet)
    {
        foreach (var player in Players)
        {
            player.Send(packet);
        }
    }
}
