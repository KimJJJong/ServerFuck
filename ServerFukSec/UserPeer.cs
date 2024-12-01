
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
    CS_ANS_USERINFO,         // Ŭ�� -> ���� : ���� ���� ����
    REL_GAME_READY,          // ���� -> Ŭ�� : ���� �غ� ���� ������
    CS_GAME_READY_OK,        // Ŭ�� -> ���� : ���� �غ� �Ϸ�
    REL_UNIT_SUMMONED,       // ���� -> Ŭ�� : ���� ��ȯ �Ϸ� ������
    REL_UNIT_ACTION,         // ���� -> Ŭ�� : ���� �ൿ �̺�Ʈ ������
    REL_GRID_UPDATE,         // ���� -> Ŭ�� : �� ���� ���� ������Ʈ */
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

                    //������ ���� �ʿ� server
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
/// Ŭ���̾�Ʈ ���� ���� ó��
/// </summary>
public void Remove()
    {
        Console.WriteLine($"Player {UID} disconnected.");
        _userToken?.Close();
    }
}
