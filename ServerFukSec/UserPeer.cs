
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;

public class UserPeer : IPeer
{
    private UserToken _userToken;

    private Server _server;

    private int _uid;
    private string _id;
    private bool _gameReady;
    public int UID
    {
        get => _uid;
        set => _uid = value;
    }
    public string ID
    {
        get => _id;
        set => _id = value;
    }
    public bool GameReady => _gameReady;

   
    private GameRoom _gameRoom;

    public UserPeer(UserToken userToken, int uid, string id, Server server)
    {
        _userToken = userToken;
        _uid = uid;
        _id = id;
        _gameReady = false;
        _server = server;
        
        _userToken.SetPeer(this);
    }

    public void SetGameRoom(GameRoom gameRoom)
    {
        _gameRoom = gameRoom;
    }
/*
    CS_ANS_USERINFO,         // 클라 -> 서버 : 유저 정보 응답
    REL_GAME_READY,          // 서버 -> 클라 : 게임 준비 상태 릴레이
    CS_GAME_READY_OK,        // 클라 -> 서버 : 게임 준비 완료
    REL_UNIT_SUMMONED,       // 서버 -> 클라 : 유닛 소환 완료 릴레이
    REL_UNIT_ACTION,         // 서버 -> 클라 : 유닛 행동 이벤트 릴레이
    REL_GRID_UPDATE,         // 서버 -> 클라 : 맵 격자 상태 업데이트 */
    public void ProcessMessage(short protocolID, byte[] buffer)
    {
        Console.WriteLine($"Protocol ID : {(int)protocolID}");
        switch ((EProtocolID)protocolID)
        {
            case EProtocolID.CS_ANS_USERINFO:
                {
                    PacketAnsUserInfo packet = new PacketAnsUserInfo();
                    packet.ToPacket(buffer);
                    _id=packet.id;

                    Console.WriteLine("CS_ANS_USERINFO " + packet.id );

                    _server.SendUserList();
                }
                break;
            

            case EProtocolID.REL_GAME_READY:
                {
                    PacketGameReady packet = new PacketGameReady();
                    packet.ToPacket(buffer);

                    //서버로 전송 필요 server
                }
                break;
            case EProtocolID.CS_GAME_READY_OK:
                _gameReady = true;
                _gameRoom.CheckReadyState();
                break;
            default:
                Console.WriteLine($"Unknown protocol ID: {protocolID}");
                break;
        }
    }

    public void Send(Packet packet)
    {
        _userToken.Send(packet);
    }


/// <summary>
/// 클라이언트 연결 종료 처리
/// </summary>
public void Remove()
    {
        Console.WriteLine($"Player {UID} disconnected.");
        _userToken?.Close();
    }
}
