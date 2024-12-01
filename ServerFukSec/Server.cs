public class Server
{
    private NetServer _netServer;                     // 네트워크 서버
    private Dictionary<int, UserPeer> _connectedClients;  // 연결된 클라이언트
    private Dictionary<int, GameRoom> _gameRooms;         // 게임 룸 관리
    private int _nextRoomID = 1;                          // 다음 GameRoom ID
    private int _nextUID = 1;                             // 다음 클라이언트 UID

    public Server()
    {
        _netServer = new NetServer();
        _netServer.onClientConnected += OnClientConnected;
        _connectedClients = new Dictionary<int, UserPeer>();
        _gameRooms = new Dictionary<int, GameRoom>();
    }

    public void Start()
    {
        Console.WriteLine("Server started");
        StartMessageDispatcher();
        _netServer.Start(10);
    }

    public void Stop()
    {
        Console.WriteLine("Server stopped");
        _netServer.End();
    }

    private void OnClientConnected(UserToken userToken)
    {
        int uid = _nextUID++;
        string id = $"Player{uid}";
        UserPeer userPeer = new UserPeer(userToken, uid, id, this);

       // Console.WriteLine($"Peer {userPeer.ID}");

        _connectedClients[uid-1] = userPeer;

      //  userToken.StartReceive();


        PacketReqUserInfo packet = new PacketReqUserInfo();
        packet.uid = uid;
        userPeer.Send(packet);
        Console.WriteLine("Send packet");

        /*        var gameRoom = FindOrCreateGameRoom();
                gameRoom.AddPlayer(userPeer);
                userPeer.SetGameRoom(gameRoom);
        

                if (gameRoom.IsFull)
                {
                    gameRoom.CheckReadyState();
                }*/
    }

    private GameRoom FindOrCreateGameRoom()
    {
        foreach (var room in _gameRooms.Values)
        {
            if (!room.IsFull)
                return room;
        }

        var newRoom = new GameRoom(_nextRoomID++);
        _gameRooms[newRoom.RoomID] = newRoom;
        Console.WriteLine($"Created GameRoom {newRoom.RoomID}");
        return newRoom;
    }

    public void PrintConnectedClients()
    {
        if (_connectedClients.Count == 0)
        {
            Console.WriteLine("현재 접속한 클라이언트가 없습니다.");
        }
        else
        {
            Console.WriteLine("현재 접속한 클라이언트 목록:");
            foreach (var client in _connectedClients.Values)
            {
                Console.WriteLine($"- 클라이언트 ID: {client.ID}, UID: {client.UID}");
            }
        }
    }

    public void SendAll(Packet packet)
    {
        foreach (UserPeer user in _connectedClients.Values)
        {
            user.Send(packet);
        }
    }

    public void SendUserList()
    {
        PacketAnsUserList sendPacket = new PacketAnsUserList();
        sendPacket.userNum = _connectedClients.Count;
        for (int i = 0; i < _connectedClients.Count; i++)
        {
            sendPacket.userInfos[i] = new UserInfo();
            sendPacket.userInfos[i].id = _connectedClients[i].ID;
            sendPacket.userInfos[i].uid = _connectedClients[i].UID;
        }
        SendAll(sendPacket);
    }



    /// <summary>
    /// 메시지 처리 스레드 시작
    /// </summary>
    private void StartMessageDispatcher()
    {
        var messageThread = new System.Threading.Thread(() =>
        {
            while (true)
            {
                PacketMessageDispatcher.Instance.Dispatch();
                System.Threading.Thread.Sleep(10); // 10ms 대기
                //Console.WriteLine("StartMsg");
            }
        })
        {
            IsBackground = true,
            Name = "MessageDispatcherThread"
        };
        messageThread.Start();
    }
}
